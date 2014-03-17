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
  public delegate void DelegateSetProgress(object sender, int value);
  #endregion
  public partial class CPackStat : Form
  {
    #region Стандартные значения
    int WAIT_PACK_WINDOW_LENGTH = 25; // 25 ms 
    private const int Minimum_Pack_Requered_Count = 20;
    #endregion
    #region Внутренние данные класса
    private CPackDetector PackDetector;
    private CLoopController LoopCtrl;
    public List<CPack> PackListBefore; //befor stim started
    public List<CPack> PackListAfter; // After stim started
    public List<CPack> PackListDetectReaction = new List<CPack>(); // Лист пачек, которые считаются реакцией культуры
    public List<TAbsStimIndex> StimList;
    private Object StimListBlock = new Object();
    private Object PacklListBlock = new Object();
    private TTime StartStimulationTime = 0; // точка отсчета начала стимуляций. 
    private int Stat_Window_Minutes = 0;
    private int Stat_Window_Seconds = 10;
    bool DoStatCollection = false;
    bool DoDrawStartStimTime = false;
    bool DoStimulation = false;
    List<int> m_channelList; //TODO: get channel list
    state CurrentState;


    
    public delegate void DelegateUpdateDistribGrath();
    public delegate void DelegateSetCollectStatButtonText(string text);
    public event DelegateSetProgress SetVal;
    public event DelegateUpdateDistribGrath UpdateDistribGrath;
    public event DelegateSetCollectStatButtonText SetCollectStatButtonText;

    private double StimStartPosition;
    #endregion
    #region Конструктор
    public CPackStat(CPackDetector _PackDetector, CLoopController _LoopController, List<int> channelList)
    {
      InitializeComponent();
      CurrentState = state.BeforeStimulation;
      PackListBefore = new List<CPack>();
      PackListAfter = new List<CPack>();
      StimList = new List<TAbsStimIndex>();
      PackDetector = _PackDetector;
      LoopCtrl = _LoopController;
      StatProgressBar.Maximum = Minimum_Pack_Requered_Count + 1;
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
      PackGraphForm formShowWindows = new PackGraphForm(m_channelList, LoopCtrl);
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
        /*if (CollectingDataThread == null)
        {
          CollectingDataThread = new Thread(CollectPacks);
          //CollectingDataThread.Start();
        }
        else
        {
          //CollectingDataThread.Resume();
        }*/

        //PackDetector.PackArrived += AddPack; 
        LoopCtrl.OnPackFound += AddPack; //now from loop controller
      }
      else
      {
        DoStatCollection = false;
        CollectStatButton.Text = "Продолжить";
        //CollectingDataThread.Suspend();
        LoopCtrl.OnPackFound -= AddPack; //now from loop controller
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
            (float)StimStartPosition / 25,// ms
            0,
            (float)StimStartPosition / 25,// ms
            e.ClipRectangle.Height);
        }
      }
    }
    #endregion
    #region CallBack фукция для принятия информации об стмуляциях.
    public void RecieveStimData(List<TAbsStimIndex> stimlist)
    {
      lock (StimListBlock)
      {
        if (stimlist != null)
        {
          foreach (TAbsStimIndex stim in stimlist)
          {
            StimList.Add(stim);
          }
        }
      }
    }

    #endregion
    #region Функция для заргрузки информации об пачках, исполняется в отдельном потоке
    private void CollectPacks() //old, random
    {
      TTime InputCount = 0;
      Random rnd = new Random(DateTime.Now.Millisecond);
      while (true)
      {
        Thread.Sleep(10);
        while (DoStatCollection)
        {

          switch (CurrentState)
          {
            case state.BeforeStimulation:
              {
                Thread.Sleep(5);
                CPack pack_to_add = new CPack(InputCount * 43000 + (TTime)rnd.Next(12000), 0, null);
                lock (PacklListBlock)
                {
                  PackListBefore.Add(pack_to_add);
                }
              }
              StatProgressBar.BeginInvoke(SetVal, null, 1);
              DistribGrath.BeginInvoke(UpdateDistribGrath);
              InputCount++;
              if (PackListBefore.Count() >= Minimum_Pack_Requered_Count
                || InputCount >= Minimum_Pack_Requered_Count - 1)
              {
                CollectStatButton.BeginInvoke(SetCollectStatButtonText, "готово");
                this.DoStatCollection = false;
              }
              break;
            case state.AfterStimulation:
              {
                Thread.Sleep(150);//Вместо функции WaitPack()
                CPack pack_to_add = new CPack(InputCount * 40000 + (TTime)rnd.Next(16000), 0, null);
                InputCount++;
                lock (PacklListBlock)
                {
                  PackListAfter.Add(pack_to_add);
                }
                PackCountGraph.Invalidate();
              }
              break;
          }

        }
      }
    }
    private void AddPack(CPack pack_to_add)
    {
      lock (PacklListBlock)
      {
        switch (CurrentState)
        {
          case state.BeforeStimulation:
            if (DoStatCollection) PackListBefore.Add(pack_to_add);
            StatProgressBar.BeginInvoke(SetVal, null, 1);
            DistribGrath.BeginInvoke(UpdateDistribGrath);
            if (PackListBefore.Count >= Minimum_Pack_Requered_Count)
            {
              DoStatCollection = false;
              CollectStatButton.BeginInvoke(SetCollectStatButtonText, "готово");
            }
            break;
          case state.AfterStimulation:

            if (DoStimulation) PackListAfter.Add(pack_to_add);
            PackCountGraph.Invalidate();

            break;
        }
      }
    }

    #endregion
    #region Подсчет статистики
    private void CalcStat()
    {
      if (PackListBefore.Count - 1 > 0)
      {
        Average Stat = new Average();
        for (int i = 0; i < PackListBefore.Count() - 1; i++)
        {
          Stat.AddValueElem(PackListBefore[i + 1].Start - PackListBefore[i].Start);
        }
        Stat.Calc();
        this.SelectedAverageBox.Text = (Stat.Value / 25).ToString() + " мсек";
        this.SelectedSigmaBox.Text = (Stat.Sigma / 25).ToString() + " мсек";
        // Обновить график       
        this.DistribGrath.Refresh();
      }
    }
    #endregion
    #region Обработка события обновления прогресс бара
    void UpdateProgressBar(object sender, int val)
    {
      if (StatProgressBar.Value < StatProgressBar.Maximum - 1)
        StatProgressBar.Value += val;
    }
    #endregion
    #region Обработка скрола ползунка времени стимуляции
    private void StimPadding_TextChanged(object sender, EventArgs e)
    {
      DoDrawStartStimTime = true;
      double i = StimStartPosition;
      double.TryParse(StimPadding.Text, out i);
      //StimStartPosition = i;
      //trackBar1.Value = (int)StimStartPosition * 5 / 25;
      //DistribGrath.Refresh();
    }
    private void trackBar1_Scroll(object sender, EventArgs e)
    {
      DoDrawStartStimTime = true;
      //2000p - 20sec - maximum
      StimStartPosition = trackBar1.Value * 25 / 5;
      StimPadding.Text = StimStartPosition.ToString();
      DistribGrath.Refresh();
    }
    #endregion
    #region Кнопка отображения опций стимуляции
    private void button1_Click(object sender, EventArgs e)
    {
      StimParams paramswindow = new StimParams(WAIT_PACK_WINDOW_LENGTH);
      paramswindow.ShowDialog();
      if (paramswindow.DoUpdateParams)
      {
        WAIT_PACK_WINDOW_LENGTH = paramswindow.Time;
      }
    }
    #endregion
    private void StartStimButton_Click(object sender, EventArgs e)
    {
      CurrentState = state.AfterStimulation;
      DoStatCollection = true;
      DoStimulation = true;
      //TODO: вызов функции, начинающей стимуляции.
      PackCountGraph.Invalidate();
      PackCountGraph.Refresh();
    }


    private void CPackStat_Load(object sender, EventArgs e)
    {
      MinutesWindow.Value = Stat_Window_Minutes;
      SecondsWindow.Value = Stat_Window_Seconds;
    }

    void ChannelChangeRequest(int number)
    {
      this.numericUpDown1.Value = number;
    }

    private void numericUpDown1_ValueChanged(object sender, EventArgs e)
    {

    }
    #region Настройка временного окна для подсчета статистики
    private void MinutesWindow_ValueChanged(object sender, EventArgs e)
    {

    }

    private void SecondsWindow_ValueChanged(object sender, EventArgs e)
    {

    }
    #endregion
    private enum state
    {
      BeforeStimulation,
      AfterStimulation
    }
    #region Отрисовка графика вероятности появления пачек
    private void PackCountGraph_Paint(object sender, PaintEventArgs e)
    {
      //Главное условие отрисовки

      if (PackListAfter.Count > 0)
      {
        lock (PacklListBlock)
        {
          //Обновляем прошедшее время
          double CurrentTime = PackListAfter[PackListAfter.Count - 1].Start / 25000 - PackListAfter[0].Start / 25000; // текущее время в секундах
          HourCount.Text = (((TTime)CurrentTime) / 3600).ToString();
          MinuteCount.Text = ((((TTime)CurrentTime) % 3600) / 60).ToString();
          SecondCount.Text = ((((TTime)CurrentTime) % 3600) % 60).ToString();
          //составляем массив для гистаграммы
          TTime dT = (TTime)MinutesWindow.Value * 60 * 25000 + (TTime)SecondsWindow.Value * 25000;
          Point[] GistoGraphPoints = new Point[(int)(CurrentTime * 25000 / dT)];
          for (int j = 0; j < GistoGraphPoints.Count(); j++)
          {
            GistoGraphPoints[j].Y = e.ClipRectangle.Height - 10;
            GistoGraphPoints[j].X = j * e.ClipRectangle.Width / GistoGraphPoints.Count();
          }
          //ищем пачки, попадающие в окно после стимула
          for (int i = 0; i < PackListAfter.Count; i++)
          {
            bool IsInWindow = false;
            foreach (TStimIndex stim in StimList)
            {
              //<DEBUG>
              if (PackListAfter[i].Start - (TAbsStimIndex)stim < 800000)
              {
                int xxx = 0;
              }
              //</DEBUG>
              if (PackListAfter[i].Start - (TAbsStimIndex)stim > 0
                && PackListAfter[i].Start - (TAbsStimIndex)stim < (TAbsStimIndex)WAIT_PACK_WINDOW_LENGTH * 250
                ) // 
              {
                PackListDetectReaction.Add(PackListAfter[i]);
                break;
              }
              else
              {
                // добавить и стимул и пачку на удаление, как нас не интересующие
              }
            }
            if (IsInWindow)

              //GistoGraphPoints[j].Y -= 10;
              break;

          }
          // Заполним значения массива гистограммы соотвественно количеству стимулов в интервале окна подсчета

          for (int i = 1; i < GistoGraphPoints.Count(); i++)
          {
            for (int jj = 0; jj < PackListDetectReaction.Count; jj++)
            {
              if (dT * (ulong)i > PackListDetectReaction[jj].Start - PackListAfter[0].Start
                && dT * (ulong)(i - 1) < PackListDetectReaction[jj].Start - PackListAfter[0].Start)
              {
                GistoGraphPoints[i].Y /= 2;
              }
            }
          }
          //отрисовка массива
          Pen pen = new Pen(Color.Red);
          if (GistoGraphPoints.Count() >= 2)
          {
            e.Graphics.DrawLines(pen, GistoGraphPoints);
          }
        }
      }
    }
    #endregion


  }

}
