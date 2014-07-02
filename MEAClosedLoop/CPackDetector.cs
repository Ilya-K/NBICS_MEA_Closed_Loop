//#define DEBUG_SPIKETRAINS
//#define RANDOM_PACKS

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
    private const int MIN_TRAINS_TO_START_PACK = 3;
    private const int MIN_TRAINS_TO_CONTINUE_PACK = 3;
    private const int MAX_FRONT_WIDTH = 15 * Param.MS;      // Насколько один спайк-трэйн может выдаваться вперёд пачки (ширина фронта пачки).
                                                            // В текущем алгоритме не может быть больше 100 - Param.PRE_SPIKE = 92 мс (2300 отсчетов)
    private const int MAX_PACK_LENGTH = 5000 * Param.MS;    // Максимальная длина пачки. При её достижении пачка будет закончена, и начнётся следующая при необходимости.


    // Алгоритм выделения спайк-трэйнов
    // Считаем дисперсию сигнала в экспоненциальном окне с временем релаксации 250 точек (10 мс, окно ~20 мс).
    // Считаем шум стационарным, т.е. дисперсия шума должна меняться слабо и медленно. Поэтому любое быстрое увеличение дисперсии сигнала будем считать началом пачки.
    // Для того чтобы обнаружить изменение дисперсии шума будем считать дисперсию дисперсии шума D[D[data]] в том же экспоненциальном окне.
    // Если дисперсия сигнала D[data] превысит среднюю дисперсию шума E[D[data]] на 

    private class CSpikeTrainDetector
    {
      private const TData ZERO_LEVEL = 0.001;               // Actually there is a strict zero after SALPA
      private const int ZERO_COUNT = 2;                     // How many sequental zero points have got to consider signal = 0;
      private const TData BLIND_THRESH = 7;                 // Below this noise level (SE) we consider the channel to be blind
      private const TData SPIKE_THRESH = 4.0;               // *sigma; Leading spikes threshold 
      private const TData SE_THRESH = 5.5;                  // *sigma; Start of spike-train
      private const int PRE_DETECT_TIME = 12 * Param.MS;    // Интервал перед выполнением критерия начала спайк-трэйна, в которм будем искать первый спайк
      private const int MIN_SPIKES_COUNT = 3;               // How many spikes in PRE_DETECT_TIME interval should be encountered to start a spike-train
      private const int MAX_SPIKE_QUEUE = 5;                // Length of queue for pre-train spikes
      private const int WARMUP_COUNT = 500;                 // How many points to use to warm up SE calculators
      
      /*
      private const TData THRESH1 = 0.14;
      private const TData THRESH2 = 1.0;
      private const TData DECAY = 0.98;
       */

      private CSpikeTrainFrame m_continueTrain;             // The return value inside a spike-train

      private enum State
      {
        Zero = 0,
        Blind,
        Noise,
        Train,
        WarmUp
      }
      private State m_state;

      private TTime m_absTime = 0;
      private Int16 m_channel;
      
      // Short term mean and variance calculator
      private CCalcExpWndSE m_calcShortSE = new CCalcExpWndSE((10 * Param.MS) / 2);       // 2*tau = 10ms (250 samples)

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
      private TTime m_lastSpike = 0;
      private float percentActive = 0;
      private float percentBlind = 0;
      public TData MeanNoise { get { return m_calcNoiseSE2.Mean; } }
      public TData SignalNoiseRatio { get { return m_calcPackSE.PrevSE / m_calcNoiseSE2.Mean; } }

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
          if (!lastOne.EOT)
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
        m_warmUpCount = WARMUP_COUNT;
        m_prevPacket = new TData[PRE_DETECT_TIME];
        m_prevPacket.PopulateArray<TData>(0);
        m_preSpikes = new Queue<TTime>(MAX_SPIKE_QUEUE);
        m_continueTrain = new CSpikeTrainFrame(m_channel, 0);
      }

      public CSpikeTrainFrame FindSpikeTrains(TData[] data)
      {
        CSpikeTrainFrame spikeTrain = (m_state == State.Train) ? m_continueTrain : null;
        bool skipThisPacket = false;

        int size = data.Length;
        for (int i = 0; i < size; i++)
        {
          // Check if signal is 0
          // If signal is 0 for ZERO_COUNT (def = 2) samples, consider electrode is blind or blanked
          if (Math.Abs(data[i]) < ZERO_LEVEL)
          {
            if (--m_zeroCount == 0)                                       // Zero count goes < 0 here, but it is not critical
            {
              // If we are in the spike-train, finalize it
              if (m_state == State.Train)
              {
                spikeTrain.Length = (int)(m_absTime + (TTime)i - spikeTrain.Start);
                m_continueTrain = null;
#if DEBUG_SPIKETRAINS
                lock (m_dbgTrainList)
                {
                  // We need to get the start time from the previous debug spike-train, becouse the real spike-trains might be joined.
                  TTime dbgStartTime = m_dbgTrainList[m_dbgTrainList.Count - 1].Start;
                  CSpikeTrainFrame dbgSpikeTrain = new CSpikeTrainFrame(m_channel, dbgStartTime);
                  dbgSpikeTrain.Length = (int)(m_absTime + (TTime)i - dbgStartTime);
                  m_dbgTrainList.Add(dbgSpikeTrain);
                }
#endif
              }
              if (m_inSpike)
              {
                m_lastSpike = m_absTime + (TTime)i + 1;
                m_inSpike = false;
              }
              m_state = State.Zero;
            }
          }
          else
          {
            m_zeroCount = ZERO_COUNT;
          }

          TData shortSE, seOfShortSE, meanOfNoiseSE, noiseSE, packSE;

          switch (m_state)
          {
            // We fall into this state when data == 0 for ZERO_COUNT (def = 2) subsequent samples
            case State.Zero:
              if (Math.Abs(data[i]) > ZERO_LEVEL)
              {
                // Yes, I know that we'll skip the current sample, but it is probably irrelevant anyway, for we've just recovered from 0-clamping
                m_state = (m_warmedUp) ? State.Noise : State.WarmUp;
                m_zeroCount = ZERO_COUNT;
              }
              break;

            // We fall into this state when data SE is less than BLIND_THRESH (def = 10) close to 0
            case State.Blind:
              shortSE = m_calcShortSE.SE(data[i]);
              m_calcShortSE2.SE(shortSE);
              if (m_calcShortSE2.Mean > BLIND_THRESH + m_calcShortSE2.PrevSE)     // Make some hysteresis for leaving blind state at higher level
              {
                m_calcShortSE.WarmedUp(false);
                m_calcNoiseSE.WarmedUp(false);
                m_calcPackSE.WarmedUp(false);
                m_calcNoiseSE2.WarmedUp(false);
                m_calcShortSE2.WarmedUp(false);
                m_warmedUp = false;
                m_warmUpCount = WARMUP_COUNT;
                m_state = State.WarmUp;
              }
              break;

            // Here we process a noisy input signal
            case State.Noise:
              shortSE = m_calcShortSE.SE(data[i]);
              seOfShortSE = m_calcShortSE2.PrevSE;                                  // Changes slowly (~2s)
              meanOfNoiseSE = m_calcNoiseSE2.Mean;                                  // the previous mean

              if (m_calcShortSE2.Mean < BLIND_THRESH)
              {
                m_state = State.Blind;
                if (m_inSpike)
                {
                  m_lastSpike = m_absTime + (TTime)i + 1;
                  m_inSpike = false;
                }
                break;
              }

              // Remember position of several spike-like events
              if (Math.Abs(data[i]) > SPIKE_THRESH * m_calcNoiseSE.PrevSE)
              {
                if (!m_inSpike)
                {
                  m_inSpike = true;

                  // Get rid of old spikes
                  if (m_preSpikes.Count > 0)
                  {
                    TTime preSpikeTime = m_preSpikes.Peek();
                    while (preSpikeTime + PRE_DETECT_TIME < m_absTime + (TTime)i)
                    {
                      m_preSpikes.Dequeue();
                      if (m_preSpikes.Count == 0) break;
                      preSpikeTime = m_preSpikes.Peek();
                    }
                  }

                  if (m_preSpikes.Count >= MAX_SPIKE_QUEUE) m_preSpikes.Dequeue();
                  m_preSpikes.Enqueue(m_absTime + (TTime)i);
                }
              }
              else
              {
                m_inSpike = false;
                m_lastSpike = m_absTime + (TTime)i + 1;
              }

              // Check start of spike-train criteria
              if ((shortSE > meanOfNoiseSE + SE_THRESH * seOfShortSE) && (m_preSpikes.Count >= MIN_SPIKES_COUNT))
              {
                m_state = State.Train;
                m_inSpike = false;
                TTime firstSpikeTime = 0;

                // Consider spike-train starts at the time of the first spike in PRE_DETECT_TIME interval before meeting of spike-train criteria
                while (m_preSpikes.Count > 0)
                {
                  TTime preSpikeTime = m_preSpikes.Dequeue();
                  if (preSpikeTime + PRE_DETECT_TIME > m_absTime + (TTime)i)
                  {
                    firstSpikeTime = preSpikeTime;
                    m_preSpikes.Clear();
                    break;
                  }
                }
                if (firstSpikeTime == 0) firstSpikeTime = m_absTime + (TTime)i;
                if (spikeTrain == null)
                {
                  spikeTrain = new CSpikeTrainFrame(m_channel, firstSpikeTime);         // Create SpikeTrain only when its start is detected
                  m_trainStartTime = firstSpikeTime;
                }
                else
                {
                  spikeTrain.EOT = false;     // If a new spike-train has started right after the previous one, just continue the previous one
                }
#if DEBUG_SPIKETRAINS
                lock (m_dbgTrainList) m_dbgTrainList.Add(new CSpikeTrainFrame(m_channel, firstSpikeTime));
#endif
                m_calcPackSE.SE(data[i]);
                break;
              }

              // Calculate long term noise 
              noiseSE = m_calcNoiseSE.SE(data[i]);
              m_calcNoiseSE2.SE(noiseSE);
              // Calculate SE of short term noise. Calculate it only here, not in State.Train
              m_calcShortSE2.SE(shortSE);
              break;

            // We fall here when start of a train is detected, so we should look for its end
            case State.Train:
              packSE = m_calcPackSE.SE(data[i]);
              shortSE = m_calcShortSE.SE(data[i]);

              meanOfNoiseSE = m_calcNoiseSE2.Mean;                                  // mean SE of the stationary noise

              // Remember position of the last spike
              if (Math.Abs(data[i]) > SPIKE_THRESH * m_calcNoiseSE.PrevSE)
              {
                if (!m_inSpike) m_inSpike = true;
              }
              else
              {
                m_inSpike = false;
                m_lastSpike = m_absTime + (TTime)i + 1;
              }

              if (shortSE < meanOfNoiseSE)
              {
                m_state = State.Noise;
                // Finalize current spike-train
                spikeTrain.Length = (int)(m_lastSpike - spikeTrain.Start);
                m_continueTrain = null;
#if DEBUG_SPIKETRAINS
                lock (m_dbgTrainList)
                {
                  // We need to get the start time from the previous debug spike-train, becouse the real spike-trains might be joined.
                  TTime dbgStartTime = m_dbgTrainList[m_dbgTrainList.Count - 1].Start;
                  CSpikeTrainFrame dbgSpikeTrain = new CSpikeTrainFrame(m_channel, dbgStartTime);
                  dbgSpikeTrain.Length = (int)(m_lastSpike - dbgSpikeTrain.Start);
                  m_dbgTrainList.Add(dbgSpikeTrain);
                }
#endif
              }
              break;

            // This state is used only once in the begining to "warm up" statistic calculators 
            case State.WarmUp:
              // Start warming up from the current position i
              for (int t = i; t < data.Length; t++)
              {
                if (Math.Abs(data[t]) > ZERO_LEVEL)
                {
                  --m_warmUpCount;
                  TData shortSEwu = Math.Sqrt(m_calcShortSE.WarmUp(data[t]));
                  TData noiseSEwu = Math.Sqrt(m_calcNoiseSE.WarmUp(data[t]));
                  TData packSEwu = Math.Sqrt(m_calcPackSE.WarmUp(data[t] * 2));      // Consider signal to be at least 2 times higher than noise
                  m_calcShortSE2.SE(shortSEwu);
                  m_calcNoiseSE2.WarmUp(noiseSEwu);
                }
                if (m_warmUpCount == 0)
                {
                  m_warmUpCount = WARMUP_COUNT;
                  m_calcShortSE.WarmedUp();
                  m_calcNoiseSE.WarmedUp();
                  m_calcPackSE.WarmedUp();
                  m_calcNoiseSE2.WarmedUp();
                  m_calcShortSE2.WarmedUp();

                  m_warmedUp = true;
                  m_state = (m_calcShortSE2.Mean < BLIND_THRESH) ? State.Blind : State.Noise;
                  break;
                }
              }
              // If we have not managed to warm up calculators until the end of packet, just exit
              if (!m_warmedUp) skipThisPacket = true;
              break;
          }

          if (skipThisPacket) break;
        }

        if ((spikeTrain != null) && !spikeTrain.EOT) m_continueTrain = new CSpikeTrainFrame(m_channel, spikeTrain.Start);

        m_absTime += (TTime)size;
        m_prevPacket = data;
        return spikeTrain;
      }

    }

    //private readonly List<CSpikeTrainFrame> NO_SPIKETRAINS = new List<CSpikeTrainFrame>();
    private Dictionary<int, CSpikeTrainDetector> m_spikeTraintDet;
    private Dictionary<int,TData> m_noiseInPack;
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
    private Int32 m_packDataStart = 0;                                  // Relative start of data (first spike - PreSpike) in the first packet of a pack
    private TTime m_timestamp = 0;
    private Int32 m_prevPacketLength = 0;
    private bool m_inPack = false;
    private bool m_sendNewPack = false;
    private volatile bool m_kill = false;

    // [DEBUG]
