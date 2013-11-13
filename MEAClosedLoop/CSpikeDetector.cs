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
  using TFltDataPacket = Dictionary<int, System.Double[]>;
  
  public class CSpikeDetector
  {
    private const int MAX_NUM_CHANNELS = 60;
    private const int SE_AVG_RANGE = 20;        // 2 sec
    private CFiltering m_filteredStream;
    private Queue<Spike> m_spikeQueue;
    private AutoResetEvent m_notEmpty;
    private ulong m_prevSlice;
    private TData m_threshold;
    private double[] m_thresholds;
    private volatile bool m_kill;
    private Dictionary<int, CCalcSE_Block> m_calcSE;


    public CSpikeDetector(CFiltering filteredStream, TData threshold)
    {
      m_threshold = threshold;
      m_prevSlice = 0;
      m_filteredStream = filteredStream;
      m_filteredStream.OnStreamKill = Dismiss;
      m_spikeQueue = new Queue<Spike>();
      m_notEmpty = new AutoResetEvent(false);
      m_thresholds = new double[MAX_NUM_CHANNELS];
      m_calcSE = new Dictionary<int, CCalcSE_Block>(filteredStream.NChannels);
      filteredStream.ChannelList.ForEach(channel => m_calcSE[channel] = new CCalcSE_Block(SE_AVG_RANGE));

      Thread t = new Thread(new ThreadStart(DetectSpikes));
      m_kill = false;
      t.Start();
    }

    private void Dismiss()
    {
      m_kill = true;
      m_notEmpty.Set();
    }

    public Spike WaitData()
    {
      Spike spikesSlice = null;
      do
      {
        m_notEmpty.WaitOne();
        lock (m_spikeQueue)
        {
          if (m_spikeQueue.Count > 0)
          {
            spikesSlice = m_spikeQueue.Dequeue();
            if (m_spikeQueue.Count > 0) m_notEmpty.Set();
          }
          else if (m_kill) break;
        }
      } while (spikesSlice == null);

      return spikesSlice;
    }

    public void DetectSpikes()
    {
      do
      {
        TFltDataPacket currPacket = m_filteredStream.WaitData();
        if (m_kill) break;    // If we've caught kill signal in WaitData()

        // Calculate Standard Error over SE_AVG_RANGE recent packets
        currPacket.Keys.AsParallel().ForAll(channel =>
        {
          double mean, se;
          m_calcSE[channel].se(currPacket[channel], out mean, out se);
          m_thresholds[channel] = mean + m_threshold * se;
        });
        
        int packetLength = currPacket.First().Value.Length;
        ulong timestamp = m_filteredStream.TimeStamp;
        // [ToDo] Если потребуется уменьшить время реакции, то порезать блоки на куски тут
        // [ToDo] Extremely stupid realization. Should be replaced by something more smart.
        for (uint i = 0; i < packetLength; ++i)
        {
          ulong currSlice = 0;
          foreach (int channel in currPacket.Keys)
          {
            currSlice |= (currPacket[channel][i] < m_thresholds[channel]) ? (1UL << channel) : 0;
          }
          ulong spikeData = currSlice & ~m_prevSlice;
          m_prevSlice = currSlice;
          if (spikeData != 0)
          {
            lock (m_spikeQueue) m_spikeQueue.Enqueue(new Spike(timestamp + i, spikeData));
            m_notEmpty.Set();
          }
        }
      } while (!m_kill);
    }
  }
}
