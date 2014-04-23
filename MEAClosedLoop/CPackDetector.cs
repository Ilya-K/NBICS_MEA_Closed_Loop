﻿#define DEBUG_SPIKETRAINS

using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;


namespace MEAClosedLoop
{
  using TTime = System.UInt64;
  using TData = System.Double;
  using TFltDataPacket = Dictionary<int, System.Double[]>;

  public class CPackDetector
  {
    private const int MAX_LEADING_SPIKETRAIN = 20 * Param.MS;     // Насколько один спайк-трэйн может выдаваться вперёд пачки.
    // В текущем алгоритме не может быть больше 100 - Param.PRE_SPIKE = 92 мс (2300 отсчетов)

    // Алгоритм выделения спайк-трэйнов
    // Считаем дисперсию сигнала в экспоненциальном окне с временем релаксации 250 точек (10 мс, окно ~20 мс).
    // Считаем шум стационарным, т.е. дисперсия шума должна меняться слабо и медленно. Поэтому любое быстрое увеличение дисперсии сигнала будем считать началом пачки.
    // Для того чтобы обнаружить изменение дисперсии шума будем считать дисперсию дисперсии шума D[D[data]] в том же экспоненциальном окне.
    // Если дисперсия сигнала D[data] превысит среднюю дисперсию шума E[D[data]] на 

    private class CSpikeTrainDetector
    {
      private const TData ZERO_LEVEL = 1;                   // Approximately 1 bit
      private const TData THRESH1 = 0.14;
      private const TData THRESH2 = 1.0;
      private const TData SPIKE_THRESH = 3.5;               // *sigma 
      private const TData DECAY = 0.98;
      private const int PRE_DETECT_TIME = 10 * Param.MS;    // Интервал перед выполнением критерия начала спайк-трэйна, в которм будем искать первый спайк
      private const int ZERO_COUNT = 3;                     // How many sequental zero points have got to consider signal = 0;
      private const int MAX_SPIKE_QUEUE = 10;               // Length of queue for pre-train spikes
      private readonly CSpikeTrainFrame CONTINUE_TRAIN;     // A return value inside a spike-train

      private enum State
      {
        Zero = 0,
        Noise,
        Train,
        WarmUp
      }
      private State m_state;

      private TTime m_absTime = 0;
      private TTime m_endTime = 0;
      private Int16 m_channel;
      
      // Short term mean and variance calculator
      private CCalcExpWndSE m_calcShortSE = new CCalcExpWndSE((20 * Param.MS) / 2);       // 2*tau = 20ms (500 samples)

      // Second order mean and variance of the short term variance of a signal
      private CCalcExpWndSE m_calcShortSE2 = new CCalcExpWndSE((2000 * Param.MS) / 2);    // 2*tau = 2s (50000 samples)

      // Long term mean and variance calculator. Used to estimate noise level
      private CCalcExpWndSE m_calcNoiseSE = new CCalcExpWndSE((2000 * Param.MS) / 2);     // 2*tau = 2s (50000 samples)

      // Long term mean and variance calculator. Used to estimate signal variance inside packs
      private CCalcExpWndSE m_calcPackSE = new CCalcExpWndSE((2000 * Param.MS) / 2);      // 2*tau = 2s (50000 samples)

      // Moving average of the long term variance with exponential window ~2s
      private CCalcExpWndSE m_calcNoiseSE2 = new CCalcExpWndSE((2000 * Param.MS) / 2);    // 2*tau = 2s (50000 samples)

      // Moving average with exponential window
      private CExpAvg m_expAvg = new CExpAvg(167);                              // Window width ~500 packets (50s)
      
      private TData m_hpf = 0;      // High Pass Filter
      private TData m_maOld = 0;
      private bool m_packFound = false;
      private bool m_warmedUp = false;
      private bool m_inSpike = false;
      private int m_zeroCount;
      private int m_warmUpCount;
      private TData[] m_prevPacket;
      private Queue<TTime> m_preSpikes;
      private TTime m_trainStartTime;

