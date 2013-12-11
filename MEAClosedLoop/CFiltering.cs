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
  using TTime = System.UInt64;
  using TStimIndex = System.Int16;
  using TRawDataPacket = Dictionary<int, ushort[]>;
  using TFltDataPacket = Dictionary<int, System.Double[]>;

  public class CFiltering
  {
    const int DEFAULT_VALUE = 32767;
    const int MIN_PACKET_SIZE = 150;

    private CInputStream m_inputStream;
    private TRawDataPacket m_prevPacket = null;
    //private TRawDataPacket m_currPacket;
    //private TFltDataPacket m_filteredData;
    private Queue<TFltDataPacket> m_filteredQueue;
    private AutoResetEvent m_notEmpty;
    private CStimDetector m_stimDetector;
    public CStimDetector StimDetector { get { return m_stimDetector; } }
    private bool m_missedStimulus = false;
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

    public CFiltering(CInputStream inputStream, CStimDetector stimDetector, SALPAParams parSALPA, BFParams parBF)
    {
      m_inputStream = inputStream;
      m_inputStream.OnStreamKill = Dismiss;
      m_inputStream.ConsumerList.Add(ReceiveData);

      m_stimDetector = stimDetector;

      // [TODO] Allow user to choose stimulus artifact detection channel
      stimDetector.ArtifactChannel = m_inputStream.ChannelList[0];

      if (parSALPA != null)
      {
        if (m_stimDetector == null)
        {
          throw new ArgumentException("Unable to use SALPA without a Stimulus Artifact Detector", "stimDetector");
        }
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

    // Получает очередной пакет входного потока, проверяет, ожидается ли стимуляция и фильтрует его Сальпой и/или Баттервортом
    /// <summary>
    /// Filter current data packet with the SALPA or/and Butterworth
    /// </summary>
    /// <param name="currPacket">Current packet of input stream</param>
    public void ReceiveData(TRawDataPacket currPacket)
    {
      TFltDataPacket filteredData = new TFltDataPacket(currPacket.Count);
      // Fill keys to enable parallel foreach on the next step
      foreach (int channel in currPacket.Keys) filteredData[channel] = null;

      if (m_salpaFilters != null)
      {
        int currPacketLength = currPacket[currPacket.Keys.ElementAt(0)].Length;
        List<TStimIndex> stimIndices = null;

        if (m_missedStimulus)
        {
          stimIndices = m_stimDetector.Detect(currPacket);

          // Stimulation hasn't been found in the two subsequent packets
          if (stimIndices == null) throw new Exception("Loop is out of sync!");
        }
        else
        {
          // Check here if we need to call the Stim Detector for the current packet
          if (m_stimDetector.IsStimulusExpected(m_inputStream.TimeStamp + (TTime)currPacketLength))
          {
            stimIndices = m_stimDetector.Detect(currPacket);
            if (stimIndices == null)
            {
              m_missedStimulus = true;
              m_prevPacket = currPacket;
              // Put off processing of the current packet until a stimuli artifacts are found
              return;
            }
          }
        }
        // Сюда попадаем, либо в обычном случае, либо когда найдены артефакты во втором пакете в случае m_missedStimulus

        // В обычном случае, если артефакты не ожидались, сразу вызываем SALPA без списка артефактов
        // If stimuli are not expected, just run SALPA without expected stimuli list.
        if (stimIndices == null)
        {
          // Process all channels in parallel (using PLINQ)
          currPacket.Keys.AsParallel().ForAll(channel =>
          {
            filteredData[channel] = m_salpaFilters[channel].filter(currPacket[channel], null);
          });
        }
        else  // Если артефакты ожидались и найдены, обрабатываем текущий пакет
        {
          Dictionary<int, List<TStimIndex>> parStimInd = new Dictionary<int, List<TStimIndex>>(currPacket.Count);
          // Make a parallel collection of the expected stimuli lists to enable parallel processing
          foreach (int channel in currPacket.Keys) parStimInd[channel] = new List<TStimIndex>(stimIndices);

          // Если предыдущий пакет ещё не обработан, то значит артефакты ожидались в нём, а найдены в текущем. Обрабатываем оба пакета
          if (m_prevPacket != null)
          {
            // Process all channels of the previous packet in parallel (using PLINQ)
            m_prevPacket.Keys.AsParallel().ForAll(channel =>
            {
              filteredData[channel] = m_salpaFilters[channel].filter(currPacket[channel], parStimInd[channel]);
            });
            PushForth(filteredData);
            m_prevPacket = null;
          }

          // Process all channels in parallel (using PLINQ)
          currPacket.Keys.AsParallel().ForAll(channel =>
          {
            filteredData[channel] = m_salpaFilters[channel].filter(currPacket[channel], parStimInd[channel]);
          });

          // Обработали пропущенный на прошлой итерации пакет
          m_missedStimulus = false;
        }
      }
      else
      {
        // Without SALPA just copy data and change type to TData
        currPacket.Keys.AsParallel().ForAll(channel =>
        {
          filteredData[channel] = Array.ConvertAll(currPacket[channel], x => (TData)x);
        });
      }

      PushForth(filteredData);
    }

    private void PushForth(TFltDataPacket filteredData)
    {
      if (m_bandpassFilters != null)
      {
        filteredData.Keys.AsParallel().ForAll(channel =>
        {
          m_bandpassFilters[channel].filterData(filteredData[channel]);
        });
      }
      lock (m_filteredQueue) m_filteredQueue.Enqueue(filteredData);

      // Callback
      if (m_onDataAvailable != null) m_onDataAvailable(filteredData);

      m_notEmpty.Set();
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