#if RANDOM_PACKS
    private TTime m_debugTimestamp;
    private System.Timers.Timer m_dummyTimer;
    Int32 m_dummyPackTime = -1;
    Random m_rnd;
    private bool m_inPackDbg = false;
#endif

#if DEBUG_SPIKETRAINS
    public Dictionary<int, List<CSpikeTrainFrame>> GetSpikeTrainDbg()
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
      if (MAX_FRONT_WIDTH + Param.PRE_SPIKE > Param.DEF_PACKET_LENGTH) throw new Exception("Wrong constans in the class Param.");

      m_filteredStream = filteredStream;
      if (channelList == null)
      {
        channelList = Enumerable.Range(0, MEA.MAX_CHANNELS - 1).ToList();
      }
      m_activeChannelList = channelList;
      m_spikeTraintDet = new Dictionary<int, CSpikeTrainDetector>(channelList.Count);
      foreach (Int16 channel in m_activeChannelList) m_spikeTraintDet[channel] = new CSpikeTrainDetector(channel);

      m_packSpikeTrainList = new List<CSpikeTrainFrame>();
      m_prevSpikeTrains = new List<CSpikeTrainFrame>();   // NO_SPIKETRAINS;
      m_packDataList = new List<TFltDataPacket>();
      m_prevPacket = new TFltDataPacket();
      m_prevPacket[0] = new TData[0];                     // just to avoid NullReferenceException at the first packet
      m_filteredQueue = new Queue<TFltDataPacket>();
      m_notEmpty = new AutoResetEvent(false);

      m_filteredStream.AddDataConsumer(ReceiveData);


      // [DEBUG]
