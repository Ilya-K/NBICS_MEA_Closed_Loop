using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Neurorighter;
using Common;

namespace MEAClosedLoop
{
  using TData = System.Double;
  using TStimIndex = System.Int16;
  using TRawDataPacket = Dictionary<int, ushort[]>;
  using TFltDataPacket = Dictionary<int, System.Double[]>;
  using StimuliList = List<TStimGroup>;

  public class CFiltering
  {
    const int DEFAULT_VALUE = 32767;
    const int MIN_PACKET_SIZE = 150;

    private CInputStream m_inputStream;
    private TRawDataPacket m_prevPacket;
    //private TRawDataPacket m_currPacket;
    //private TFltDataPacket m_filteredData;
    private Queue<TFltDataPacket> m_filteredQueue;
    private AutoResetEvent m_notEmpty;
    private CStimDetector m_stimDetector;
    private int m_artifChannel;
    private StimuliList m_expectedStims;
    private Dictionary<int, ButterworthFilter> m_bandpassFilters = null;
    private Dictionary<int, LocalFit> m_salpaFilters = null;
    private OnStreamKillDelegate m_onStreamKill = null;
    public OnStreamKillDelegate OnStreamKill { set { m_onStreamKill = value; } }
    private OnDataAvailableDelegate m_onDataAvailable = null;
    public OnDataAvailableDelegate OnDataAvailable { set { m_onDataAvailable = value; } }

    public int NChannels { get { return m_inputStream.NChannels; } }
    public List<int> ChannelList { get { return m_inputStream.ChannelList; } }
    public ulong TimeStamp { get { return m_inputStream.TimeStamp; } }
    private volatile bool m_kill;

    // [DEBUG]

    public CFiltering(CInputStream inputStream, SALPAParams parSALPA, BFParams parBF)
    {
      m_inputStream = inputStream;
      m_inputStream.OnStreamKill = Dismiss;
      m_inputStream.ConsumerList.Add(ReceiveData);


      // [TODO] Get parameters from UI and save them in Settings
      m_stimDetector = new CStimDetector(15, 35, 150);
      // [TODO] Allow user to choose stimulus artifact detection channel
      m_artifChannel = m_inputStream.ChannelList[0];
      m_expectedStims = null;

      if (parSALPA != null)
      {
        m_salpaFilters = new Dictionary<int, LocalFit>(inputStream.NChannels);
        foreach (int channel in inputStream.ChannelList)
        {
          m_salpaFilters[channel] = new LocalFit(parSALPA.thresh[channel],
                                                 parSALPA.length_sams,
                                                 parSALPA.blank_sams,
                                                 parSALPA.ahead_sams,
                                                 parSALPA.asym_sams,
                                                 parSALPA.railHigh,
                                                 parSALPA.railLow,
                                                 parSALPA.forcepeg_sams);
        }
      }

      if (parBF != null)
      {
        m_bandpassFilters = new Dictionary<int, ButterworthFilter>(inputStream.NChannels);
        foreach (int channel in inputStream.ChannelList)
        {
          m_bandpassFilters[channel] = new ButterworthFilter(parBF.filterOrder,
                                                             parBF.samplingFreq,
                                                             parBF.lowCutFreq,
                                                             parBF.highCutFreq,
                                                             parBF.dataBufLength);
        }
      }

      m_filteredQueue = new Queue<TFltDataPacket>();
      m_notEmpty = new AutoResetEvent(false);
//      Thread t = new Thread(new ThreadStart(DoFiltering));
      m_kill = false;
//      t.Start();
    }

    public void ConfigureSALPA()
    {

    }

    public TFltDataPacket WaitData()
    {
      TFltDataPacket dataPacket = null;
      do
      {
        m_notEmpty.WaitOne();
        lock (m_filteredQueue)
        {
          if (m_filteredQueue.Count > 0)
          {
            dataPacket = m_filteredQueue.Dequeue();
            if (m_filteredQueue.Count > 0) m_notEmpty.Set();
          }
          else if (m_kill) break;
        }
      } while (dataPacket == null);

      return dataPacket;
    }