      // [DEBUG]
#if DEBUG_SPIKETRAINS
      private List<CSpikeTrainFrame> m_dbgTrainList = new List<CSpikeTrainFrame>();
      public List<CSpikeTrainFrame> GetSpikeTrainListDbg()
      {
        lock(m_dbgTrainList) return new List<CSpikeTrainFrame>(m_dbgTrainList);
      }
      public void ClearSpikeTrainListDbg()
      {
        lock (m_dbgTrainList)
        {
          if (m_dbgTrainList.Count == 0) return;
          CSpikeTrainFrame lastOne = m_dbgTrainList[m_dbgTrainList.Count - 1];
          m_dbgTrainList.Clear();
          if (m_dbgTrainList.Count % 2 == 0)
          {
            m_dbgTrainList.Add(lastOne);
          }
        }
      }
#endif

      public CSpikeTrainDetector(Int16 channel)
      {
        m_channel = channel;
        m_state = State.WarmUp;
        m_zeroCount = ZERO_COUNT;
        m_warmUpCount = 500;
        m_prevPacket = new TData[PRE_DETECT_TIME];
        m_prevPacket.PopulateArray<TData>(0);
        m_preSpikes = new Queue<TTime>(MAX_SPIKE_QUEUE);
        CONTINUE_TRAIN = new CSpikeTrainFrame(channel, 0);
      }