#if RANDOM_PACKS
      m_dummyTimer = new System.Timers.Timer(1000);
      m_dummyTimer.Elapsed += DummyTimer;
      m_dummyTimer.Start();
      //m_rnd = new Random(123);
      m_rnd = new Random(DateTime.Now.Millisecond);
#endif
    }

      // [DEBUG]
#if RANDOM_PACKS
    private void DummyTimer(object o1, EventArgs e1)
    {
      lock (m_dummyTimer)
      {
        m_dummyPackTime = m_rnd.Next(5000);
      }
    }
#endif

    // Callback to readout data from the Filtered Stream
    private void ReceiveData(TFltDataPacket packet)
    {
#if RANDOM_PACKS
      m_debugTimestamp = m_filteredStream.TimeStamp;
#endif
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
    // Если пришло начало спайк-трейна, то в следующих пакетах, должны приходить !null CSpikeTrainFrame-ы пока не буден найден конец.
    // В одном пакете на одном электроде не может быть более одного спайк-трэйна.
    // Если мы вне пачки, то ищем спайк-трэйны у которых ширина фронта не превышает заданную, и если их больше порога, начинаем пачку.
    // Когда на всех электродах закончились спайк-трэйны и не начались новые в течение времени Param.POST_SPIKE (200 мс), то пачка закончилась.
    //
    // DetectPacks может возвращать S-Pack (Start), null или T-Pack (Tail).
    // null означает середину или отсутствие пачки
    
    private CPack DetectPacks(TFltDataPacket packet)
    {
      // [DEBUG]
#if RANDOM_PACKS
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
#endif      

      int currPacketLength = packet[packet.Keys.ElementAt(0)].Length;

      Dictionary<int, CSpikeTrainFrame> spikeTrains = new Dictionary<int, CSpikeTrainFrame>();
      Dictionary<int, TData> noiseLevelDic = new Dictionary<int, TData>();

      // Fill keys to enable parallel foreach on the next step
      foreach (int channel in m_activeChannelList)
      {
        noiseLevelDic[channel] = 0;
        spikeTrains[channel] = null;
      }
      // Find spike-trains on all active electrodes
      m_activeChannelList.AsParallel().ForAll(channel =>
      {
        noiseLevelDic[channel] = m_spikeTraintDet[channel].MeanNoise;
        spikeTrains[channel] = m_spikeTraintDet[channel].FindSpikeTrains(packet[channel]);
      });

      // Select found spike-trains
      var spikeTrainList = spikeTrains.Values.Where(el => el != null).ToList();

      // Throw an exception if some spike-trains have been lost. Should never happen.
      if (m_prevSpikeTrains.Where(el => !el.EOT).Any(el_prev => !spikeTrainList.Any(el_new => el_new.Channel == el_prev.Channel)))
      {
        throw new Exception("Something is wrong in Spike-train Detector. Spike-train has been lost.");
      }
      m_prevSpikeTrains = spikeTrainList;

      var finishedTrains = spikeTrainList.Where(el => el.EOT).ToList();
      int numActiveTrainsLeft = spikeTrainList.Count - finishedTrains.Count;

      if (m_inPack)
      #region Process a packet inside a pack
      {
        TTime expectedPackEnd = m_lastSpikeTime + Param.POST_SPIKE;
        bool finishCurrentPack = false;
        
        // Check if we have already decided to stop this pack, but more than threshold spike-trains have been found 
        if ((m_lastSpikeTime > 0) && (spikeTrainList.Count >= MIN_TRAINS_TO_CONTINUE_PACK))
        {
          // Check if the end of pack was expected in this packet
          if (expectedPackEnd < m_timestamp + (TTime)currPacketLength)
          {
            int numTrainsBeforeThreshold = spikeTrainList.Count(el => el.Start < expectedPackEnd);

            // If there are some spike-trains in the current packet before expectedPackEnd enough to continue a pack
            if (numTrainsBeforeThreshold >= MIN_TRAINS_TO_CONTINUE_PACK)
            {
              m_lastSpikeTime = 0;          // Current pack will be continued
            }
            else
            {
              finishCurrentPack = true;     // Finish current pack despite of active trains at the ent of packet, cos they've come too late
            }
          }
        }

        // Check if pack length limit exceeded
        if (m_timestamp + (TTime)currPacketLength > m_firstSpikeTime + MAX_PACK_LENGTH)
        {
          if (finishedTrains.Count > 0)
          {
            // Find the last spike in the spike-traines finished in current packet
            m_lastSpikeTime = finishedTrains.Max(el => el.Start + (TTime)el.Length);
          }
          else
          {
            // Just stop this pack as soon as possible, i.e. at the start of this packet
            m_lastSpikeTime = m_timestamp; // m_firstSpikeTime + MAX_PACK_LENGTH // Or at MAX_PACK_LENGTH
          }
          finishCurrentPack = true;
        }

        if ((numActiveTrainsLeft < MIN_TRAINS_TO_CONTINUE_PACK) || finishCurrentPack)
        #region Finish current pack
        {
          if (m_lastSpikeTime == 0)
          {
            // Find the last spike in the spike-traines finished in current packet
            m_lastSpikeTime = finishedTrains.Max(el => el.Start + (TTime)el.Length);
            expectedPackEnd = m_lastSpikeTime + Param.POST_SPIKE;
          }

          // If we are in a pack and haven't got any spikes for POST_SPIKE time, terminate this pack
          if (expectedPackEnd < m_timestamp + (TTime)currPacketLength)
          {
            // Join all the necessary data from m_packDataList and Create EOP here
            Int32 packLength = (Int32)(m_lastSpikeTime - m_firstSpikeTime);
            Int32 dataLength = packLength + Param.PRE_SPIKE + Param.POST_SPIKE;
            TFltDataPacket packData = new TFltDataPacket(m_activeChannelList.Count);

            // Fill packData with active channels keys
            foreach (int channel in m_activeChannelList) packData[channel] = new TData[dataLength];

            // Process the first packet
            TFltDataPacket firstDataPacket = m_packDataList[0];
            m_packDataList.RemoveAt(0);
            int firstPacketLength = firstDataPacket[firstDataPacket.Keys.ElementAt(0)].Length;
            packData.Keys.AsParallel().ForAll(channel =>
            {
              int length = firstPacketLength;
              int j = 0;
              for (int i = m_packDataStart; i < length; ++i)
              {
                packData[channel][j++] = firstDataPacket[channel][i];
              }
            });
            int lastPosition = firstPacketLength - m_packDataStart;

            // Process all intermediate packets
            m_packDataList.ForEach(dataPacket =>
            {
              int packetLength = dataPacket[dataPacket.Keys.ElementAt(0)].Length;
              packData.Keys.AsParallel().ForAll(channel =>
              {
                int length = packetLength;
                int j = lastPosition;
                for (int i = 0; i < length; ++i)
                {
                  packData[channel][j++] = dataPacket[channel][i];
                }
              });
              lastPosition += packetLength;
            });

            // Process the last packet
            packData.Keys.AsParallel().ForAll(channel =>
            {
              int length = dataLength - lastPosition;
              int j = lastPosition;
              for (int i = 0; i < length; ++i)
              {
                //попытка пофиксить баг с packData = null при работе на медленном железе
                if (packData == null) continue;  
                packData[channel][j++] = packet[channel][i];
              }
            });

            TData[] noiseLevel = new TData[MEA.MAX_CHANNELS];
            foreach (var chNoise in noiseLevelDic) noiseLevel[chNoise.Key] = chNoise.Value;
            
            // Create a new T-Pack (EOP)
            CPack pack = new CPack(m_firstSpikeTime, packLength, packData, noiseLevel);

            m_lastSpikeTime = 0;
            m_packDataList.Clear();
            m_inPack = false;

            // Check if we need to create a new pack in the next packet (actually in this, but we can't)
            m_firstSpikeTime = finishCurrentPack ? FindFirstSpikeTime(spikeTrainList) : 0;
            if (m_firstSpikeTime > 0)
            {
              Int32 prevPacketLength = m_prevPacket[m_prevPacket.Keys.ElementAt(0)].Length;

              // Calculate pack start relatively to the first data packet start
              TTime firstPacketTime = m_timestamp - ((m_firstSpikeTime >= m_timestamp + Param.PRE_SPIKE) ? 0 : (TTime)prevPacketLength);

              // Store the previous packet if we have to take data from it to create a new pack on the next iteration
              if (firstPacketTime < m_timestamp) m_packDataList.Add(m_prevPacket);
              m_packDataList.Add(packet);
              m_packDataStart = (Int32)(m_firstSpikeTime - firstPacketTime) - Param.PRE_SPIKE;

              // If the pack is going to be finished, remember the last spike time
              m_lastSpikeTime = (numActiveTrainsLeft < MIN_TRAINS_TO_CONTINUE_PACK) ? finishedTrains.Max(el => el.Start + (TTime)el.Length) : 0;

              // Store noise level at the beginning of a pack
              m_noiseInPack = noiseLevelDic;

              m_sendNewPack = true;
              m_inPack = true;
            }
            m_prevPacket = packet;
            return pack;
          }
          // Else, just wait for the next packet
        }
        #endregion
        m_packDataList.Add(packet);
      }
      #endregion
      else
      #region Process a packet outside a pack
      {
        // Here we should decide whether to start a new pack or not
        // If there are more MIN_TRAINS_TO_START_PACK spike-trains, just start a new pack immediately
        if (spikeTrainList.Count >= MIN_TRAINS_TO_START_PACK)
        {
          m_firstSpikeTime = FindFirstSpikeTime(spikeTrainList);
          if (m_firstSpikeTime > 0)
          {
            Int32 prevPacketLength = m_prevPacket[m_prevPacket.Keys.ElementAt(0)].Length;

            // Calculate pack start relatively to the first data packet start
            TTime firstPacketTime = m_timestamp - ((m_firstSpikeTime >= m_timestamp + Param.PRE_SPIKE) ? 0 : (TTime)prevPacketLength);
            
            if (firstPacketTime < m_timestamp) m_packDataList.Add(m_prevPacket);
            m_packDataList.Add(packet);

            // if (m_firstSpikeTime < firstPacketTime) throw new Exception("Pack start has been detected before data start"); // Should never happen
            m_packDataStart = (Int32)(m_firstSpikeTime - firstPacketTime) - Param.PRE_SPIKE;

            // If the pack is going to be finished, remember the last spike time
            m_lastSpikeTime = (numActiveTrainsLeft < MIN_TRAINS_TO_CONTINUE_PACK) ? finishedTrains.Max(el => el.Start + (TTime)el.Length) : 0;
            m_inPack = true;
            m_prevPacket = packet;

            // Store noise level at the beginning of a pack
            m_noiseInPack = noiseLevelDic;
            // Create and return a new S-Pack
            return new CPack(m_firstSpikeTime, 0); // length == 0 means S-Pack
          }
        }
      }
      #endregion

      // Remember the current packet in case we find start of a pack in the next packet
      m_prevPacket = packet;
      if (m_sendNewPack)
      {
        m_sendNewPack = false;
        // Create and return a new S-Pack
        return new CPack(m_firstSpikeTime, 0); // length == 0 means S-Pack
      }
      return null;
    }

    TTime FindFirstSpikeTime(List<CSpikeTrainFrame> spikeTrainList)
    {
      var spikeTrainsSorted = spikeTrainList.OrderBy(el => el.Start).ToList();
      //spikeTrainList.Sort((el1, el2) => (el1.Start > el2.Start) ? 1 : 0);

      // Discard spike-trains which violate MAX_FRONT_WIDTH restriction, i.e. were started too early (channels with high spontaneous activity)
      int i;
      do
      {
        TTime firstSpikeTime = spikeTrainsSorted[0].Start;
        for (i = 1; i < spikeTrainsSorted.Count; ++i)
        {
          if (spikeTrainsSorted[i].Start > firstSpikeTime + MAX_FRONT_WIDTH)
          {
            spikeTrainsSorted.RemoveAt(0);
            i = 0;
            break;
          }
          if (i + 1 == MIN_TRAINS_TO_START_PACK) break;
        }
      } while ((spikeTrainsSorted.Count >= MIN_TRAINS_TO_START_PACK) && (i + 1 < MIN_TRAINS_TO_START_PACK));

      // Check if there are enough channels remained to start a new pack
      return (spikeTrainsSorted.Count >= MIN_TRAINS_TO_START_PACK) ? spikeTrainsSorted[0].Start : 0;
    }
  }
}
