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
    private class CSpikeTrainDetector
    {
      private const TData THRESH1 = 0.14;
      private const TData THRESH2 = 1.0;
      private const TData DECAY = 0.98;

      private TTime m_absTime = 0;
      private Int16 m_channel;
      
      // Short term mean and variance calculator
      private CCalcExpWndSE m_se = new CCalcExpWndSE((20 * Param.MS) / 3);      // 3*tau = 20ms (500 samples)
      
      // Long term mean and variance calculator
      private CCalcExpWndSE m_seLT = new CCalcExpWndSE((2000 * Param.MS) / 3);  // 3*tau = 2s (50000 samples)
      
      // Moving average with exponential window
      private CExpAvg m_expAvg = new CExpAvg(167);                              // Window width ~500 packets (50s)
      
      private TData m_hpf = 0;      // High Pass Filter
      private TData m_maOld = 0;
      private bool m_packFound = false;

      public CSpikeTrainDetector(Int16 channel)
      {
        m_channel = channel;
      }

      public CSpikeTrainFrame FindSpikeTrains(TData[] data)
      {
        CSpikeTrainFrame spikeTrain = null;

        int size = data.Length;

        TData ma = m_expAvg.Add(m_se.SE(data[0]) - m_seLT.SE(data[0]));
        TData diff = ma - m_maOld;
        m_maOld = ma;
        m_hpf = (Math.Abs(diff) > THRESH1) ? m_hpf + diff : m_hpf * DECAY;

        for (int i = 1; i < data.Length; i++)
        {
          TData delta = m_se.SE(data[i]) - m_seLT.SE(data[i]);
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
        }

        m_absTime += (TTime)size;
        return spikeTrain;
      }
    }

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
    private TTime m_timeStamp = 0;
    private Int32 m_prevPacketLength = 0;
    private bool m_inPack = false;
    private volatile bool m_kill = false;

    // [DEBUG]
    private int m_entryCount = 0;
    private int m_entryCount2 = 0;
    private System.Timers.Timer m_dummyTimer;
    Int32 m_dummyPackTime = -1;
    Random m_rnd;

    public CPackDetector(CFiltering filteredStream, List<int> channelList = null)
    {
      if (filteredStream == null) throw new ArgumentNullException("filteredStream");

      m_filteredStream = filteredStream;
      if (channelList == null)
      {
        channelList = Enumerable.Range(0, MEA.MAX_CHANNELS - 1).ToList();
      }
      m_activeChannelList = channelList;
      m_spikeTraintDet = new Dictionary<int, CSpikeTrainDetector>(channelList.Count);
      foreach (Int16 channel in m_activeChannelList) m_spikeTraintDet[channel] = new CSpikeTrainDetector(channel);

      m_packSpikeTrainList = new List<CSpikeTrainFrame>();
      m_prevSpikeTrains = new List<CSpikeTrainFrame>();
      m_packDataList = new List<TFltDataPacket>();
      m_prevPacket = new TFltDataPacket();
      m_prevPacket[0] = new TData[0];                 // just to avoid NullReferenceException at the first packet
      m_filteredQueue = new Queue<TFltDataPacket>();
      m_notEmpty = new AutoResetEvent(false);

      m_filteredStream.AddDataConsumer(ReceiveData);


      // [DEBUG]
      /*
      m_dummyTimer = new System.Timers.Timer(4000);
      m_dummyTimer.Elapsed += DummyTimer;
      m_dummyTimer.Start();
      m_rnd = new Random(123);
      */
    }

    private void DummyTimer(object o1, EventArgs e1)
    {
      lock (m_dummyTimer)
      {
        m_dummyPackTime = m_rnd.Next(2500);
      }
    }

    private void ReceiveData(TFltDataPacket packet)
    {
      lock (m_filteredQueue) m_filteredQueue.Enqueue(packet);
      m_notEmpty.Set();
    }

    /*
    public CPack WaitPack()
    {
      CPack pack = null;
      do
      {
        m_entryCount2++;
        pack = DetectPacks(m_filteredStream.WaitData());
      } while (pack == null);

      return pack;
    }
    */

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

        m_timeStamp += (TTime)m_prevPacketLength;
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
    // Считаем, что в одном пакете на одном электроде не может быть более одного спайк-трэйна.
    // Если мы вне пачки, то если найден хотя бы один спайк трэйн, начинаем пачку.
    //
    //
    //
    // DetectPacks может возвращать S-Pack (Start), null или T-Pack (Tail).
    // null означает середину или отсутствие пачки
    
    private CPack DetectPacks(TFltDataPacket packet)
    {
      #region DEBUG
      m_entryCount++;
      // [DEBUG]
      /*

      CPack debugPack = null;
      if (m_dummyPackTime >= 0)
      {
        debugPack = new CPack((TTime)m_dummyPackTime, 0, null);
        m_dummyPackTime = -1;
      }

      return debugPack;
      */
      #endregion // DEBUG

      int currPacketLength = packet[packet.Keys.ElementAt(0)].Length;

      Dictionary<int, CSpikeTrainFrame> spikeTrains = new Dictionary<int, CSpikeTrainFrame>();

      // Fill keys to enable parallel foreach on the next step
      foreach (int channel in m_activeChannelList) spikeTrains[channel] = null;
      // Find spike-trains on all active electrodes

      m_activeChannelList.AsParallel().ForAll(channel => spikeTrains[channel] = m_spikeTraintDet[channel].FindSpikeTrains(packet[channel]));

      // Select found spike-trains
      var spikeTrainList = spikeTrains.Values.Where(any => any != null).ToList();

      // Check if spikeTrains contains at least one spike-train
      if (spikeTrainList.Count > 0)
      {
        // Check if all spike-trains finish here
        bool packContinues = spikeTrainList.Any(el => !el.EOP);

        if (m_inPack)
        {
          // Throws an exception if some spike-trains have been lost. Should never happen.
          ConsistencyCheck(m_prevSpikeTrains, spikeTrainList);
          m_prevSpikeTrains = spikeTrainList;
          m_packDataList.Add(packet);

          // If the pack is going to be finished, remember the last spike time
          m_lastSpikeTime = packContinues ? 0 : spikeTrainList.Max(el => el.Start + (TTime)el.Length);
        }
        else                                                // Outside a pack
        {
          // Here we should decide whether to start a new pack or not
          if (spikeTrainList.Count > 1)                     // If there are more than one spike-train, just start the new pack
          {
            Int32 prevPacketLength = m_prevPacket[m_prevPacket.Keys.ElementAt(0)].Length;
            m_firstSpikeTime = spikeTrainList.Min(el => el.Start);
            if (m_prevSpikeTrains.Count > 0)                // Account for a single spike-train in the previous packet if it was there
            {
              TTime prevSpikeTrainTime = spikeTrainList.Min(el => el.Start);

              if (prevSpikeTrainTime + (TTime)prevPacketLength >= m_timeStamp + Param.PRE_SPIKE)
              {
                m_packDataList.Add(m_prevPacket);
                m_firstSpikeTime = prevSpikeTrainTime;
              }
              else                                          // Something wrong has happened, but we won't care about this.
              {
                // This could happen if we have strong spontaneous activity at one channel. And also the first packet could fall here
                // So just start a new pack from the current packet
              }
            }
            m_packDataList.Add(packet);

            TTime firstPacketTime = m_timeStamp - ((m_firstSpikeTime > m_timeStamp) ? 0 : (TTime)prevPacketLength);

            if (m_firstSpikeTime < firstPacketTime) throw new Exception("Pack start has been detected before data start"); // Never should happen
            m_packDataStart = (Int32)(m_firstSpikeTime - firstPacketTime);

            // Create a new S-Pack
            CPack pack = new CPack(m_firstSpikeTime, 0, null); // length == 0 means S-Pack

            m_inPack = true;
            m_prevSpikeTrains = spikeTrainList;
            return pack;
          }
          // Else, we have only one spike-train. Just remember it for the better times
          m_prevSpikeTrains = spikeTrainList;
          // Remember the current packet in case we find start of a pack in the next packet
          m_prevPacket = packet;
        }
      }
      else                                                  // spikeTrainList == 0, we haven't found any spike-train in the current packet
      {
        m_prevPacket = packet;
        m_prevSpikeTrains = spikeTrainList;

        if (m_inPack)
        {
          // If there are unfinished spike-trains, throw the exception
          if (m_lastSpikeTime == 0) throw new Exception("Something is wrong in Spike-train Detector. Spike-train has been lost.");

          // If we are in a pack and haven't got any spikes for POST_SPIKE time, terminate this pack
          if (m_timeStamp + (TTime)currPacketLength > m_lastSpikeTime + Param.POST_SPIKE)
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
      return null;
    }

    void ConsistencyCheck(List<CSpikeTrainFrame> prevSpikeTrains, List<CSpikeTrainFrame> spikeTrainList)
    {
      if (m_lastSpikeTime == 0)                             // There were some unfinished spike-trains in the previous packet
      {
        if (prevSpikeTrains.Where(el => !el.EOP).Any(el1 => !spikeTrainList.Any(el2 => el2.Channel == el1.Channel)))
        {
          throw new Exception("Something is wrong in Spike-train Detector. Spike-train has been lost.");
        }
      }
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
