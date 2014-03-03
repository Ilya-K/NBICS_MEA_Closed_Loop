using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using MEAClosedLoop.Common;
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
    #region Стандартные значения
    int WAIT_PACK_WINDOW_LENGTH = 25; // 25 ms 
    #endregion
    #region Внутренние данные класса
    private CPackDetector PackDetector;
    public List<CPack> PackListBefore; //befor stim started
    public List<CPack> PackListAfter; // After stim started
    public List<CPack> PackListDetectReaction; // Лист пачек, которые считаются реакцией культуры
    public List<TStimIndex> StimList;
    private Object StimListBlock = new Object();
    private Object PacklListBlock = new Object();
    private const int Minimum_Pack_Requered_Count = 600;
    bool DoStatCollection = false;
    bool DoDrawStartStimTime = false;
    bool DoStimulation = false;
    List<int> m_channelList; //TODO: get channel list
    state CurrentState;
    Thread CollectingDataThread;
    public delegate void DelegateSetProgress(object sender, int value);
    public delegate void DelegateUpdateDistribGrath();
    public delegate void DelegateSetCollectStatButtonText(string text);
    public event DelegateSetProgress SetVal;
    public event DelegateUpdateDistribGrath UpdateDistribGrath;
    public event DelegateSetCollectStatButtonText SetCollectStatButtonText;

    private double StimStartPosition;
    #endregion
    #region Конструктор
    public CPackStat(CPackDetector _PackDetector, List<int> channelList)
    {
      InitializeComponent();
      CurrentState = state.BeforeStimulation;
      PackListBefore = new List<CPack>();
      PackListAfter = new List<CPack>();
      StimList = new List<TStimIndex>();
      PackDetector = _PackDetector;
      StatProgressBar.Maximum = Minimum_Pack_Requered_Count;
      m_channelList = channelList;

      SetVal += UpdateProgressBar;
      UpdateDistribGrath += DistribGrath.Refresh;
      UpdateDistribGrath += CalcStat;
      SetCollectStatButtonText += SetStatButtontext;
    }
    #endregion

    #region Клик по кнопке выбора канала
    private void GraphChannelSelectButton_Click(object sender, EventArgs e)
    {
      PackGraphForm formShowWindows = new PackGraphForm(m_channelList);
      formShowWindows.loadSelection += ChannelChangeRequest;
      formShowWindows.Show();
    }
    #endregion

    #region Клик по кнопке сбора статистики
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
    private void SetStatButtontext(string text)
    {
      CollectStatButton.Text = text;
    }
    #endregion
    #region Перерисовка поля статистики пачек
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
            i * 5 + 4,
            e.ClipRectangle.Height - 1 - PackLengthDestrib[i] * 3 * 50 / e.ClipRectangle.Height);
          if (PackLengthDestrib[i] > PackLengthDestrib[maxInd]) maxInd = i;

        }
        //Draw Average
        pen.Color = Color.Red;
        e.Graphics.DrawLine(pen, maxInd * 5 + 3, 0, maxInd * 5 + 3, e.ClipRectangle.Height);
        //Draw Start Stim Time
        pen.Color = Color.Green;
        if (DoDrawStartStimTime)
        {
          e.Graphics.DrawLine(pen,
            (float)StimStartPosition / 25,
            0,
            (float)StimStartPosition / 25,
            e.ClipRectangle.Height);
        }
      }
    }
    #endregion
    #region CallBack фукция для принятия информации об стмуляциях.
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
    
    #endregion
    #region Функция для заргрузки информации об пачках, исполняется в отдельном потоке
    private void CollectPacks()
    {
      int InputCount = 0;
      Random rnd = new Random(DateTime.Now.Millisecond);
      while (true)
      {

        Thread.Sleep(10);
        while (DoStatCollection)
        {
          lock (PacklListBlock)
          {
            switch (CurrentState)
            {
              case state.BeforeStimulation:
                //PackListBefore.Add(PackDetector.WaitPack());
                {
                  Thread.Sleep(5);//Вместо функции WaitPack()
                  CPack pack_to_add = new CPack((TTime)(InputCount * 43000 + rnd.Next(12000)), 0, null);
                  PackListBefore.Add(pack_to_add);
                }
                StatProgressBar.BeginInvoke(SetVal, null, 1);
                DistribGrath.BeginInvoke(UpdateDistribGrath);
                InputCount++;
                if (PackListBefore.Count() >= Minimum_Pack_Requered_Count
                  || InputCount >= Minimum_Pack_Requered_Count - 1) // for debug(If WaitPack dont work )
                {
                  CollectStatButton.BeginInvoke(SetCollectStatButtonText, "готово");
                  this.DoStatCollection = false;
                }
                break;
              case state.AfterStimulation:
                //PackListAfter.Add(PackDetector.WaitPack());
                {
                  Thread.Sleep(25);//Вместо функции WaitPack()
                  CPack pack_to_add = new CPack((TTime)(InputCount * 40000 + rnd.Next(16000)), 0, null);
                  PackListAfter.Add(pack_to_add);
                }
                break;
            }
          }
        }
      }
    }
