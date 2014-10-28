using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using MEAClosedLoop;
namespace MEAClosedLoop
{
  #region Definitions
  using TTime = System.UInt64;
  using TData = System.Double;
  using TFltDataPacket = Dictionary<int, System.Double[]>;
  using TStimIndex = System.Int16;
  using TAbsStimIndex = System.UInt64;
  using TRawData = UInt16;
  using TRawDataPacket = Dictionary<int, ushort[]>;
  #endregion
  public partial class FChSorter : Form, IRecieveEvokedBurst
  {
    //private delegate void ProcessBurstDelegate(CPack burst);
    //private ProcessBurstDelegate processBurstDelegate;
    //private delegate void ProcessStimDelegate(TAbsStimIndex stim);
    //private ProcessStimDelegate processStimDelegate;

    private delegate void ProcessEvBurstdelegate(SEvokedPack evBurst);
    private ProcessEvBurstdelegate processEvBurstDelegate;

    private Dictionary<int, int> T1 = new Dictionary<int, int>();
    private Dictionary<int, int> T2 = new Dictionary<int, int>();
    private Dictionary<int, int> T3 = new Dictionary<int, int>();
    private List<int> KeysCollection = new List<int>();

    private Queue<SEvokedPack> BurstQueue = new Queue<SEvokedPack>();

    private object LockBurstQueue = new object();

    private int SigmaCount = 8;

    public FChSorter()
    {
      InitializeComponent();
    } 

    public void ProcessEvBurst(SEvokedPack EvBusrt)
    {
      lock (LockBurstQueue) BurstQueue.Enqueue(EvBusrt);
      foreach (int key in T1.Keys)
      {
        if (ChekSpike(EvBusrt, 0, ulong.Parse(T1EndValue.Text), key))
        {
          T1[key] += 1;
        }
        if (ChekSpike(EvBusrt, ulong.Parse(T1EndValue.Text) * Param.MS, ulong.Parse(T3StartValue.Text) * Param.MS, key))
        {
          T2[key] += 1;
        }
        if (ChekSpike(EvBusrt, ulong.Parse(T3StartValue.Text) * Param.MS, ulong.Parse(T1EndValue.Text) * Param.MS + 25000, key))
        {
          T3[key] += 1;
        }
        int rowID = 0;
        foreach (DataGridViewRow row in StatTable.Rows)
        {
          if(row.Cells[0].Value.Equals(key))
            rowID = row.Index;
        }
        StatTable.Rows[rowID].Cells[2].Value = T1[key];///(double) BurstQueue.Count;
        StatTable.Rows[rowID].Cells[3].Value = T2[key];///(double) BurstQueue.Count;
        StatTable.Rows[rowID].Cells[4].Value = T3[key];///(double) BurstQueue.Count;
      }

      //refresh table
      //TODO need export to anther function like RefreshTable()
      

    }
    private bool ChekSpike(SEvokedPack ev_pack, TTime StartTime, TTime EndTime, int Channel)
    {
      //Pack Start Time && Stim Time may be absolute but Center && Delta mast be relative (all in Points), NOT MS!

      //Сдвиг начала  пачки (если стимул произошел раньше)
      int PackShift = (ev_pack.stim < ev_pack.Burst.Start) ? (int)(ev_pack.Burst.Start - ev_pack.stim) : 0;

      //Смещение пачки влево (если стимул внутри пачки)
      int StimShift = (ev_pack.stim > ev_pack.Burst.Start) ? (int)(ev_pack.stim - ev_pack.Burst.Start) : 0;

      TTime StartSearchTime = (TTime)StimShift + Param.PRE_SPIKE + StartTime - (TTime)PackShift;

      Average average = new Average();

      //вычесление среднего и сигмы для участка данных перед пачкой

      for (int i = 0; i < Param.PRE_SPIKE; i++)
      {
        average.AddValueElem(Math.Abs(ev_pack.Burst.Data[0][i]));
      }
      average.Calc();

      double[] BurstData = ev_pack.Burst.Data[Channel];
      for (TTime i = StartSearchTime; i < EndTime && i < (TTime)ev_pack.Burst.Length - 1; i++)
      {
        double Value = Math.Abs(BurstData[i]);
        if (Value > (average.Sigma * SigmaCount))
        {
          return true;
        }
      }
      return false;
    }

    public void PrepeareTable(SEvokedPack evBurst)
    {
      processEvBurstDelegate -= PrepeareTable;
      foreach (int key in evBurst.Burst.Data.Keys)
      {
        StatTable.Rows.Add(key, MEA.IDX2NAME[key]);
        T1.Add(key, 0);
        T2.Add(key, 0);
        T3.Add(key, 0);
      }
      processEvBurstDelegate += ProcessEvBurst;
    }



    private void StartButton_Click(object sender, EventArgs e)
    {
      processEvBurstDelegate += PrepeareTable;
      StartButton.Enabled = false;
    }

    private void FChSorter_FormClosing(object sender, FormClosingEventArgs e)
    {
      processEvBurstDelegate -= ProcessEvBurst;
    }

    public void RecieveEvokedBurst(SEvokedPack evBurst)
    {
      BeginInvoke(processEvBurstDelegate, evBurst);
    }

    private void FChSorter_Load(object sender, EventArgs e)
    {
      SigmaUpDown.Value = SigmaCount;
    }

    private void SigmaUpDown_ValueChanged(object sender, EventArgs e)
    {
      SigmaCount = (int)SigmaUpDown.Value;
    }

  }
}
