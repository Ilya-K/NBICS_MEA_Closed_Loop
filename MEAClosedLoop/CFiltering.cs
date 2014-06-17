using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Neurorighter;
using System.Diagnostics;
using Common;

namespace MEAClosedLoop
{
  using TData = System.Double;
  using TTime = System.UInt64;
  using TStimIndex = System.Int16;
  using TAbsStimIndex = System.UInt64;
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
    //private Queue<TFltDataPacket> m_filteredQueue;
    //private AutoResetEvent m_notEmpty;
    private CStimDetectShift m_stimDetector;
    public CStimDetectShift StimDetector { get { return m_stimDetector; } }
    private Dictionary<int, ButterworthFilter> m_bandpassFilters = null;
    private Dictionary<int, LocalFit> m_salpaFilters = null;
    private OnStreamKillDelegate m_onStreamKill = null;
    public OnStreamKillDelegate OnStreamKill { set { m_onStreamKill = value; } }
    //private OnDataAvailableDelegate m_onDataAvailable = null;
    //public OnDataAvailableDelegate OnDataAvailable { set { m_onDataAvailable = value; } }

    public int NChannels { get { return m_inputStream.NChannels; } }
    public List<int> ChannelList { get { return m_inputStream.ChannelList; } }
    private TTime m_timeStamp = 0;                                              // Start time of the last processed packet
    private TTime m_localTimeStamp = 0;
    private object m_timeLock = new object();
    public ulong TimeStamp { get { lock (m_timeLock) return m_timeStamp; } }
    private volatile bool m_kill;

    public delegate void ConsumerDelegate(TFltDataPacket data);
    private List<ConsumerDelegate> m_consumerList = null;



    public delegate void StimulTimeDelegate(List<TAbsStimIndex> stimul);
    private List<StimulTimeDelegate> m_stimulCallback = null;

    private List<TStimIndex> m_noArtifacts = new List<TStimIndex>();
    // [DEBUG STIM LOAD VARIABLES]

    private const int DAQ_FREQ = 25000;
    private const int TIME_MULT = DAQ_FREQ / 1000; //!< Коэффициент перевода времени в милисекунды
    private const TTime SINGLE_STIM_PERIOD = 10000 * TIME_MULT; //!< Период между одиночными стимулами (stimType = 1)
    private const int MULTI_PACK_NUM = 6; //!< Количество стимулов в пачке
    private const UInt16 MULTI_INNER_PERIOD = 10 * TIME_MULT; //!< Период между соседними стимулами внутри пачки
    private const UInt16 MULTI_PACK_PERIOD = 300 * TIME_MULT; //!< Период между пачками
    private const UInt16 MAX_TIME_NOISE = 9 * TIME_MULT; //!< Максимальный разброс приблизительного времени стимуляции
    private const TTime MAX_FILE_LENGTH = 9000000 * TIME_MULT; //!< Максимальная длина входного файла 

    TTime StartStimTime = 4210; // время начала стимуляций
    int StimType = 2;
    private List<TStimGroup> sl_groups; //!< Точный список моментов стимуляции
    //[DEBUG STIM LOAD VARIABLES /]

    // [/DEBUG]
    public int m_Count = 0;

    public void AddDataConsumer(ConsumerDelegate consumer)
    {
      lock (m_consumerList)
      {
        if (m_consumerList.Contains(consumer)) return;
        m_consumerList.Add(consumer);
      }
    }

    public void RemoveDataConsumer(ConsumerDelegate consumer)
    {
      lock (m_consumerList) m_consumerList.Remove(consumer);
      return;
    }

    public void AddStimulConsumer(StimulTimeDelegate stimConsumer)
    {
      lock (m_stimulCallback)
      {
        if (m_stimulCallback.Contains(stimConsumer)) return;
        m_stimulCallback.Add(stimConsumer);
      }
    }

    public void RemoveStimulConsumer(StimulTimeDelegate stimConsumer)
    {
      lock (m_stimulCallback) m_stimulCallback.Remove(stimConsumer);
      return;
    }