/*    private void CollectPacks() //new, under construction
    {
      int InputCount = 0;
      //Random rnd = new Random(DateTime.Now.Millisecond);
      while (true)
      {

        //Thread.Sleep(10);
        //while (DoStatCollection)
        //{
          lock (PacklListBlock)
          {
            switch (CurrentState)
            {
              case state.BeforeStimulation:
                //PackListBefore.Add(PackDetector.WaitPack());
                {
                  //Thread.Sleep(5);//Вместо функции WaitPack()
                  CPack pack_to_add = new CPack((TTime)(InputCount * 43000 + rnd.Next(12000)), 0, null);
                  PackListBefore.Add(pack_to_add);
                }
                StatProgressBar.BeginInvoke(SetVal, null, 1);
                DistribGrath.BeginInvoke(UpdateDistribGrath);
                InputCount++;
                if (PackListBefore.Count() >= Minimum_Pack_Requered_Count
                  || InputCount >= Minimum_Pack_Requered_Count - 1) // for debug(If WaitPack dont work )
                {
                  CollectStatButton.BeginInvoke(SetCollectStatButtonText, "готово");
                  this.DoStatCollection = false;
                }
                break;
              case state.AfterStimulation:
                //PackListAfter.Add(PackDetector.WaitPack());
                {
                  //Thread.Sleep(25);//Вместо функции WaitPack()
                  CPack pack_to_add = new CPack((TTime)(InputCount * 40000 + rnd.Next(16000)), 0, null);
                  PackListAfter.Add(pack_to_add);
                }
                break;
            }
          }
        //}
      }
    }*/
    
    #endregion
    #region Подсчет статистики
    private void CalcStat()
    {
      Average Stat = new Average();
      for (int i = 0; i < PackListBefore.Count() - 1; i++)
      {
        Stat.AddValueElem(PackListBefore[i + 1].Start - PackListBefore[i].Start);
      }
      Stat.Calc();
      this.SelectedAverageBox.Text = (Stat.Value / 25000).ToString() + " сек";
      this.SelectedSigmaBox.Text = (Stat.Sigma / 25000).ToString() + " сек";
      // Обновить график       
      this.DistribGrath.Refresh();

    }
    #endregion
    #region Обработка события обновления прогресс бара
    void UpdateProgressBar(object sender, int val)
    {
      if (StatProgressBar.Value < StatProgressBar.Maximum)
        StatProgressBar.Value += val;
    }
    #endregion
    #region Обработка скрола ползунка времени стимуляции
    private void trackBar1_Scroll(object sender, EventArgs e)
    {
      DoDrawStartStimTime = true;
      //2000p - 20sec - maximum
      StimStartPosition = trackBar1.Value * 25 / 5;
      DistribGrath.Refresh();
    }
    #endregion

    private void button1_Click(object sender, EventArgs e)
    {
      StimParams paramswindow = new StimParams(25);
      paramswindow.ShowDialog();
      if (paramswindow.DoUpdateParams)
      {
        WAIT_PACK_WINDOW_LENGTH = paramswindow.Time;
      }
    }
    private void StartStimButton_Click(object sender, EventArgs e)
    {
      CurrentState = state.AfterStimulation;
      DoStatCollection = true;
      DoStimulation = true;

    }

    private enum state
    {
      BeforeStimulation,
      AfterStimulation
    }

    private void CPackStat_Load(object sender, EventArgs e)
    {

    }

    void ChannelChangeRequest(int number)
    {
      this.numericUpDown1.Value = number;
    }

    private void numericUpDown1_ValueChanged(object sender, EventArgs e)
    {

    }
  }

}
