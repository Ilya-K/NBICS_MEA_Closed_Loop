using System;
using System.Collections.Generic;
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
      private CCalcExpWndSE m_se = new CCalcExpWndSE(167); // 3*tau = 20ms (500 samples)
      private CCalcExpWndSE m_seLT = new CCalcExpWndSE(16667); // 3*tau = 2s (50000 samples)
      private CExpAvg m_expAvg = new CExpAvg(167);
      private TData m_hpf = 0;      // High Pass Filter
      private TData m_maOld = 0;
      private bool m_packFound = false;

      public List<Int32> FindSpikeTrains(TData[] data)
      {
        List<Int32> trainList = null;

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
            if (trainList == null) trainList = new List<Int32>();         // Create List only when pack is detected
            trainList.Add(i);
          }

          if (m_packFound && (diff < 0))
          {

          }
        }
        return trainList;
      }

    }

    private Dictionary<int, CSpikeTrainDetector> m_stDet;
    private CFiltering m_filteredData;

    // [DEBUG]
    private System.Timers.Timer m_dummyTimer;
    Int32 m_dummyPackTime = -1;
    Random m_rnd;

    public CPackDetector(CFiltering filteredData)
    {
      m_filteredData = filteredData;
      m_stDet = new Dictionary<int, CSpikeTrainDetector>(filteredData.ChannelList.Count);
      foreach (int channel in filteredData.ChannelList) m_stDet[channel] = new CSpikeTrainDetector();

      // [DEBUG]
      m_dummyTimer = new System.Timers.Timer(4000);
      m_dummyTimer.Elapsed += DummyTimer;
      m_dummyTimer.Start();
      m_rnd = new Random(123);
      
    }

    private void DummyTimer(object o1, EventArgs e1)
    {
      lock (m_dummyTimer)
      {
        m_dummyPackTime = m_rnd.Next(2500);
      }
    }

    public CPack WaitPack()
    {
      CPack pack = null;
      TFltDataPacket packet;
      List<TTime> packList = null;

      do
      {
        packet = m_filteredData.WaitData();
        packList = DetectPacks(packet);
      } while (packList == null);

      return pack;
    }

    private List<TTime> DetectPacks(TFltDataPacket packet)
    {
      List<TTime> packList = new List<TTime>();

      // [DEBUG]
      #region DEBUG

      if (m_dummyPackTime >= 0)
      {
        packList.Add((TTime)m_dummyPackTime);
        m_dummyPackTime = -1;
      }
      
      return packList;
      #endregion // DEBUG

      Dictionary<int, List<Int32>> spikeTrains = new Dictionary<int, List<int>>();

      // Fill keys to enable parallel foreach on the next step
      foreach (int channel in packet.Keys) spikeTrains[channel] = null;
      packet.Keys.AsParallel().ForAll(channel => spikeTrains[channel] = m_stDet[channel].FindSpikeTrains(packet[channel]));

      

      return packList;
    }

    // [OBSOLETE] Moved to class CSpikeTrainDetector
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