    public void ReceiveData(TRawDataPacket currPacket)
    {
      TFltDataPacket filteredData = new TFltDataPacket(currPacket.Count);
      // Fill keys to enable parallel foreach on the next step
      foreach (int channel in currPacket.Keys) filteredData[channel] = null;

      if ((m_salpaFilters != null) && (m_stimDetector != null))
      {
        // Prepare "previous" packet for the first packet processing
        if (m_prevPacket == null)
        {
          m_prevPacket = new TRawDataPacket(currPacket.Count);
          foreach (int channel in currPacket.Keys) m_prevPacket[channel] = new ushort[MIN_PACKET_SIZE];
          m_prevPacket.Keys.AsParallel().ForAll(channel => Helpers.PopulateArray<ushort>(m_prevPacket[channel], currPacket[channel][0]));
        }
        // [TODO] Check here if we need to call Stim Detector now
        // if(IsStimulusExpected(timestamp, m_expectedStims) {
        List<TStimIndex> stimIndices = m_stimDetector.Detect(currPacket[m_artifChannel], m_expectedStims);
        Dictionary<int, List<TStimIndex>> parStimInd = new Dictionary<int, List<TStimIndex>>(currPacket.Count);
        foreach (int channel in currPacket.Keys) parStimInd[channel] = new List<TStimIndex>(stimIndices);
        
        // [DONE] Add multithreading here // PLINQ
        m_prevPacket.Keys.AsParallel().ForAll(channel =>
        {
          // [TODO] Make SALPA with TRawData input and eliminate the next line
          filteredData[channel] = Array.ConvertAll(m_prevPacket[channel], x => (TData)x);
          filteredData[channel] = m_salpaFilters[channel].filter(filteredData[channel], parStimInd[channel]);
          //filteredData[channel] = m_salpaFilters[channel].filter(filteredData[channel], stimIndices);
        });
      }
      else
      {
        currPacket.Keys.AsParallel().ForAll(channel =>
        {
          filteredData[channel] = Array.ConvertAll(currPacket[channel], x => (TData)x);
        });
      }

      if (m_bandpassFilters != null)
      {
        // [DONE] Add multithreading here
        currPacket.Keys.AsParallel().ForAll(channel =>
        {
          m_bandpassFilters[channel].filterData(filteredData[channel]);
        });
      }
      lock (m_filteredQueue) m_filteredQueue.Enqueue(filteredData);

      // Callback
      if (m_onDataAvailable != null) m_onDataAvailable(filteredData);

      m_notEmpty.Set();
      
      m_prevPacket = currPacket;
    }

    private void Dismiss()
    {
      m_kill = true;
      if (m_onStreamKill != null) m_onStreamKill();
      m_notEmpty.Set();
    }

    // There are some obsolete functions here ************************************************
    // Obsolete
#if false
    public void DoFilteringOld()
    {
      do
      {
        // m_prevPacket = m_currPacket;
        m_currPacket = m_inputStream.WaitData();
        if (m_kill) break;    // If we've caught kill signal in WaitData()

        TFltDataPacket filteredData = new TFltDataPacket(m_currPacket.Count);
        // Fill keys to enable parallel foreach on the next step
        foreach (int channel in m_currPacket.Keys)
        {
          filteredData[channel] = null;
        }
        // [DONE] Add multithreading here
        m_currPacket.Keys.AsParallel().ForAll(channel =>
        {
          filteredData[channel] = Array.ConvertAll(m_currPacket[channel], x => (TData)x);
          m_bandpassFilters[channel].filterData(filteredData[channel]);
        });

        /*
        foreach (int channel in m_currPacket.Keys)
        {
          // [ToDo] Если потребуется уменьшить время реакции, то порезать блоки на куски тут
          m_filteredData[channel] = Array.ConvertAll(m_currPacket[channel], x => (TData)x);
          m_bandpassFilters[channel].filterData(m_filteredData[channel]);

          // Dummy
          // ushort[] filtered;
          // Filter(m_currPacket[channel], m_prevPacket[channel], out filtered);
          // m_filteredData[channel] = filtered;
        }
        */

        lock (m_filteredQueue) m_filteredQueue.Enqueue(filteredData);

        if (m_onDataAvailable != null) m_onDataAvailable(filteredData);

        m_notEmpty.Set();

      } while (!m_kill);
    }

    // Obsolete
    private void Filter(ushort[] data, ushort[] prevData, out ushort[] outData)
    {
      const int FILTER_LENGTH = 32;
      outData = new ushort[data.Length];
      int prevOffset = prevData.Length - FILTER_LENGTH;

      for (int i = 0; i < FILTER_LENGTH; ++i)
      {
        outData[i] = (ushort)((data[i] + prevData[prevOffset + i]) / 2);
      }
      for (int i = FILTER_LENGTH; i < data.Length; ++i) 
      {
        outData[i] = (ushort)((data[i] + data[i - FILTER_LENGTH]) / 2);
      }
    }
#endif
  }
}
