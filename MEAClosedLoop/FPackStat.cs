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
  public partial class FPackStat : Form
  {
    #region Стандартные значения
    int WAIT_PACK_WINDOW_LENGTH = 25; // 25 ms 
    private const int Minimum_Pack_Requered_Count = 140;
    #endregion
    #region Внутренние данные класса
    private CLoopController LoopCtrl;
    private CFiltering Filter;

    public List<CPack> PackListBefore; //befor stim started
    public List<CPack> PackListAfter; // After stim started
    public List<CPack> PackListDetectReaction = new List<CPack>(); // Лист пачек, которые считаются реакцией культуры
    public List<TAbsStimIndex> StimList;
    private Object StimListBlock = new Object();
    private Object PacklListBlock = new Object();
    private TTime StartStimulationTime = 0; // точка отсчета начала стимуляций. 
    private const int Stat_Window_Minutes = 0;
    private const int Stat_Window_Seconds = 10;
    private int AveragePackPeriod = 0;
    private double StimStartPosition;
    bool DoStatCollection = false;
    bool DoDrawStartStimTime = false;
    bool DoStimulation = false;
    List<int> m_channelList; //TODO: get channel list
    state CurrentState;
    private PackGraphForm formShowWindows;


    public delegate void DelegateUpdateDistribGrath();
    public delegate void DelegateSetCollectStatButtonText(string text);
    public delegate void StatPrepare(bool val);
    public event DelegateSetProgress SetVal;
    public event DelegateUpdateDistribGrath UpdateDistribGrath;
    public event DelegateSetCollectStatButtonText SetCollectStatButtonText;

    


    #endregion
    #region Конструктор
    public FPackStat(CLoopController _LoopController,CFiltering _Filter, List<int> channelList)
    {
      InitializeComponent();
      CurrentState = state.BeforeStimulation;
      PackListBefore = new List<CPack>();
      PackListAfter = new List<CPack>();
      StimList = new List<TAbsStimIndex>();
      LoopCtrl = _LoopController;
      Filter = _Filter;
      m_channelList = channelList;

      UpdateDistribGrath += DistribGrath.Refresh;
      UpdateDistribGrath += PreCalcStat;
      UpdateDistribGrath += CurrentCalcStat;
      SetCollectStatButtonText += SetStatButtontext;
      numericUpDown1.ValueChanged += GraphChannelSelected;

      StimType.Enabled = false;
      StartStimButton.Enabled = false;
      MinutesWindow.Enabled = false;
      SecondsWindow.Enabled = false;

      MinutesWindow.ValueChanged += StimReady;
      SecondsWindow.ValueChanged += StimReady;
      MinutesWindow.EnabledChanged += SetDefaultMinWindow;
      SecondsWindow.EnabledChanged += SetDefaultSecWindow;

      formShowWindows = new PackGraphForm(m_channelList, LoopCtrl);
      formShowWindows.loadSelection += ChannelChangeRequest;
    }
    #endregion

    #region Клик по кнопке выбора канала
    private void GraphChannelSelectButton_Click(object sender, EventArgs e)
    {
      formShowWindows.Show();
      formShowWindows.VisibleChanged += UnlockCompareButton;
    }

    private void UnlockCompareButton(Object sender, EventArgs e)
    {
      compareButton.Enabled = true;
    }

    private void GraphChannelSelected(Object sender, EventArgs e)
    {
      this.MinutesWindow.Enabled = true;
      this.SecondsWindow.Enabled = true;
    }
    #endregion

    #region Клик по кнопке сбора статистики
    private void CollectStatButton_Click(object sender, EventArgs e)
    {

      if (!DoStatCollection)
      {
        DoStatCollection = true;
        CollectStatButton.Text = "Остановить"; 
        LoopCtrl.OnPackFound += AddPack; //now from loop controller
      }
      else
      {
        DoStatCollection = false;
        CollectStatButton.Text = "Продолжить";
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
      float dt = (float)0.05;//sec, is optimal time window to collect packs for gistgrath// derived from Ilia Sokolov experiment's
      float HorisondalProportion = ((float)e.ClipRectangle.Width) / ((int)StatGraphXRange.Value); // from s to px
      float dp = dt * 25000; // dt in points
      //summ time Length = dt*n sec, n = TimeLength/dt
      Pen pen = new Pen(Color.Blue);
      pen.Width = 1;
      int maxInd = 0;
      int[] PackLengthDestrib = new int[(int)((int)(StatGraphXRange.Value) / dt)];
      //set as 0
      for (int i = 0; i < PackLengthDestrib.Length; i++)
      {
        PackLengthDestrib[i] = 0;
      }
      lock (PacklListBlock)
      {
        for (int i = 0; i < PackListBefore.Count() - 1; i++)
        {
          for (int j = 0; j < PackLengthDestrib.Length; j++)
          {
            if (PackListBefore[i + 1].Start - PackListBefore[i].Start > (TAbsStimIndex)j * dp
              && PackListBefore[i + 1].Start - PackListBefore[i].Start < (TAbsStimIndex)(j + 1) * dp)
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
            (float)i * dt * HorisondalProportion,
            (float)(e.ClipRectangle.Height - PackLengthDestrib[i] * e.ClipRectangle.Height / ((float)StatGraphYRange.Value)),
            (float)(i + 1) * dt * HorisondalProportion + dt,
            (float)(e.ClipRectangle.Height - PackLengthDestrib[i] * e.ClipRectangle.Height / ((float)StatGraphYRange.Value)));
          if (PackLengthDestrib[i] > PackLengthDestrib[maxInd]) maxInd = i;
        }
        //Draw Average
        pen.Color = Color.Red;
        e.Graphics.DrawLine(pen, (float)AveragePackPeriod * HorisondalProportion / 1000, (float)0, (float)AveragePackPeriod * HorisondalProportion / 1000, (float)e.ClipRectangle.Height);
        //Draw Start Stim Time
        pen.Color = Color.Green;
        if (DoDrawStartStimTime)
        {
          e.Graphics.DrawLine(pen,
            (float)StimStartPosition * HorisondalProportion,
            0,
            (float)StimStartPosition * HorisondalProportion,
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
    #region CallBack Функция для загрузки информации об пачках
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
      //light pack - Pack version that not include PackDataArray - memory optimization
      CPack lightPack = new CPack(pack_to_add.Start, pack_to_add.Length, null);
      lock (PacklListBlock)
      {
        switch (CurrentState)
        {
          case state.BeforeStimulation:
            //light pack - Pack version that not include PackDataArray - memory optimization
            
            if (DoStatCollection) PackListBefore.Add(lightPack);
            if (!this.IsDisposed) DistribGrath.BeginInvoke(UpdateDistribGrath);
            if (PackListBefore.Count >= Minimum_Pack_Requered_Count)
            {
              DoStatCollection = false;
              CollectStatButton.BeginInvoke(SetCollectStatButtonText, "готово");
            }
            break;
          case state.AfterStimulation:

            if (DoStimulation) PackListAfter.Add(lightPack);
            PackCountGraph.Invalidate();
            DistribGrath.BeginInvoke(UpdateDistribGrath);
            break;
        }
      }
    }

    #endregion
    #region Подсчет статистики
    private void PreCalcStat()
    {
      if (PackListBefore.Count > 1)
      {
        lock (PacklListBlock)
        {
          Average Stat = new Average();
          for (int i = 0; i < PackListBefore.Count() - 1; i++)
          {
            Stat.AddValueElem(PackListBefore[i + 1].Start - PackListBefore[i].Start);
          }
          Stat.Calc();
          this.AveragePackPeriod = (int)Stat.Value / 25; //милли секунд
          this.SelectedAverageBox.Text = String.Format("{0:0.###}", (Stat.Value / 25));
          this.SelectedSigmaBox.Text = String.Format("{0:0.###}", (Stat.Sigma / 25));
          // Обновить график       
          this.DistribGrath.Refresh();
        }
      }

    }
    private void CurrentCalcStat()
    {

      if(PackListBefore.Count < 2 && PackListAfter.Count < 2) return;

      TAbsStimIndex CurrentTime = (PackListAfter.Count >= 2) ? PackListAfter.Last().Start : PackListBefore.Last().Start;
      TAbsStimIndex TimeDelay = (TAbsStimIndex)StatWindowMinCount.Value * 60 * 25000 + (TAbsStimIndex)StatWindowSecCount.Value * 25000;
      Average Stat = new Average();
      

      lock (PacklListBlock)
      {
        if (PackListBefore.Count > 1)
        {
          for (int i = 0; i < PackListBefore.Count() - 1; i++)
          {
            if (PackListBefore[i].Start + TimeDelay > CurrentTime)
              Stat.AddValueElem(PackListBefore[i + 1].Start - PackListBefore[i].Start);
          }
        }
        if (PackListAfter.Count > 1)
        {
          for (int i = 0; i < PackListAfter.Count() - 1; i++)
          {
            if (PackListAfter[i].Start + TimeDelay > CurrentTime)
              Stat.AddValueElem(PackListAfter[i + 1].Start - PackListAfter[i].Start);
          }
        }
      }
      Stat.Calc();
      this.CurrentAverage.Text = String.Format("{0:0.###}", (Stat.Value / 25));
      this.CurrentSigma.Text = String.Format("{0:0.###}", (Stat.Sigma / 25));
    }

    #endregion
   
    #region Обработка скрола ползунка времени стимуляции
    private void StimPadding_TextChanged(object sender, EventArgs e)
    {
      DoDrawStartStimTime = true;
      double i = StimStartPosition;
      double.TryParse(StimPadding.Text, out i);
      StimStartPosition = i;
      int newTrackBarPosition = (int) (StimStartPosition * trackBar1.Maximum / (int)(StatGraphXRange.Value));
      if (newTrackBarPosition > trackBar1.Maximum)
      {
        trackBar1.Value = trackBar1.Maximum;
      }
      else
      {
        trackBar1.Value = (newTrackBarPosition < trackBar1.Minimum) ? trackBar1.Minimum : newTrackBarPosition;
      }
      LoopCtrl.ReceivedStimShift = (int)(1000 * StimStartPosition * Param.MS);

      //DistribGrath.Refresh();
    }
    private void trackBar1_Scroll(object sender, EventArgs e)
    {
      DoDrawStartStimTime = true;
      //2000p - 20sec - maximum
      StimStartPosition = ((float)((int)StatGraphXRange.Value) * trackBar1.Value) / trackBar1.Maximum; // in Seconds
      StimPadding.Text = String.Format("{0:0.###}", StimStartPosition);
      DistribGrath.Refresh();
    }
    #endregion
    #region Кнопка отображения опций стимуляции
    private void button1_Click(object sender, EventArgs e)
    {
      FStimParams paramswindow = new FStimParams(WAIT_PACK_WINDOW_LENGTH);
      paramswindow.ShowDialog();
      if (paramswindow.DoUpdateParams)
      {
        WAIT_PACK_WINDOW_LENGTH = paramswindow.Time;
      }
    }
    #endregion

    private void StimReady(Object sender, EventArgs e)
    {
      StimType.Enabled = true;
      StartStimButton.Enabled = true;
    }

    private void StartStimButton_Click(object sender, EventArgs e)
    {
      CurrentState = state.AfterStimulation;
      DoStatCollection = true;
      DoStimulation = true;
      LoopCtrl.ReceivedStimShift = (int)(1000 * StimStartPosition * Param.MS);
      //TODO: вызов функции, начинающей стимуляции.
      DrawPackCountGraph((int)this.numericUpDown1.Value);
      PackCountGraph.Invalidate();
      PackCountGraph.Refresh();
    }

    private void SetDefaultMinWindow(object sender, EventArgs e)
    {
      MinutesWindow.Value = Stat_Window_Minutes;
    }

    private void SetDefaultSecWindow(object sender, EventArgs e)
    {
      SecondsWindow.Value = Stat_Window_Seconds;
    }

    private void DrawPackCountGraph(int channel)
    {
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
            GistoGraphPoints[j].Y = e.ClipRectangle.Height;
            GistoGraphPoints[j].X = j * e.ClipRectangle.Width / GistoGraphPoints.Count();
          }
          //ищем пачки, попадающие в окно после стимула
          for (int i = 0; i < PackListAfter.Count; i++)
          {
            lock (StimListBlock)
            {
              foreach (TAbsStimIndex stim in StimList)
              {
                if (PackListAfter[i].Start > stim
                  && PackListAfter[i].Start < stim + (TAbsStimIndex)WAIT_PACK_WINDOW_LENGTH * 25
                  && !PackListDetectReaction.Contains(PackListAfter[i])
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
            }
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

    private void StatGraphXRange_ValueChanged(object sender, EventArgs e)
    {
      DistribGrath.Invalidate();
    }

    private void StatGraphYRange_ValueChanged(object sender, EventArgs e)
    {
      DistribGrath.Invalidate();
    }

    private void PackCountGraph_Click(object sender, EventArgs e)
    {
      PackCountGraph.Invalidate();
    }

    private void DistribGrath_Click(object sender, EventArgs e)
    {

    }

    private void StatWindowCount_ValueChanged(object sender, EventArgs e)
    {
      this.CurrentCalcStat();
    }

    private void compareButton_Click(object sender, EventArgs e)
    {
      this.GraphChannelSelectButton.Enabled = false;
      this.formShowWindows.EnableCompareMode();
      formShowWindows.Show();
    }

    private void CPackStat_Load(object sender, EventArgs e)
    {

    }

    private void RunNewMSHLearnCycle_Click(object sender, EventArgs e)
    {
      FLearnCycle LearnCycle = new FLearnCycle(LoopCtrl, Filter);
      LearnCycle.MdiParent = this.MdiParent;
      LearnCycle.Show();
    }
  }
}
