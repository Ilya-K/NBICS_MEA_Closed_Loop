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
    private Dictionary<int, Bitmap> BurstPlotData = new Dictionary<int, Bitmap>();
    private List<int> KeysCollection = new List<int>();

    private const int MaxBurstPlotWidth = 3300;
    private const int MaxBurstPlotDuration = MaxBurstPlotWidth * Param.MS;
    private int CurrentChannelID = 58;

    private Queue<SEvokedPack> BurstQueue = new Queue<SEvokedPack>();

    private object LockBurstQueue = new object();
    private object LockBurstPlotData = new object();


    private int SigmaCount = 8;

    public FChSorter()
    {
      InitializeComponent();
    }

    public void ProcessEvBurst(SEvokedPack evBurst)
    {
      //Сдвиг начала  пачки (если стимул произошел раньше)
      int PackShift = (evBurst.stim < evBurst.Burst.Start) ? (int)(evBurst.Burst.Start - evBurst.stim) : 0;

      //Смещение пачки влево (если стимул внутри пачки)
      int StimShift = (evBurst.stim > evBurst.Burst.Start) ? (int)(evBurst.stim - evBurst.Burst.Start) : 0;

      TTime StartDrawTime = (TTime)StimShift + Param.PRE_SPIKE  - (TTime)PackShift;
      TTime EndDrawTime = (TTime)StimShift + Param.PRE_SPIKE  - (TTime)PackShift ;

      lock (LockBurstQueue) BurstQueue.Enqueue(evBurst);
      foreach (int key in evBurst.Burst.Data.Keys)
      {

        if (ChekSpike(evBurst, 0, ulong.Parse(T1EndValue.Text) * Param.MS, key))
        {
          T1[key] += 1;
        }
        if (ChekSpike(evBurst, ulong.Parse(T1EndValue.Text) * Param.MS, ulong.Parse(T3StartValue.Text) * Param.MS, key))
        {
          T2[key] += 1;
        }
        if (ChekSpike(evBurst, ulong.Parse(T3StartValue.Text) * Param.MS, ulong.Parse(T1EndValue.Text) * Param.MS + 100 * Param.MS, key))
        {
          T3[key] += 1;
        }
        TFltDataPacket data = evBurst.Burst.Data;
        Brush brush = new SolidBrush(Color.Gray);
        Pen grayPen = new Pen(brush);
        lock (LockBurstPlotData)
        using (Graphics gr = Graphics.FromImage(BurstPlotData[key]))
        {

          for (int i = (int)StartDrawTime + 1; i < data[key].Length && i < (int)StartDrawTime + MaxBurstPlotDuration; i++)
          {
            gr.DrawLine(grayPen,
              (float)((i - 1) * .5),
              (float)(data[key][i - 1]*.1 + 50),
              (float)(i * (float).5),
              (float)(data[key][i] * .1 + 50));
          }
        }

        //refresh table
        //TODO need export to anther function like RefreshTable()
        foreach (DataGridViewRow row in StatTable.Rows)
        {
          if (row.Cells[0].Value.Equals(key))
          {
            row.Cells[2].Value = T1[key];///(double) BurstQueue.Count;
            row.Cells[3].Value = T2[key];///(double) BurstQueue.Count;
            row.Cells[4].Value = T3[key];///(double) BurstQueue.Count;
            row.Cells[5].Value = T2[key] * 100 / (T1[key] + 1);
            row.Cells[6].Value = T2[key] * 100 / (T3[key] + 1);
          }
        }
      }


    }

    private bool ChekSpike(SEvokedPack ev_pack, TTime StartTime, TTime EndTime, int Channel)
    {
      //Pack Start Time && Stim Time may be absolute but Center && Delta mast be relative (all in Points), NOT MS!

      //Сдвиг начала  пачки (если стимул произошел раньше)
      int PackShift = (ev_pack.stim < ev_pack.Burst.Start) ? (int)(ev_pack.Burst.Start - ev_pack.stim) : 0;

      //Смещение пачки влево (если стимул внутри пачки)
      int StimShift = (ev_pack.stim > ev_pack.Burst.Start) ? (int)(ev_pack.stim - ev_pack.Burst.Start) : 0;

      TTime StartSearchTime = (TTime)StimShift + Param.PRE_SPIKE + StartTime - (TTime)PackShift;
      TTime EndSearchTime = (TTime)StimShift + Param.PRE_SPIKE + EndTime - (TTime)PackShift;
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
        if (!T1.ContainsKey(key))
          T1.Add(key, 0);
        if (!T2.ContainsKey(key))
          T2.Add(key, 0);
        if (!T3.ContainsKey(key))
          T3.Add(key, 0);
        lock (LockBurstPlotData)
        {
          if(!BurstPlotData.ContainsKey(key))
          BurstPlotData.Add(key, new Bitmap(AverageBurstPanel.Width, MaxBurstPlotWidth));
        }
      }
      foreach (DataGridViewRow row in StatTable.Rows)
      {
        row.Cells[0].Style.BackColor = Color.Aqua;
        row.Cells[1].Style.BackColor = Color.LightBlue;
        row.Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        row.Cells[3].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        row.Cells[4].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
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

    private void StatTable_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      CurrentChannelID = (int)StatTable.Rows[e.RowIndex].Cells[0].Value;
      if (BurstPlotData.Keys.Contains(CurrentChannelID))
        AverageBurstPanel.Image = (Bitmap)BurstPlotData[CurrentChannelID].Clone();
    }

    private void AverageBurstPanel_Paint(object sender, PaintEventArgs e)
    {
    }

    private void AverageBurstPanel_Click(object sender, EventArgs e)
    {
      AverageBurstPanel.Refresh();
    }

    private void AverageBurstPanel_DoubleClick(object sender, EventArgs e)
    {
      switch (MessageBox.Show("отобразить подробно?", "", MessageBoxButtons.YesNo))
      {
        case System.Windows.Forms.DialogResult.Yes:

          break;
        case System.Windows.Forms.DialogResult.No:
          break;
      }
    }

  }
}