      public CSpikeTrainFrame FindSpikeTrains(TData[] data)
      {
        CSpikeTrainFrame spikeTrain = null;

        int size = data.Length;
        /*
        TData ma = m_expAvg.Add(m_calcShortSE.SE(data[0]) - m_calcNoiseSE.SE(data[0]));
        TData diff = ma - m_maOld;
        m_maOld = ma;
        m_hpf = (Math.Abs(diff) > THRESH1) ? m_hpf + diff : m_hpf * DECAY;
        */
        for (int i = 0; i < size; i++)
        {
          // Check if signal is 0
          if (Math.Abs(data[i]) < ZERO_LEVEL)
          {
            if (--m_zeroCount == 0)
            {
              // If we are in the spike-train, finalize it
              if (m_state == State.Train)
              {
                if (spikeTrain == null) spikeTrain = new CSpikeTrainFrame(m_channel, m_trainStartTime);
                spikeTrain.Length = (int)(m_absTime + (TTime)i - spikeTrain.Start);
              }
              m_state = State.Zero;
            }
          }
          else
          {
            m_zeroCount = ZERO_COUNT;
          }

          switch (m_state)
          {
            // We fall into this state when data is close to 0 for ZERO_COUNT (3) subsequent samples
            case State.Zero:
              if (Math.Abs(data[i]) > ZERO_LEVEL)
              {
                // Yes, I know that we'll skip the current sample, but it is probably irrelevant anyway, for we've just recovered from 0-clamping
                m_state = (m_warmedUp) ? State.Noise : State.WarmUp;
                m_zeroCount = ZERO_COUNT;
              }
              break;

            // Here we process a noisy input signal
            case State.Noise:
              TData shortSE = m_calcShortSE.SE(data[i]);
              TData seOfShortSE = m_calcShortSE2.PrevSE;                                  // Changes slowly (~2s)
              TData meanOfNoiseSE = m_calcNoiseSE2.Mean;                                  // the previous mean

              // Remember position of several spike-like events
              if (Math.Abs(data[i]) > SPIKE_THRESH * m_calcNoiseSE.PrevSE)
              {
                if (!m_inSpike)
                {
                  m_inSpike = true;
                  if (m_preSpikes.Count >= MAX_SPIKE_QUEUE)
                  {
                    m_preSpikes.Dequeue();
                  }
                  m_preSpikes.Enqueue(m_absTime + (TTime)i);
                }
              }
              else
              {
                m_inSpike = false;
              }

              // Check start of spike-train criteria
              if (shortSE > meanOfNoiseSE + 3 * seOfShortSE)
              {
                m_state = State.Train;
                m_inSpike = false;
                TTime firstSpikeTime = 0;
                
                // Consider spike-train starts at the time of the first spike in PRE_DETECT_TIME interval before meeting of spike-train criteria
                while (m_preSpikes.Count > 0)
                {
                  firstSpikeTime = m_preSpikes.Dequeue();
                  if (firstSpikeTime + PRE_DETECT_TIME > m_absTime + (TTime)i) break;
                }
                if (firstSpikeTime == 0) firstSpikeTime = m_absTime + (TTime)i;
                if (spikeTrain == null) 
                {
                  spikeTrain = new CSpikeTrainFrame(m_channel, firstSpikeTime);         // Create SpikeTrain only when its start is detected
                  m_trainStartTime = firstSpikeTime;
                  // [DEBUG]
#if DEBUG_SPIKETRAINS
                  lock(m_dbgTrainList) m_dbgTrainList.Add(spikeTrain);
#endif
                }
                else
                {
                  spikeTrain.EOP = false;     // If a new spike-train has started right after the previous one, just continue the previous one
#if DEBUG_SPIKETRAINS
                  CSpikeTrainFrame dbgSpikeTrain = new CSpikeTrainFrame(m_channel, firstSpikeTime);
                  lock (m_dbgTrainList) m_dbgTrainList.Add(dbgSpikeTrain);
#endif
                }
                m_calcPackSE.SE(data[i]);
                break;
              }

              // Calculate long term noise 
              TData noiseSE = m_calcNoiseSE.SE(data[i]);
              m_calcNoiseSE2.SE(noiseSE);
              m_calcShortSE2.SE(shortSE);
              break;

            // We fall here when start of a train is detected, so we should look for its end
            case State.Train:
              TData packSE = m_calcPackSE.SE(data[i]);
              shortSE = m_calcShortSE.SE(data[i]);

              meanOfNoiseSE = m_calcNoiseSE2.Mean;                                  // mean SE of the stationary noise

              if (shortSE < m_calcNoiseSE2.Mean + m_calcNoiseSE2.PrevSE)
              {
                m_endTime = m_absTime + (TTime)i;
              }

              if (shortSE < m_calcNoiseSE2.Mean)
              {
                m_state = State.Noise;
                // Finalize current spike-train
                if (spikeTrain == null) spikeTrain = new CSpikeTrainFrame(m_channel, m_trainStartTime);
                spikeTrain.Length = (int)(m_endTime - spikeTrain.Start);
#if DEBUG_SPIKETRAINS
                lock (m_dbgTrainList)
                {
                  TTime dbgStartTime = m_dbgTrainList[m_dbgTrainList.Count - 1].Start;
                  CSpikeTrainFrame dbgSpikeTrain = new CSpikeTrainFrame(m_channel, dbgStartTime);
                  m_dbgTrainList.Add(dbgSpikeTrain);
                }
#endif
              }
              break;

            // This state is used only once in the begining to "warm up" statistic calculators 
            case State.WarmUp:
              for (int t = 0; t < data.Length; t++)
              {
                if (Math.Abs(data[i]) > ZERO_LEVEL)
                {
                  --m_warmUpCount;
                  TData shortSEwu = Math.Sqrt(m_calcShortSE.WarmUp(data[t]));
                  TData noiseSEwu = Math.Sqrt(m_calcNoiseSE.WarmUp(data[t]));
                  TData packSEwu = Math.Sqrt(m_calcNoiseSE.WarmUp(data[t] * 2));      // Consider signal at least 2 times higher than noise
                  m_calcNoiseSE2.WarmUp(noiseSEwu);
                }
                if (m_warmUpCount == 0)
                {
                  m_warmedUp = true;
                  m_state = State.Noise;
                  break;
                }
              }
              break;
          }

          /*
          TData delta = m_calcShortSE.SE(data[i]) - m_calcNoiseSE.SE(data[i]);
          ma = m_expAvg.Add(delta);
          diff = ma - m_maOld;
          m_maOld = ma;
          m_hpf = (Math.Abs(diff) > THRESH1) ? m_hpf + diff : m_hpf * DECAY;

          if ((m_hpf > THRESH2) && !m_packFound)
          {
            m_packFound = true;
            spikeTrain = new CSpikeTrainFrame(m_channel, m_absTime + (TTime)i);         // Create SpikeTrain only when its start is detected
          }

          if (m_packFound && (diff < 0))
          {

          }
           */
        }

        if ((m_state == State.Train) && (spikeTrain == null))
        {
          spikeTrain = CONTINUE_TRAIN;
        }

        m_absTime += (TTime)size;
        m_prevPacket = data;
        return spikeTrain;
      }

    }

