using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

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
  public partial class CPackStat : Form
  {
    #region Внутренние данные класса
    private CPackDetector PackDetector;
    public List<CPack> PackListBefore;
    public List<CPack> PackListAfter;
    public List<TStimIndex> StimList;
    private Object StimListBlock = new Object();
    private Object PacklListBlock = new Object();
    private const int Minimum_Pack_Requered_Count = 100;
    bool DoStatCollection = false;
    Thread CollectingDataThread;
    #endregion
    #region Констнуртор
    public CPackStat(CPackDetector _PackDetector)
    {
      PackListBefore = new List<CPack>();
      PackListAfter = new List<CPack>();
      PackDetector = _PackDetector;
      InitializeComponent();
      StatProgressBar.Maximum = 100;
    }
    #endregion

    private void CollectStatButton_Click(object sender, EventArgs e)
    {

      if (!DoStatCollection)
      {
        DoStatCollection = true;
        CollectStatButton.Text = "Остановить";
        if (CollectingDataThread == null)
        {
          CollectingDataThread = new Thread(CollectPacks);
          CollectingDataThread.Start();
        }
        else
        {
          //CollectingDataThread.Resume();
        }
      }
      else
      {
        DoStatCollection = false;
        CollectStatButton.Text = "Продолжить";
        //CollectingDataThread.Suspend();
      }
    }

    private void DistribGrath_Paint(object sender, PaintEventArgs e)
    {
      Pen pen = new Pen(Color.Blue);
      pen.Width = 1;
      int[] PackLengthDestrib = new int[50];
      //set as 0
      for (int i = 0; i < PackLengthDestrib.Length; i++)
      {
        //dt = 0.1 sec
        //summ time Length = 5 sec
        PackLengthDestrib[i] = 0;//50 / (1 + (i - 25) * (i - 25) / 10);
      }
      lock (PacklListBlock)
      {
        for (int i = 0; i < PackListBefore.Count() - 1; i++)
        {
          for (int j = 0; j < PackLengthDestrib.Length; j++)
          {
            if (PackListBefore[i].Start - PackListBefore[i + 1].Start > (TAbsStimIndex)j * 2500)
              PackLengthDestrib[j] += 1;
          }
        }
        //Draw
        for (int i = 0; i < PackLengthDestrib.Length; i++)
        {
          e.Graphics.DrawLine(pen,
            i * 10,
            PackLengthDestrib[i] * 3,
            i * 10 + 10,
            PackLengthDestrib[i] * 3);
        }
      }
    }
    public void RecieveStimData(List<TStimIndex> stimlist)
    {
      lock (StimListBlock)
      {
        if (stimlist != null)
        {
          foreach (TStimIndex stim in stimlist)
          {
            StimList.Add(stim);
          }
        }
      }
    }
    private void CollectPacks()
    {
      while (true)
      {
        Thread.Sleep(20);
        while (DoStatCollection)
        {
          lock (PacklListBlock)
          {
            PackListBefore.Add(PackDetector.WaitPack());
            StatProgressBar.Value += 1;
            if (PackListBefore.Count() >= Minimum_Pack_Requered_Count)
            {
              this.CollectStatButton.Text = "Готово";
              this.DoStatCollection = false;
            }
          }
        }
      }
    }
    private void CalcStat()
    {

    }
  }
}
