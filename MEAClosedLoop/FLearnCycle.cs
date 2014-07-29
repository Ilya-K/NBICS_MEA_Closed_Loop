using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
  public partial class FLearnCycle : Form
  {
    #region Внутренние константы

    private TTime StimControlDuration = Param.MS * 25; //Время до которого после стимула пачка считается вызванной 
    private TTime StimActualityDuration = Param.MS * 100; // Время, на протяжении которого для стимула ищется пачка
    private int SigmaCount = 14; // во сколько раз значение сигнала должно превышать сигму шума, что бы считаться  спайком
    public int ChannelIdx = 1;
    #endregion

    #region Внутренние данные
    private CFiltering Filter;
    private CLoopController loopController;
    //private Form1 MainForm;
    //очередь пачек (содержит RS пачки из последних N вызванных)
    private Queue<EvokedPackInfo> PackQueue = new Queue<EvokedPackInfo>();
    //очередь стимулов (содержит не устаревшие стимулы)
    private Queue<TTime> StimQueue = new Queue<TTime>();
    //очередь вызванных пачек
    private Queue<EvokedPackInfo> EvokedPacksQueue = new Queue<EvokedPackInfo>();


    private Object StimQueueLock = new Object();
    private Object PackQueueLock = new Object();
    private Object EvokedPacksLock = new Object();

    private TTime CurrentTime = 0;
    private TTime StartTime = 0;

    private ShahafCycleState CycleState = ShahafCycleState.NotStarted;
    private List<ShahafCycleIteration> CycleInfo = new List<ShahafCycleIteration>();
    private ShahafCycleIteration currentIteration = new ShahafCycleIteration();

    #endregion

    public FLearnCycle(CLoopController _LoopController, CFiltering _Filter)
    {
      InitializeComponent();

      Filter = _Filter;
      loopController = _LoopController;
      this.PRSCount.Value = 2;
      this.PSelectName.Value = 54;

    }

    private void FLearnCycle_Load(object sender, EventArgs e)
    {
      this.evBurstPanel.Controls.Clear();
      int BoxHeight = evBurstPanel.Height / 10;
      for (int i = 0; i < 10; i++)
      {
        PictureBox SomePack = new PictureBox();
        SomePack.Location = new Point(0, BoxHeight * i);
        SomePack.Anchor = AnchorStyles.Right | AnchorStyles.Left;
        SomePack.BackColor = Color.White;
        SomePack.Size = new Size(evBurstPanel.Width, BoxHeight);
        SomePack.Paint += evPack_Paint;
        this.evBurstPanel.Controls.Add(SomePack);
        SomePack.Refresh();
      }
    }

    private void pictureBox1_Paint(object sender, PaintEventArgs e)
    {
      int padding_left = 5;
      int padding_bottom = 5;
      int padding_top = 0;
      int padding_right = 0;
      SolidBrush AxisBrush = new SolidBrush(Color.Black);
      Pen AxisPen = new Pen(AxisBrush, 1);
      // Отрисовка осей
      e.Graphics.DrawLine(AxisPen,
        new Point(0, e.ClipRectangle.Height - padding_bottom),
        new Point(e.ClipRectangle.Width - padding_right, e.ClipRectangle.Height - padding_bottom));
      e.Graphics.DrawLine(AxisPen,
        new Point(padding_left, padding_top),
        new Point(padding_left, e.ClipRectangle.Height));
      // Отрисовка графика
      if (CycleInfo.Count == 0) return;
      float XProportional = (e.ClipRectangle.Width - padding_bottom) / CycleInfo.Count;
      float YProportional = (e.ClipRectangle.Height - padding_left) / (float)this.PStimLength.Value;
      for (int i = 0; i < CycleInfo.Count - 1; i++)
      {
        e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Black)),
          (float)(padding_left + i * XProportional),
          (float)(e.ClipRectangle.Height - padding_bottom - CycleInfo[i].ElapsedStimTime / 25000 * YProportional),
          (float)(padding_left + (i + 1) * XProportional),
          (float)(e.ClipRectangle.Height - padding_bottom - CycleInfo[i + 1].ElapsedStimTime / 25000 * YProportional)
          );
      }
      for (int i = 0; i < CycleInfo.Count - 1; i++)
      {
        e.Graphics.DrawEllipse(new Pen(new SolidBrush(Color.Red)),
          (float)(padding_left + i * XProportional - 2),
          (float)(e.ClipRectangle.Height - padding_bottom - CycleInfo[i].ElapsedStimTime / 25000 * YProportional - 2),
          (float)4,
          (float)4
          );
      }
    }

    public void RecieveStimData(List<TAbsStimIndex> stimlist)
    {
      if (CycleState != ShahafCycleState.RunningStim) return;
      lock (StimQueueLock)
      {
        foreach (TTime stim in stimlist)
        {
          StimQueue.Enqueue(stim);
        }
        for (; StimQueue.ElementAt(0) + StimActualityDuration < CurrentTime; StimQueue.Dequeue()) ;
      }

    }

    public void RecievePackData(CPack pack)
    {

      if (TrainEvolutionGraph.InvokeRequired)
        TrainEvolutionGraph.BeginInvoke(new Action<Control>(c => c.Refresh()), TrainEvolutionGraph);
      else
        TrainEvolutionGraph.Refresh();

      if (CycleState != ShahafCycleState.RunningStim) return;
      lock (PackQueueLock)
      {
        // Нам подходят только вызванные пачки
        // Совместим пачки со стимулами
        lock (StimQueueLock)
        {
          lock (PackQueueLock)
          {
            foreach (TTime stim in StimQueue)
            {
              // стимул должен находится внутри пачки или быть раньшее её не более чем на StimControlDuration.
              if (stim + StimControlDuration > pack.Start && stim < pack.Start + (TTime)pack.Length)
              {
                EvokedPackInfo evokedPackInfo = new EvokedPackInfo();
                evokedPackInfo.Pack = pack;
                evokedPackInfo.AbsStim = stim;
                EvokedPacksQueue.Enqueue(evokedPackInfo);

              }
            }
          }
        }
        lock (PackQueueLock)
        {
          PackQueue.Clear();
          foreach (EvokedPackInfo ev_pack in EvokedPacksQueue)
          {
            if (ChekSpike(ev_pack.Pack, ev_pack.AbsStim, (TAbsStimIndex)this.PDelayTime.Value * Param.MS, (TAbsStimIndex)this.PSearchDelta.Value * Param.MS))
              PackQueue.Enqueue(ev_pack);
          }

          //10 - максимальное число S в отношении R/S

          // Очистка переполненных очередей
          for (; PackQueue.Count > this.PRSCount.Value; PackQueue.Dequeue()) ;
          for (; EvokedPacksQueue.Count > 10; EvokedPacksQueue.Dequeue()) ;
        }
      }
      foreach (Control picturebox in RSPacks.Controls)
      {
        if (picturebox.InvokeRequired)
          picturebox.BeginInvoke(new Action<Control>(c => c.Refresh()), picturebox);
        else
          picturebox.Refresh();
      }
      foreach (Control picturebox in evBurstPanel.Controls)
      {
        if (picturebox.InvokeRequired)
          picturebox.BeginInvoke(new Action<Control>(c => c.Refresh()), picturebox);
        else
          picturebox.Refresh();
      }
    }

    public void UpdateTime(TFltDataPacket data) //Recieve Flt Data
    {
      //Update Time Count
      CurrentTime = Filter.TimeStamp - StartTime;

      if (TimeStamp.InvokeRequired)
        TimeStamp.BeginInvoke(new Action<string>(s => TimeStamp.Text = s), (((double)CurrentTime) / 25000).ToString());
      else
        TimeStamp.Text = (((double)CurrentTime) / 25000).ToString();
      //Do Shahaf Cycle 
      if (currentIteration == null) return;

      switch (CycleState)
      {
        case ShahafCycleState.NotStarted:
          //ничего не делаем, цикл еще не начался
          break;
        case ShahafCycleState.Finished:
          //ничего не делаем, цикл уже завершен
          break;
        case ShahafCycleState.RunningStim:
          //добавляем врмя стимуляции
          currentIteration.ElapsedStimTime = (int)(CurrentTime - currentIteration.StartTime);
          //Если время стимуляции пройденно полностью или выполнился R/S > xxx, 
          //завершаем стимуляцию и переходим к отдыху 
          //[TODO]: вычислить R/S, а пока false
          lock (PackQueueLock)
            if (currentIteration.ElapsedStimTime >= (int)(this.PStimLength.Value) * Param.MS * 1000 || PackQueue.Count() >= this.PRSCount.Value)
            {
              loopController.DoStim = false;
              CycleState = ShahafCycleState.CoolDown;
              currentIteration.StartCoolDown = CurrentTime;
              if (PackQueue.Count() >= this.PRSCount.Value)
              {
                LernLogTextBox.BeginInvoke(new Action<string>(s => LernLogTextBox.AppendText(s)),
                  Environment.NewLine + "[" + (CurrentTime / 25000).ToString() + "] Выполнен RS критерий, переход в отдых культуры");
              }
              else
              {
                LernLogTextBox.BeginInvoke(new Action<string>(s => LernLogTextBox.AppendText(s)),
                  Environment.NewLine + "[" + (CurrentTime / 25000).ToString() + "] Превышено время ожидания RS критерия, переход в отдых культуры");
              }
              PackQueue.Clear();
              EvokedPacksQueue.Clear();
              TrainEvolutionGraph.BeginInvoke(new Action(() => TrainEvolutionGraph.Refresh()));
              /*
              loopController.OnPackFound -= RecievePackData;
              Filter.RemoveStimulConsumer(RecieveStimData);
              Filter.RemoveDataConsumer(UpdateTime);
              
               */
              return;
            }

          break;
        case ShahafCycleState.CoolDown:
          //добавляем время отдыха
          currentIteration.ElapsedCoolDownTime = (int)(CurrentTime - currentIteration.StartCoolDown);
          //если отдых пройден полностью - завершаем итерацию.
          // + Запускаем следующую итерацию цикла
          if (currentIteration.ElapsedCoolDownTime >= (int)(this.PCoolDownLength.Value) * Param.MS * 1000)
          {

            currentIteration = new ShahafCycleIteration();
            /*
            loopController.OnPackFound += RecievePackData;
            Filter.AddStimulConsumer(RecieveStimData);
            Filter.AddDataConsumer(UpdateTime);
            */
            LernLogTextBox.BeginInvoke(new Action<string>(s => LernLogTextBox.Text += s),
                 Environment.NewLine + "[" + (CurrentTime / 25000).ToString() + "] Выполнен отдых культуры, итерация завершена");
            RunNewCycleIteration();
            return;
          }
          break;
      }
      // Если цикл длится слишком долго.
      if (CurrentTime - StartTime > this.PExpMaxLength.Value * Param.MS * 60 * 1000 &&
         (CycleState == ShahafCycleState.RunningStim || CycleState == ShahafCycleState.CoolDown))
      {
        LernLogTextBox.BeginInvoke(new Action<string>(s => LernLogTextBox.AppendText(s)),
                 Environment.NewLine + "[" + (CurrentTime / 25000).ToString() + "] Превышено максимальное время эксперимента, эксперимент завершен");
        CycleState = ShahafCycleState.Finished;

      }
    }

    private void RunNewCycleIteration()
    {
      currentIteration = new ShahafCycleIteration();
      currentIteration.StartTime = CurrentTime;
      currentIteration.ElapsedStimTime = 0;
      currentIteration.ElapsedCoolDownTime = 0;
      currentIteration.StartCoolDown = 0;

      CycleInfo.Add(currentIteration);

      CycleState = ShahafCycleState.RunningStim;

      loopController.DoStim = true;
      LernLogTextBox.BeginInvoke(new Action<string>(s => LernLogTextBox.AppendText(s)),
                Environment.NewLine + "[" + (CurrentTime / 25000).ToString() + "] Начало новой итерации цикла");
    }

    private bool ChekSpike(CPack pack, TTime StimTime, TTime CenterTime, TTime Delta)
    {
      //Pack Start Time && Stim Time may be absolute but Center && Delta mast be relative (all in Points), NOT MS!

      //Пачка началась сильно позже стимула? т.е. не считалась вызванной
      if (pack.Start > StimTime + StimControlDuration)
      {
        MessageBox.Show("Ошибка в паре пачка - стимул");
        return false;
      }

      //Пачка закончилась раньше начала стимула
      if (pack.Start + (TTime)pack.Length < StimTime)
      {
        MessageBox.Show("Ошибка в паре пачка - стимул");
        return false;
      }
      TTime StartSearchTime = (pack.Start + Param.PRE_SPIKE <= StimTime)
        ? StimTime - (pack.Start + Param.PRE_SPIKE) + CenterTime - Delta
        : (pack.Start + Param.PRE_SPIKE) - StimTime + CenterTime - Delta;
      Average average = new Average();
      for (int i = 0; i < pack.NoiseLevel.Length; i++)
      {
        average.AddValueElem(Math.Abs(pack.NoiseLevel[i]));
      }
      average.Calc();
      for (TTime i = StartSearchTime; i < StartSearchTime + 2 * Delta && i < (TTime)pack.Length - 1; i++)
      {
        if (Math.Abs(pack.Data[(int)this.PSelectIndex.Value][i]) > (average.Sigma * SigmaCount))
        {
          return true;
        }
      }

      return false;
    }

    private void StartCycle_Click(object sender, EventArgs e)
    {
      loopController.OnPackFound += RecievePackData;
      Filter.AddStimulConsumer(RecieveStimData);
      Filter.AddDataConsumer(UpdateTime);

      foreach (Control control in ParamBox.Controls)
      {
        control.Enabled = false;
      }
      StartCycle.Enabled = false;
      CurrentTime = Filter.TimeStamp - StartTime;
      RunNewCycleIteration();
    }

    private void FinishCycle_Click(object sender, EventArgs e)
    {
      loopController.OnPackFound -= RecievePackData;
      Filter.RemoveStimulConsumer(RecieveStimData);
      Filter.RemoveDataConsumer(UpdateTime);
      foreach (Control control in ParamBox.Controls)
      {
        control.Enabled = true;
      }
      ParamBox.Enabled = false;
    }

    private void SelectName_ValueChanged(object sender, EventArgs e)
    {
      if (!MEA.IDX2NAME.Contains((int)PSelectName.Value))
      {
        MessageBox.Show("Канал не существует");
        return;
      }
      else
      {
        PSelectIndex.Value = MEA.NAME2IDX[(int)PSelectName.Value];
      }
    }

    private void SelectIndex_ValueChanged(object sender, EventArgs e)
    {
      if (!MEA.NAME2IDX.Contains((int)PSelectIndex.Value))
      {
        MessageBox.Show("Канал не существует");
        return;
      }
      else
      {
        PSelectName.Value = MEA.IDX2NAME[(int)PSelectIndex.Value];
      }

    }

    private void PRSCount_ValueChanged(object sender, EventArgs e)
    {
      this.RSPacks.Controls.Clear();
      for (int i = 0; i < PRSCount.Value; i++)
      {
        PictureBox SomePack = new PictureBox();
        SomePack.Location = new Point(0, 20 + 158 * i);
        SomePack.Anchor = AnchorStyles.Right | AnchorStyles.Left;
        SomePack.BackColor = Color.White;
        SomePack.Size = new Size(RSPacks.Width, 150);
        SomePack.Paint += SomePack_Paint;
        this.RSPacks.Controls.Add(SomePack);
        SomePack.Refresh();
      }
    }

    void SomePack_Paint(object sender, PaintEventArgs e)
    {
      int i;
      int HorisontalProportional = 10;
      for (i = 0; i < RSPacks.Controls.Count; i++)
      {
        if (RSPacks.Controls[i].Equals(sender))
        {
          break;
        }
      }
      lock (PackQueueLock)
      {
        if (PackQueue.Count > i)
        {
          SolidBrush packBrush = new SolidBrush(Color.DarkGray);
          SolidBrush BurstSpikeBrush = new SolidBrush(Color.DarkViolet);
          SolidBrush stimBrush = new SolidBrush(Color.DarkBlue);
          SolidBrush DeltaBrush = new SolidBrush(Color.Red);
          if (i >= EvokedPacksQueue.Count) return;
          TFltDataPacket PackData = PackQueue.ElementAt(i).Pack.Data;
          CPack Pack = PackQueue.ElementAt(i).Pack;
          TTime StimTime = PackQueue.ElementAt(i).AbsStim;
          // Пусть все окно - Время поиска + дельта + 10 мс
          int WindowTimelength = (int)this.PDelayTime.Value * Param.MS + (int)this.PSearchDelta.Value * Param.MS + 10 * Param.MS;

          // Отрисовываем пачку от момента начала стимула
          //Сдвиг начала отрисовки пачки (если стимул произошел раньше)
          int PackShift = (StimTime < Pack.Start) ? (int)(Pack.Start - StimTime) : 0;

          //Смещение пачки влево (если стимул внутри пачки)
          int StimShift = (StimTime > Pack.Start) ? (int)(StimTime - Pack.Start) : 0;

          //переводит Время(отсчет) в позицию на экране
          float k = (float)e.ClipRectangle.Width / (float)WindowTimelength;

          //DEBUG
          Average average = new Average();
          for (int idx = 0; idx < Pack.NoiseLevel.Length; idx++)
          {
            average.AddValueElem((Pack.NoiseLevel[idx]));
          }
          average.Calc();
          //отрисовка пачки
          //[TODO]: Сделать оптимизацию (отрисовывать только входяющую в окно часть пачки)
          for (int idx = 0; idx < PackData[(int)this.PSelectIndex.Value].Length - 1 /*&& idx < 110 * Param.MS*/; idx++)
          {
            SolidBrush current_brush = (Math.Abs(PackData[(int)this.PSelectIndex.Value][idx]) < average.Sigma * SigmaCount) ? packBrush : BurstSpikeBrush;
            e.Graphics.DrawLine(new Pen(current_brush),
              (float)((idx - StimShift + PackShift - Param.PRE_SPIKE) * k),
              (float)PackData[(int)this.PSelectIndex.Value][idx] / HorisontalProportional + e.ClipRectangle.Height / 2,
              (float)((idx - StimShift + PackShift - Param.PRE_SPIKE) * k),
              (float)PackData[(int)this.PSelectIndex.Value][idx + 1] / HorisontalProportional + e.ClipRectangle.Height / 2 + 1 // + 1 - фикс для отрисовки линии, равной нулю.
              );
          }
          //отрисовка стимула
          e.Graphics.DrawLine(new Pen(stimBrush),
            new Point((int)(0 * k), 0),
            new Point((int)(0 * k), e.ClipRectangle.Height));

          // отрисовка области поиска спайка
          e.Graphics.DrawLine(new Pen(DeltaBrush),
            new Point((int)((float)(this.PDelayTime.Value - this.PSearchDelta.Value) * Param.MS * k), 0),
            new Point((int)((float)(this.PDelayTime.Value - this.PSearchDelta.Value) * Param.MS * k), e.ClipRectangle.Height));
          e.Graphics.DrawLine(new Pen(DeltaBrush),
            new Point((int)((float)(this.PDelayTime.Value + this.PSearchDelta.Value) * Param.MS * k), 0),
            new Point((int)((float)(this.PDelayTime.Value + this.PSearchDelta.Value) * Param.MS * k), e.ClipRectangle.Height));
          //отрисовка уровня ограничния шума
          e.Graphics.DrawLine(new Pen(BurstSpikeBrush),
            (float)0,
            (float)e.ClipRectangle.Height / 2 - (float)average.Sigma * SigmaCount / HorisontalProportional,
            (float)e.ClipRectangle.Width,
            (float)e.ClipRectangle.Height / 2 - (float)average.Sigma * SigmaCount / HorisontalProportional);
          e.Graphics.DrawLine(new Pen(BurstSpikeBrush),
            (float)0,
            (float)e.ClipRectangle.Height / 2 + (float)average.Sigma * SigmaCount / HorisontalProportional,
            (float)e.ClipRectangle.Width,
            (float)e.ClipRectangle.Height / 2 + (float)average.Sigma * SigmaCount / HorisontalProportional);
          // отрисовка надписей
          e.Graphics.DrawString("noise",
            SystemFonts.MessageBoxFont, BurstSpikeBrush,
            new PointF(0,
              (float)e.ClipRectangle.Height / 2 + (float)average.Sigma * SigmaCount / HorisontalProportional));
          e.Graphics.DrawString(((Pack.Start - currentIteration.StartTime) / 25000).ToString() + " sec",
            SystemFonts.MessageBoxFont, BurstSpikeBrush,
            new PointF(0, 0));
        }

      }
    }

    void evPack_Paint(object sender, PaintEventArgs e)
    {
      int i;
      float HorisontalProportional = 15;
      for (i = 0; i < evBurstPanel.Controls.Count; i++)
      {
        if (evBurstPanel.Controls[i].Equals(sender))
        {
          break;
        }
      }
      lock (PackQueueLock)
      {
        if (evBurstPanel.Controls.Count > i)
        {
          SolidBrush packBrush = new SolidBrush(Color.DarkGray);
          SolidBrush BurstSpikeBrush = new SolidBrush(Color.DarkViolet);
          SolidBrush stimBrush = new SolidBrush(Color.DarkBlue);
          SolidBrush DeltaBrush = new SolidBrush(Color.Red);
          if (i >= EvokedPacksQueue.Count) return;
          TFltDataPacket PackData = EvokedPacksQueue.ElementAt(i).Pack.Data;
          CPack Pack = EvokedPacksQueue.ElementAt(i).Pack;
          TTime StimTime = EvokedPacksQueue.ElementAt(i).AbsStim;
          // Пусть все окно - Время поиска + дельта + 10 мс
          int WindowTimelength = (int)this.PDelayTime.Value * Param.MS + (int)this.PSearchDelta.Value * Param.MS + 10 * Param.MS;

          // Отрисовываем пачку от момента начала стимула
          //Сдвиг начала отрисовки пачки (если стимул произошел раньше)
          int PackShift = (StimTime < Pack.Start) ? (int)(Pack.Start - StimTime) : 0;

          //Смещение пачки влево (если стимул внутри пачки)
          int StimShift = (StimTime > Pack.Start) ? (int)(StimTime - Pack.Start) : 0;

          //переводит Время(отсчет) в позицию на экране
          float k = (float)e.ClipRectangle.Width / (float)WindowTimelength;

          //DEBUG
          Average average = new Average();
          for (int idx = 0; idx < Pack.NoiseLevel.Length; idx++)
          {
            average.AddValueElem(Pack.NoiseLevel[idx]);
          }
          average.Calc();
          //отрисовка пачки
          //[TODO]: Сделать оптимизацию (отрисовывать только входяющую в окно часть пачки)
          for (int idx = 0; idx < PackData[(int)this.PSelectIndex.Value].Length - 1 /*&& idx < 110 * Param.MS*/; idx++)
          {
            SolidBrush current_brush = (Math.Abs(PackData[(int)this.PSelectIndex.Value][idx]) < average.Sigma * SigmaCount) ? packBrush : BurstSpikeBrush;
            e.Graphics.DrawLine(new Pen(current_brush),
              (float)((idx - StimShift + PackShift - Param.PRE_SPIKE) * k),
              (float)PackData[(int)this.PSelectIndex.Value][idx] / HorisontalProportional + e.ClipRectangle.Height / 2,
              (float)((idx - StimShift + PackShift - Param.PRE_SPIKE) * k),
              (float)PackData[(int)this.PSelectIndex.Value][idx + 1] / HorisontalProportional + e.ClipRectangle.Height / 2 + 1 // + 1 - фикс для отрисовки линии, равной нулю.
              );
          }

          // отрисовка области поиска спайка
          e.Graphics.DrawLine(new Pen(DeltaBrush),
            new Point((int)((float)(this.PDelayTime.Value - this.PSearchDelta.Value) * Param.MS * k), 0),
            new Point((int)((float)(this.PDelayTime.Value - this.PSearchDelta.Value) * Param.MS * k), e.ClipRectangle.Height));
          e.Graphics.DrawLine(new Pen(DeltaBrush),
            new Point((int)((float)(this.PDelayTime.Value + this.PSearchDelta.Value) * Param.MS * k), 0),
            new Point((int)((float)(this.PDelayTime.Value + this.PSearchDelta.Value) * Param.MS * k), e.ClipRectangle.Height));

          //отрисовка уровня ограничния шума
          e.Graphics.DrawLine(new Pen(BurstSpikeBrush),
            (float)0,
            (float)e.ClipRectangle.Height / 2 - (float)average.Sigma * SigmaCount / HorisontalProportional,
            (float)e.ClipRectangle.Width,
            (float)e.ClipRectangle.Height / 2 - (float)average.Sigma * SigmaCount / HorisontalProportional);
          e.Graphics.DrawLine(new Pen(BurstSpikeBrush),
            (float)0,
            (float)e.ClipRectangle.Height / 2 + (float)average.Sigma * SigmaCount / HorisontalProportional,
            (float)e.ClipRectangle.Width,
            (float)e.ClipRectangle.Height / 2 + (float)average.Sigma * SigmaCount / HorisontalProportional);

          // отрисовка надписей
          e.Graphics.DrawString(((Pack.Start - currentIteration.StartTime) / 25000).ToString() + " sec",
            SystemFonts.MessageBoxFont, BurstSpikeBrush,
            new PointF(0, 0));

        }

      }
    }

    private void TrainEvolutionGraph_Click(object sender, EventArgs e)
    {
      TrainEvolutionGraph.Refresh();
    }

  }

  public enum ShahafCycleState
  {
    NotStarted,
    RunningStim,
    CoolDown,
    Finished
  }
  public class ShahafCycleIteration
  {
    public int num = 0;
    public int RS = 0;
    public int ElapsedStimTime = 0;
    public int ElapsedCoolDownTime = 0;
    public TTime StartTime = 0;
    public TTime StartCoolDown = 0;
    public TTime FinishTime = 0;

    public ShahafCycleIteration()
    {
    }
  }
  public struct EvokedPackInfo
  {
    public CPack Pack;
    public TTime AbsStim;
  }

}