    private readonly List<CSpikeTrainFrame> NO_SPIKETRAINS = new List<CSpikeTrainFrame>();
    private Dictionary<int, CSpikeTrainDetector> m_spikeTraintDet;
    private CFiltering m_filteredStream;
    private List<int> m_activeChannelList;
    private List<CSpikeTrainFrame> m_packSpikeTrainList;
    private List<CSpikeTrainFrame> m_prevSpikeTrains;
    private TFltDataPacket m_prevPacket;
    private List<TFltDataPacket> m_packDataList;
    private Queue<TFltDataPacket> m_filteredQueue;
    private AutoResetEvent m_notEmpty;
    private TTime m_firstSpikeTime = 0;
    private TTime m_lastSpikeTime = 0;
    private Int32 m_packDataStart = 0;
    private TTime m_timestamp = 0;
    private Int32 m_prevPacketLength = 0;
    private bool m_inPack = false;
    private volatile bool m_kill = false;

    // [DEBUG]
    private TTime m_debugTimestamp;
    private int m_entryCount = 0;
    private int m_entryCount2 = 0;
    private System.Timers.Timer m_dummyTimer;
    Int32 m_dummyPackTime = -1;
    Random m_rnd;
    private bool m_inPackDbg = false;

#if DEBUG_SPIKETRAINS
    public Dictionary<int, List<CSpikeTrainFrame>> GetSpikeTrainDebug()
    {
      Dictionary<int, List<CSpikeTrainFrame>> spikeTrainDbg = new Dictionary<int,List<CSpikeTrainFrame>>();
      foreach (Int16 channel in m_activeChannelList)
      {
        spikeTrainDbg[channel] = m_spikeTraintDet[channel].GetSpikeTrainListDbg();
        m_spikeTraintDet[channel].ClearSpikeTrainListDbg();
      }
      return spikeTrainDbg;
    }
#endif

    public CPackDetector(CFiltering filteredStream, List<int> channelList = null)
    {
      if (filteredStream == null) throw new ArgumentNullException("filteredStream");
      if (MAX_LEADING_SPIKETRAIN + Param.PRE_SPIKE > Param.DEF_PACKET_LENGTH) throw new Exception("Wrong constans in the class Param.");

      m_filteredStream = filteredStream;
      if (channelList == null)
      {
        channelList = Enumerable.Range(0, MEA.MAX_CHANNELS - 1).ToList();
      }
      m_activeChannelList = channelList;
      m_spikeTraintDet = new Dictionary<int, CSpikeTrainDetector>(channelList.Count);
      foreach (Int16 channel in m_activeChannelList) m_spikeTraintDet[channel] = new CSpikeTrainDetector(channel);

      m_rnd = new Random(DateTime.Now.Millisecond);
      m_packSpikeTrainList = new List<CSpikeTrainFrame>();
      m_prevSpikeTrains = NO_SPIKETRAINS;
      m_packDataList = new List<TFltDataPacket>();
      m_prevPacket = new TFltDataPacket();
      m_prevPacket[0] = new TData[0];                 // just to avoid NullReferenceException at the first packet
      m_filteredQueue = new Queue<TFltDataPacket>();
      m_notEmpty = new AutoResetEvent(false);

      m_filteredStream.AddDataConsumer(ReceiveData);


      // [DEBUG]
      
      m_dummyTimer = new System.Timers.Timer(1000);
      m_dummyTimer.Elapsed += DummyTimer;
      m_dummyTimer.Start();
      //m_rnd = new Random(123);
      m_rnd = new Random(DateTime.Now.Millisecond);
      
    }