    public CFiltering(CInputStream inputStream, CStimDetectShift stimDetector, SALPAParams parSALPA, BFParams parBF)
    {
      m_inputStream = inputStream;
      m_inputStream.OnStreamKill = Dismiss;
      m_inputStream.ConsumerList.Add(ReceiveData);

      m_stimDetector = stimDetector;

      m_consumerList = new List<ConsumerDelegate>();
      m_stimulCallback = new List<StimulTimeDelegate>();

      // [TODO] Allow user to choose stimulus artifact detection channel
      //stimDetector.m_Artif_Channel = m_inputStream.ChannelList[0]; // old fixed
      stimDetector.m_Artif_Channel = MEA.NAME2IDX[33];
      ///[DEBUG STIM]
      ///

      //sl_groups = GenStimulList(StartStimTime, StimType, MAX_FILE_LENGTH);




      //ACHTUNG IDIOT WRITES HERE!!!!
      //foreach (TStimGroup stim in sl_groups)
      //{
        //m_stimDetector.SetExpectedStims(stim);

      //}
      ///[/DEBUG STIM]
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

      //m_filteredQueue = new Queue<TFltDataPacket>();
      //m_notEmpty = new AutoResetEvent(false);
      //      Thread t = new Thread(new ThreadStart(DoFiltering));
      m_kill = false;
      //      t.Start();
    }

    public void ConfigureSALPA()
    {

    }

    /*
    public TFltDataPacket WaitData()
    {
      m_Count++;
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

      lock (m_timeLock)
      {
        m_timeStamp += (TTime)m_sentPacketLength;
        m_sentPacketLength = dataPacket[dataPacket.Keys.ElementAt(0)].Length;
        return dataPacket;
      }
    }
    */
    private void PushToSalpa(TRawDataPacket packet, List<TStimIndex> stimIndices)
    {
      TFltDataPacket filteredData = new TFltDataPacket(packet.Count);
      // Fill keys to enable parallel foreach on the next step
      foreach (int channel in packet.Keys) filteredData[channel] = null;
      if (stimIndices == null)
      {
        packet.Keys.AsParallel().ForAll(channel =>
        {
          filteredData[channel] = m_salpaFilters[channel].filter(packet[channel], null);
        });
      }
      else
      {
        Dictionary<int, List<TStimIndex>> parStimInd = new Dictionary<int, List<TStimIndex>>(packet.Count);
        // Make a parallel collection of the expected stimuli lists to enable parallel processing
        foreach (int channel in packet.Keys) parStimInd[channel] = new List<TStimIndex>(stimIndices);

        // Process all channels of the packet in parallel (using PLINQ)
        packet.Keys.AsParallel().ForAll(channel =>
        {
          filteredData[channel] = m_salpaFilters[channel].filter(packet[channel], parStimInd[channel]);
        });
      }

      PushToButterworth(filteredData);
    }

    private void PassBySalpa(TRawDataPacket packet)
    {
      TFltDataPacket filteredData = new TFltDataPacket(packet.Count);
      // Fill keys to enable parallel foreach on the next step
      foreach (int channel in packet.Keys) filteredData[channel] = null;

      // Without SALPA just copy data and change type to TData
      packet.Keys.AsParallel().ForAll(channel =>
      {
        filteredData[channel] = Array.ConvertAll(packet[channel], x => (TData)x);
      });
      PushToButterworth(filteredData);
    }

