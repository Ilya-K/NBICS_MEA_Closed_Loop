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
    private const int Minimum_Pack_Requered_Count = 600;
    bool DoStatCollection = false;
    Thread CollectingDataThread;
    public delegate void DelegateSetProgress(object sender, int value);
    public event DelegateSetProgress SetVal;
    #endregion
    #region Констнуртор
    public CPackStat(CPackDetector _PackDetector)
    {
      PackListBefore = new List<CPack>();
      PackListAfter = new List<CPack>();
      PackDetector = _PackDetector;
      InitializeComponent();
      StatProgressBar.Maximum = Minimum_Pack_Requered_Count;
      SetVal += UpdateProgressBar;
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
      int maxInd = 0;
      int[] PackLengthDestrib = new int[400];
      //set as 0
      for (int i = 0; i < PackLengthDestrib.Length; i++)
      {
        //dt = 0.05 sec
        //summ time Length = 10 sec
        PackLengthDestrib[i] = 0;//50 / (1 + (i - 25) * (i - 25) / 10);
      }
      lock (PacklListBlock)
      {
        for (int i = 0; i < PackListBefore.Count() - 1; i++)
        {
          for (int j = 0; j < PackLengthDestrib.Length; j++)
          {
            if (PackListBefore[i + 1].Start - PackListBefore[i].Start > (TAbsStimIndex)j * 1250
              && PackListBefore[i + 1].Start - PackListBefore[i].Start < (TAbsStimIndex)(j + 1) * 1250)
            {
              PackLengthDestrib[j] += 1;
              break;
            }
          }
        }
        //Draw
        for (int i = 0; i < PackLengthDestrib.Length; i++)
        {
          e.Graphics.DrawLine(pen,
            i * 5,
            e.ClipRectangle.Height - 1 - PackLengthDestrib[i] * 3 * 50 / e.ClipRectangle.Height,
            i * 5 + 6,
            e.ClipRectangle.Height - 1 - PackLengthDestrib[i] * 3 * 50 / e.ClipRectangle.Height);
          if (PackLengthDestrib[i] > PackLengthDestrib[maxInd]) maxInd = i;

        }
        pen.Color = Color.Red;
        e.Graphics.DrawLine(pen, maxInd * 5 + 3, 0, maxInd * 5 + 3, e.ClipRectangle.Height);
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
      int InputCount = 0;
      Random rnd = new Random(DateTime.Now.Millisecond);
      while (true)
      {

        Thread.Sleep(20);
        while (DoStatCollection)
        {
          lock (PacklListBlock)
          {
            //PackListBefore.Add(PackDetector.WaitPack());
            {
              Thread.Sleep(5);//Вместо функции WaitPack()
              CPack pack_to_add = new CPack((TTime)(InputCount * 43000 + rnd.Next(12000)), 0, null);
              PackListBefore.Add(pack_to_add);
            }
            StatProgressBar.BeginInvoke(SetVal, null, 1);
            InputCount++;
            if (PackListBefore.Count() >= Minimum_Pack_Requered_Count
              || InputCount >= Minimum_Pack_Requered_Count - 1) // for debug(If WaitPuck dont work )
            {
              //this.CollectStatButton.Text = "Готово";
              MessageBox.Show("Собрано достаточное количество данных");
              this.DoStatCollection = false;
            }
          }
        }
      }
    }
    #region Подсчет статистики
    private void CalcStat()
    {
      Average Stat = new Average();
      for (int i = 0; i < PackListBefore.Count() - 1; i++)
      {
        Stat.AddValueElem(PackListBefore[i + 1].Start - PackListBefore[i].Start);
      }
      Stat.Calc();
      this.SelectedAverageBox.Text = (Stat.Value/25000).ToString() + " сек";
      this.SelectedSigmaBox.Text = (Stat.Sigma / 25000).ToString() + " сек";
      // Обновить график       
      this.DistribGrath.Refresh();

    }
    #endregion
    void UpdateProgressBar(object sender, int val)
    {
      StatProgressBar.Value += val;
    }

    private void CalcStatButton_Click(object sender, EventArgs e)
    {
      CalcStat();
    }

    private void PackCountGraph_Click(object sender, EventArgs e)
    {

    }
  }

}