    private void DummyTimer(object o1, EventArgs e1)
    {
      lock (m_dummyTimer)
      {
        m_dummyPackTime = m_rnd.Next(5000);
      }
    }

    // Callback to readout data from the Filtered Stream
    private void ReceiveData(TFltDataPacket packet)
    {
      m_debugTimestamp = m_filteredStream.TimeStamp;
      lock (m_filteredQueue) m_filteredQueue.Enqueue(packet);
      m_notEmpty.Set();
    }

    // Reads filtered packets from the queue and passes then to DetectPacks() until a pack is found
    // [TODO] It is probably usefull to make timeout here and return null if no pack is found in given time
    // public CPack WaitPack(int timeout = 0)
    public CPack WaitPack()
    {
      CPack pack = null;
      TFltDataPacket dataPacket = null;
      do
      {
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

        m_timestamp += (TTime)m_prevPacketLength;
        //[DEBUG]
        //if (m_debugTimestamp != m_timestamp) throw new Exception("Wrong Timestamp");
        m_prevPacketLength = dataPacket[dataPacket.Keys.ElementAt(0)].Length;

        pack = DetectPacks(dataPacket);
      } while (pack == null);
      return pack;
    }

    public void Dismiss()
    {
      m_kill = true;
    }

    // Алгоритм поиска пачек.
    // Разделяется на 2 случая: мы вне пачки или внутри пачки.
    // В любом случае ищем начала спайк-трэйнов на активных электродах в текущем пакете.
    // CSpikeTrainDetector возвращает null либо CSpikeTrainFrame, который содержит только время начала и длину спайк-трэйна.
    // Если длина равна нулю - спайк-трэйн ещё не закончился и будет продолжен в следующем пакете.
    // Если пришло начало спайк-трейна, то в следующих пакетах, должны приходить пустые CSpikeTrainFrame-ы пока не буден найден конец.
    // В одном пакете на одном электроде не может быть более одного спайк-трэйна.
    // Если мы вне пачки, то если найден хотя бы один спайк трэйн, запомним его, чтобы начать с него пачку, если что.
    // Но если в следующем пакете не будет найдено продолжения на длугих электродах, то пачку дискардим.
    // Когда на всех электродах закончились спайк-трэйны и не начались новые в течение времени Param.POST_SPIKE (200 мс), то пачка закончилась.
    //
    // DetectPacks может возвращать S-Pack (Start), null или T-Pack (Tail).
    // null означает середину или отсутствие пачки
    