    // Получает очередной пакет входного потока, проверяет, ожидается ли стимуляция и фильтрует его Сальпой и/или Баттервортом
    /// <summary>
    /// Filter current data packet with the SALPA or/and Butterworth
    /// </summary>
    /// <param name="currPacket">Current packet of input stream</param>
    /// 
    private List<TStimGroup> GenStimulList(TTime start_time, Int32 stimType, TTime totalTime)
    {
      TTime timeIterator;
      TStimGroup newStim;
      List<TStimGroup> output = new List<TStimGroup>();

      switch (stimType)
      {
        case 1:
          newStim.count = 1;
          newStim.period = 0;
          for (timeIterator = start_time; timeIterator < totalTime; timeIterator += SINGLE_STIM_PERIOD)
          {
            newStim.stimTime = timeIterator;
            output.Add(newStim);
          }
          break;
        case 2:
          newStim.count = 6;
          newStim.period = MULTI_INNER_PERIOD;
          for (timeIterator = start_time; timeIterator < totalTime; timeIterator += MULTI_PACK_PERIOD)
          {
            newStim.stimTime = timeIterator;
            output.Add(newStim);
          }
          break;
      }
      return output;
    }
    public void ReceiveData(TRawDataPacket currPacket)
    {
      List<TAbsStimIndex> outputIndeces = new List<TAbsStimIndex>();
      if (m_prevPacket == null)
      {
        //в прошлый раз пакет не задерживался - обновляем текущее время
        m_localTimeStamp = m_inputStream.TimeStamp;
      }
      if (m_salpaFilters == null)
      {
        PassBySalpa(currPacket);
        return;
      }
      int currPacketLength = currPacket[currPacket.Keys.ElementAt(0)].Length;
      List<TStimIndex> stimIndices = m_noArtifacts;
      
      /*[DEBUG] SET TEST STIMS*/
      /*
      if (sl_groups[0].stimTime < m_inputStream.TimeStamp + (TTime)currPacketLength)
      {
        //[HARD DEBUG]
        m_stimDetector.SetExpectedStims(sl_groups[0]);
        sl_groups.RemoveAt(0);
      }
      */
      // Check here if we need to call the Stimulus Artifact Detector for the current packet
      // Returns true if the current packet is requred (stimulation might be expected in the next packet)
      if (m_stimDetector.IsDataRequired(m_inputStream.TimeStamp + (TTime)currPacketLength))
      {
        // Retuns null when we need to put off processing of the current packet until next packet arrived
        stimIndices = m_stimDetector.GetStims(currPacket);
      }
      if (stimIndices != null && stimIndices.Count > 0)
      {
        foreach (TStimIndex stim in stimIndices) outputIndeces.Add((TAbsStimIndex)stim + m_localTimeStamp);
        lock (m_stimulCallback)
        {
          if (m_stimulCallback.Count != 0)
          {
            foreach (StimulTimeDelegate consumer in m_stimulCallback) consumer(outputIndeces);
          }
        }
      }
      if (m_prevPacket != null)                 // В прошлый раз чего-то не нашли, а теперь, может быть, нашли
      {
        PushToSalpa(m_prevPacket, stimIndices); // поэтому проталкиваем предыдущий пакет вперёд
        m_prevPacket = null;
        PushToSalpa(currPacket, null);
        return;
      }
      if (stimIndices != null)                  // Нормальный случай. Нашли, что ожидалось, или не нашли, что не ожидалось
      {
        PushToSalpa(currPacket, stimIndices);
      }
      else                                      // Put off processing of the current packet until stimuli artifacts are found
      {
        m_prevPacket = currPacket;
      }

    }

    private void PushToButterworth(TFltDataPacket filteredData)
    {
      if (m_bandpassFilters != null)
      {
        filteredData.Keys.AsParallel().ForAll(channel =>
        {
          m_bandpassFilters[channel].filterData(filteredData[channel]);
        });
      }

      lock (m_consumerList)
      {
        if (m_consumerList.Count != 0)
        {
          foreach (ConsumerDelegate consumer in m_consumerList) consumer(filteredData);
        }
      }

      lock (m_timeLock)
      {
        m_timeStamp += (TTime)filteredData[filteredData.Keys.ElementAt(0)].Length;
      }

      //lock (m_filteredQueue) m_filteredQueue.Enqueue(filteredData);

      // Callback
      //if (m_onDataAvailable != null) m_onDataAvailable(filteredData);

      //m_notEmpty.Set();
    }

    private void Dismiss()
    {
      m_kill = true;
      if (m_onStreamKill != null) m_onStreamKill();
      //m_notEmpty.Set();
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