    private CPack DetectPacks(TFltDataPacket packet)
    {
      #region DEBUG
      m_entryCount++;
      // [DEBUG]
      

      CPack debugPack = null;
      const int PACK_POINT_COUNT = 2000;
      if (m_dummyPackTime >= 0)
      {
        TData[] dummyNoise = new TData[MEA.MAX_CHANNELS];
        dummyNoise.PopulateArray(100);

        if (m_inPackDbg)
        {
          TFltDataPacket packData = new TFltDataPacket(m_activeChannelList.Count);

          // Fill packData with active channels keys
          foreach (int channel in m_activeChannelList) packData[channel] = new TData[PACK_POINT_COUNT];

          packData.Keys.AsParallel().ForAll(channel =>
          {
            // Process the first packet
            int length = PACK_POINT_COUNT;
            int j = 0;
            for (int i = 0; i < length; ++i)
            {
              packData[channel][j] = packet[channel][j];
              j++;
            }
          });

          debugPack = new CPack(m_timestamp + (TTime)m_dummyPackTime, PACK_POINT_COUNT, packData, dummyNoise);
        }
        else
        {
          debugPack = new CPack(m_timestamp + (TTime)m_dummyPackTime, 3000, null, dummyNoise);
        }
        m_dummyPackTime = -1;
        m_inPackDbg = !m_inPackDbg;
     }

      return debugPack;
      
      #endregion // DEBUG

      int currPacketLength = packet[packet.Keys.ElementAt(0)].Length;

      Dictionary<int, CSpikeTrainFrame> spikeTrains = new Dictionary<int, CSpikeTrainFrame>();

      // Fill keys to enable parallel foreach on the next step
      foreach (int channel in m_activeChannelList) spikeTrains[channel] = null;
      // Find spike-trains on all active electrodes

      m_activeChannelList.AsParallel().ForAll(channel => spikeTrains[channel] = m_spikeTraintDet[channel].FindSpikeTrains(packet[channel]));

      // Select found spike-trains
      var spikeTrainList = spikeTrains.Values.Where(el => el != null).ToList();

      // Check if spikeTrains contains at least one spike-train
      if (spikeTrainList.Count > 0)
      #region Process Spike-trains
      {
        // Check if all spike-trains finish here
        bool packContinues = spikeTrainList.Any(el => !el.EOP);

        if (m_inPack)
        {
          if (m_lastSpikeTime == 0)                          // There were some unfinished spike-trains in the previous packet
          {
            // Throws an exception if some spike-trains have been lost. Should never happen.
            if (m_prevSpikeTrains.Where(el => !el.EOP).Any(el_prev => !spikeTrainList.Any(el_new => el_new.Channel == el_prev.Channel)))
            {
              throw new Exception("Something is wrong in Spike-train Detector. Spike-train has been lost.");
            }
          }
          m_prevSpikeTrains = spikeTrainList;
          m_packDataList.Add(packet);

          // If the pack is going to be finished, remember the last spike time
          m_lastSpikeTime = packContinues ? 0 : spikeTrainList.Max(el => el.Start + (TTime)el.Length);
        }
        else                                                // Outside a pack
        {
          // Here we should decide whether to start a new pack or not
          // If there are more than one spike-train, just start a new pack immediately
          if (spikeTrainList.Count > 1)
          {
            Int32 prevPacketLength = m_prevPacket[m_prevPacket.Keys.ElementAt(0)].Length;
            m_firstSpikeTime = spikeTrainList.Min(el => el.Start);

            // Account for a single spike-train in the previous packet if it was there
            if (m_prevSpikeTrains.Count == 1)
            {
              m_firstSpikeTime = m_prevSpikeTrains[0].Start;
              m_packDataList.Add(m_prevPacket);
            }
            m_packDataList.Add(packet);

            // Calculate pack start relatively to the first data packet start
            TTime firstPacketTime = m_timestamp - ((m_firstSpikeTime >= m_timestamp) ? 0 : (TTime)prevPacketLength);
            if (m_firstSpikeTime < firstPacketTime) throw new Exception("Pack start has been detected before data start"); // Never should happen
            m_packDataStart = (Int32)(m_firstSpikeTime - firstPacketTime);

            // Create a new S-Pack
            CPack pack = new CPack(m_firstSpikeTime, 0, null); // length == 0 means S-Pack

            m_inPack = true;
            m_prevSpikeTrains = spikeTrainList;
            return pack;
          }
          // Else, we have only one spike-train. So, check if it has been started not too early and remember it for the better times
          m_prevSpikeTrains = (spikeTrainList[0].Start + MAX_LEADING_SPIKETRAIN > m_timestamp + (TTime)currPacketLength) ? spikeTrainList : NO_SPIKETRAINS;
          // Remember the current packet in case we find start of a pack in the next packet
          m_prevPacket = packet;
        }
      }
      #endregion
      else                                                  // spikeTrainList == 0, we haven't found any spike-train in the current packet
      #region Process abscence of Spike-trains
      {
        m_prevPacket = packet;
        m_prevSpikeTrains = NO_SPIKETRAINS;

        if (m_inPack)
        {
          // If there are unfinished spike-trains, throw the exception
          if (m_lastSpikeTime == 0) throw new Exception("Something is wrong in the Spike-train Detector. Spike-train has been lost.");

          // If we are in a pack and haven't got any spikes for POST_SPIKE time, terminate this pack
          if (m_timestamp + (TTime)currPacketLength > m_lastSpikeTime + Param.POST_SPIKE)
          {                                                 
            // [TODO] Join all the necessary data from m_packDataList and Create EOP here
            Int32 packLength = (Int32)(m_lastSpikeTime - m_firstSpikeTime);
            Int32 dataLength = packLength + Param.PRE_SPIKE + Param.POST_SPIKE;
            TFltDataPacket packData = new TFltDataPacket(m_activeChannelList.Count);

            // Fill packData with active channels keys
            foreach (int channel in m_activeChannelList) packData[channel] = new TData[dataLength];
            
            packData.Keys.AsParallel().ForAll(channel => {
              // Process the first packet
              int length = m_packDataList[0][m_packDataList[0].Keys.ElementAt(0)].Length;
              int j = 0;
              for (int i = m_packDataStart; i < length; ++i)
              {
                packData[channel][j++] = m_packDataList[0][channel][i];
              }
              m_packDataList.RemoveAt(0);
              
              // Process all intermediate packets
              m_packDataList.ForEach(dataPacket =>
              {
                length = dataPacket[dataPacket.Keys.ElementAt(0)].Length;
                for (int i = 0; i < length; ++i)
                {
                  packData[channel][j++] = dataPacket[channel][i];
                }
              });

              // Process the last packet
              length = dataLength - j;
              for (int i = 0; i < length; ++i)
              {
                packData[channel][j++] = packet[channel][i];
              }
            });
            
            // Create a new T-Pack (EOP)
            CPack pack = new CPack(m_firstSpikeTime, packLength, packData);

            m_lastSpikeTime = 0;
            m_packDataList.Clear();
            m_inPack = false;
            return pack;
          }
          // Else, just wait for the next packet
          m_packDataList.Add(packet);
        }
        // Else, we are not in pack, so just return null
      }
      #endregion
      return null;
    }

    // [OBSOLETE] Moved to class CSpikeTrainFrameDetector
    /*
    // [TODO] Get rid of the global variables to enable parallelization
    private List<Int32> DetectSpikeTrains(TData[] data)
    {
      List<Int32> trainList = null;

      int size = data.Length;
      //TData[] hpf = new TData[size];
      //TData[] ma = new TData[size];

      //ma[0] = m_expAvg.Add(m_se.SE(data[0]) - m_seLT.SE(data[0]));
      //TData diff = ma[0] - m_maOld;
      //hpf[0] = (Math.Abs(diff) > THRESH1) ? m_hpfOld + diff : m_hpfOld * DECAY;

      TData ma = m_expAvg.Add(m_se.SE(data[0]) - m_seLT.SE(data[0]));
      TData diff = ma - m_maOld;
      m_maOld = ma;
      m_hpf = (Math.Abs(diff) > THRESH1) ? m_hpf + diff : m_hpf * DECAY;

      for (int i = 1; i < data.Length; i++)
      {
        TData delta = m_se.SE(data[i]) - m_seLT.SE(data[i]);
        //ma[i] = m_expAvg.Add(delta);
        //diff = ma[i] - ma[i - 1];
        //hpf[i] = (Math.Abs(diff) > THRESH1) ? hpf[i - 1] + diff : hpf[i - 1] * DECAY;
        ma = m_expAvg.Add(delta);
        diff = ma - m_maOld;
        m_maOld = ma;
        m_hpf = (Math.Abs(diff) > THRESH1) ? m_hpf + diff : m_hpf * DECAY;

        if ((m_hpf > THRESH2) && !m_packFound)
        {
          m_packFound = true;
          if (trainList == null) trainList = new List<Int32>();         // Create List only when pack is detected
          trainList.Add(i);
        }

        if (m_packFound && (diff < 0))
        {
        }
      }
      //m_hpf = hpf[size - 1];
      //m_maOld = ma[size - 1];
      

      return trainList;
    }
    */ 

  }
}
