using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
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

    private int SigmaCount = 8; // во сколько раз значение сигнала должно превышать сигму шума, что бы считаться  спайком

    public int ChannelIdx = 1; // default channel index

    private bool _ManualBreak = false;

    private bool ManualBreak
    {
      set
      {
        _ManualBreak = value;
        if (_ManualBreak)
        {
          RSManualButton.BeginInvoke(new Action(() => RSManualButton.Enabled = false));
        }
      }
      get
      {
        bool old = _ManualBreak;
        _ManualBreak = false;
        RSManualButton.BeginInvoke(new Action(() => RSManualButton.Enabled = true));
        return old;
      }
    } // после одного чтения становится false

    #endregion

    #region Внутренние данные
    private CFiltering Filter;
    private CLoopController loopController;
    //private Form1 MainForm;

    //очередь пачек (содержит RS пачки из последних N вызванных)
    private Queue<EvokedPackInfo> BurstQueue = new Queue<EvokedPackInfo>();
 
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

    private string logFilePath = "";
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
        SomePack.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
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
      int padding_top = 10;
      int padding_right = 10;
      int max_overhead = 30 * 1000 * Param.MS;
      int max = (CycleInfo.Count > 0) ? (from info in CycleInfo select info.ElapsedStimTime).Max() : 0;
      max += max_overhead;
      SolidBrush AxisBrush = new SolidBrush(Color.Black);
      Pen AxisPen = new Pen(AxisBrush, 1);
      // Отрисовка осей
      e.Graphics.DrawLine(AxisPen,
        new Point(0, e.ClipRectangle.Height - padding_bottom),
        new Point(e.ClipRectangle.Width - padding_right, e.ClipRectangle.Height - padding_bottom));
      e.Graphics.DrawLine(AxisPen,
        new Point(padding_left, padding_top),
        new Point(padding_left, e.ClipRectangle.Height));
      // отрисовка масштаба

      #region вертикаль
      e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Black)),
        (float)(padding_left / (float)2),
        (float)(e.ClipRectangle.Height - padding_bottom + padding_top) / 2,
        (float)(3 * padding_left / (float)2),
        (float)((e.ClipRectangle.Height - padding_bottom + padding_top) / 2)
        );
      e.Graphics.DrawString(
        (max / (float)(2*Param.MS * 1000)).ToString(),
        new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular),
        new SolidBrush(Color.Black),
        new PointF(
          (float)(3 * padding_left / (float)2),
          (float)((e.ClipRectangle.Height - padding_bottom + padding_top) / 2)
          )
        );
      e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Black)),
         (float)(padding_left / (float)2),
         (float)(padding_top),
         (float)(3 * padding_left / (float)2),
         (float)(padding_top)
         );
      e.Graphics.DrawString(
        (max / (Param.MS * 1000)).ToString() + " ,сек",
        new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular),
        new SolidBrush(Color.Black),
        new PointF(
          (float)(3 * padding_left / (float)2),
          (float)(padding_top)
        )
      );
      #endregion

      #region горизонталь
      if ((CycleInfo.Count % 2 == 0 || CycleInfo.Count > 8) && CycleInfo.Count > 1)
      {
        e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Black)),
          (float)((e.ClipRectangle.Width - padding_right + padding_left) / 2),
          (float)(e.ClipRectangle.Height - padding_bottom / (float)2),
          (float)((e.ClipRectangle.Width - padding_right + padding_left) / 2),
          (float)(e.ClipRectangle.Height - 3 * padding_bottom / (float)2)
          );
        e.Graphics.DrawString(
          (CycleInfo.Count / 2).ToString(),
          new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular),
          new SolidBrush(Color.Black),
          new PointF(
            (float)((e.ClipRectangle.Width - padding_right + padding_left) / 2),
            (float)(e.ClipRectangle.Height - 3 * padding_bottom / (float)2)
          )
        );
      }

      e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Black)),
       (float)(e.ClipRectangle.Width - padding_right),
       (float)(e.ClipRectangle.Height - padding_bottom / (float)2),
       (float)(e.ClipRectangle.Width - padding_right),
       (float)(e.ClipRectangle.Height - 3 * padding_bottom / (float)2)
       );
      e.Graphics.DrawString(
        CycleInfo.Count.ToString() + " итераций",
        new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular),
        new SolidBrush(Color.Black),
        new PointF(
          (float)(e.ClipRectangle.Width - padding_right - 70),
          (float)(e.ClipRectangle.Height - 3 * padding_bottom / (float)2 - 10)
        )
      );
      #endregion

      // Отрисовка графика
      if (CycleInfo.Count == 0) return;
      float XProportional = (e.ClipRectangle.Width - (padding_bottom + padding_top)) / (CycleInfo.Count + 1);
      float YProportional = (e.ClipRectangle.Height - (padding_left + padding_right)) /(float) max;
      for (int i = 0; i < CycleInfo.Count - 1; i++)
      {
        e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Black)),
          (float)(padding_left + i * XProportional),
          (float)(e.ClipRectangle.Height  - padding_bottom - CycleInfo[i].ElapsedStimTime * (double)YProportional),
          (float)(padding_left + (i + 1) * XProportional),
          (float)(e.ClipRectangle.Height - padding_bottom - CycleInfo[i + 1].ElapsedStimTime * (double) YProportional)
          );
      }
      for (int i = 0; i < CycleInfo.Count; i++)
      {
        e.Graphics.DrawEllipse(new Pen(new SolidBrush(Color.Red)),
          (float)(padding_left + i * XProportional - 2),
          (float)(e.ClipRectangle.Height - padding_bottom - CycleInfo[i].ElapsedStimTime * (double)YProportional - 2),
          (float)4,
          (float)4
          );
      }
    }

    public void RecieveStimData(List<TAbsStimIndex> stimlist)
    {

      if (CycleState != ShahafCycleState.RunningStim) return;
      if (currentIteration != null) currentIteration.ElapsedStimCount++;
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
                break;
              }
            }
          }
        }
        lock (PackQueueLock)
        {
          BurstQueue.Clear();
          foreach (EvokedPackInfo ev_pack in EvokedPacksQueue)
          {
            
            if (ChekSpike(ev_pack, (TAbsStimIndex)this.PDelayTime.Value * Param.MS, (TAbsStimIndex)this.PSearchDelta.Value * Param.MS))
              BurstQueue.Enqueue(ev_pack);
          }

          //10 - максимальное число S в отношении R/S

          // Очистка переполненных очередей
          for (; BurstQueue.Count > this.PRSCount.Value; BurstQueue.Dequeue()) ;
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

      TrainEvolutionGraph.BeginInvoke(new Action(() => TrainEvolutionGraph.Refresh()));

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

          bool IfBreak = ManualBreak;
          lock (PackQueueLock)
          {
            if (currentIteration.ElapsedStimTime >= (int)(this.PStimLength.Value) * Param.MS * 1000 || 
              BurstQueue.Count() >= this.PRSCount.Value ||
              IfBreak)
            {

              if (BurstQueue.Count() >= this.PRSCount.Value || IfBreak)
              {
                RSManualButton.BeginInvoke(new Action(() => RSManualButton.Enabled = false));
                if (StimBreakCheckBox.Checked)
                {
                  //если выключаем стимуляцию
                  LernLogTextBox.BeginInvoke(new Action<string>(s => LernLogTextBox.AppendText(s)),
                    Environment.NewLine +
                    "[" + (CurrentTime / 25000).ToString() +
                    "] Выполнен RS критерий( " +
                    currentIteration.ElapsedStimCount.ToString() +
                    " стимулов ), переход в отдых культуры");
                }
                else
                {
                  //если не выключаем стимуляцию

                  LernLogTextBox.BeginInvoke(new Action<string>(s => LernLogTextBox.AppendText(s)),
                    Environment.NewLine +
                    "[" + (CurrentTime / 25000).ToString() +
                    "] Выполнен RS критерий( " +
                    currentIteration.ElapsedStimCount.ToString() +
                    " стимулов ), cтимуляция продолжается");
                  CycleState = ShahafCycleState.PostRSStim;
                  loopController.DoStim = true;
                  return;

                }
              }
              else
              {
                LernLogTextBox.BeginInvoke(new Action<string>(s => LernLogTextBox.AppendText(s)),
                  Environment.NewLine + "[" + (CurrentTime / 25000).ToString() + "] Превышено время ожидания RS критерия, переход в отдых культуры");
              }

              TrainEvolutionGraph.BeginInvoke(new Action(() => TrainEvolutionGraph.Refresh()));

              loopController.OnPackFound -= RecievePackData;
              Filter.RemoveStimulConsumer(RecieveStimData);

              loopController.DoStim = false;
              CycleState = ShahafCycleState.CoolDown;
              currentIteration.StartCoolDown = CurrentTime;
              return;
            }
          }
          break;
        case ShahafCycleState.PostRSStim:
          if ((int)(CurrentTime - currentIteration.StartTime) >= (int)(this.PStimLength.Value) * Param.MS * 1000)
          {
            loopController.DoStim = false;
            CycleState = ShahafCycleState.CoolDown;
            currentIteration.StartCoolDown = CurrentTime;
            LernLogTextBox.BeginInvoke(new Action<string>(s => LernLogTextBox.AppendText(s)),
                  Environment.NewLine + "[" + (CurrentTime / 25000).ToString() + "] Достигнуто максимальное время стимуляции, начало отдыха культуры");
            
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

            loopController.OnPackFound += RecievePackData;
            Filter.AddStimulConsumer(RecieveStimData);
            //Filter.AddDataConsumer(UpdateTime);

            LernLogTextBox.BeginInvoke(new Action<string>(s => LernLogTextBox.Text += s),
                 Environment.NewLine + "[" + (CurrentTime / 25000).ToString() + "] Выполнен отдых культуры, итерация завершена");
            BurstQueue.Clear();
            EvokedPacksQueue.Clear();
            RunNewCycleIteration();
            RSManualButton.BeginInvoke(new Action(() => RSManualButton.Enabled = true));
            return;
          }
          break;
      }
      // Если цикл длится слишком долго.
      if (CurrentTime > this.PExpMaxLength.Value * Param.MS * 60 * 1000 + StartTime &&
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

    private bool ChekSpike(EvokedPackInfo ev_pack, TTime CenterTime, TTime Delta)
    {
      //Pack Start Time && Stim Time may be absolute but Center && Delta mast be relative (all in Points), NOT MS!

      //Пачка началась сильно позже стимула? т.е. не считалась вызванной
      if (ev_pack.Pack.Start > ev_pack.AbsStim + StimControlDuration)
      {
        MessageBox.Show("Ошибка в паре пачка - стимул");
        return false;
      }

      //Пачка закончилась раньше начала стимула
      if (ev_pack.Pack.Start + (TTime)ev_pack.Pack.Length < ev_pack.AbsStim)
      {
        MessageBox.Show("Ошибка в паре пачка - стимул");
        return false;
      }
      //Сдвиг начала  пачки (если стимул произошел раньше)
      int PackShift = (ev_pack.AbsStim < ev_pack.Pack.Start) ? (int)(ev_pack.Pack.Start - ev_pack.AbsStim) : 0;

      //Смещение пачки влево (если стимул внутри пачки)
      int StimShift = (ev_pack.AbsStim > ev_pack.Pack.Start) ? (int)(ev_pack.AbsStim - ev_pack.Pack.Start) : 0;

      
      //OLD BUG-Friendly
      /*
      TTime StartSearchTime = ((ev_pack.Pack.Start + Param.PRE_SPIKE) <= ev_pack.AbsStim)
        ? ev_pack.AbsStim - (ev_pack.Pack.Start + Param.PRE_SPIKE) + CenterTime - Delta
        : (ev_pack.Pack.Start + Param.PRE_SPIKE) - ev_pack.AbsStim + CenterTime - Delta;
      */
      //NEW BUG-Free ?? need test

      TTime StartSearchTime = (TTime)StimShift + Param.PRE_SPIKE + CenterTime - Delta - (TTime)PackShift;

      Average average = new Average();
      
      //вычесление среднего и сигмы для участка данных перед пачкой
      if (ev_pack.average == null || ev_pack.average.Sigma == 0)
      {
        for (int i = 0; i < Param.PRE_SPIKE; i++)
        {
          average.AddValueElem(Math.Abs(ev_pack.Pack.Data[(int)this.PSelectIndex.Value][i]));
        }
        average.Calc();
        ev_pack.average = average;
      }
      else
      {
        average = ev_pack.average;
      }
      double[] BurstData = ev_pack.Pack.Data[(int)this.PSelectIndex.Value];
      for (TTime i = StartSearchTime; i < StartSearchTime + 2 * Delta && i < (TTime)ev_pack.Pack.Length - 1; i++)
      {
        double Value = Math.Abs(BurstData[i]);
        if (Value > (average.Sigma * SigmaCount))
        {
          return true;
        }
      }
      return false;
    }

    private void StartCycle_Click(object sender, EventArgs e)
    {
      SaveFileDialog dialog = new SaveFileDialog();
      switch (dialog.ShowDialog())
      {
        case System.Windows.Forms.DialogResult.OK:
          logFilePath = dialog.FileName;
          break;
      }
      StartTime = Filter.TimeStamp;
      loopController.OnPackFound += RecievePackData;
      Filter.AddStimulConsumer(RecieveStimData);
      Filter.AddDataConsumer(UpdateTime);
      foreach (Control control in ParamBox.Controls)
      {
        control.Enabled = false;
      }
      RSManualButton.Enabled = true;
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
        SomePack.Location = new Point(5, 1 + 151 * i);
        SomePack.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
        SomePack.BackColor = Color.White;
        SomePack.Size = new Size(RSPacks.Width - 10, 150);
        SomePack.Paint += SomePack_Paint;
        this.RSPacks.Controls.Add(SomePack);
        SomePack.Refresh();
      }
    }

    void SomePack_Paint(object sender, PaintEventArgs e)
    {
      int position;
      int HorisontalProportional = 10;
      for (position = 0; position < RSPacks.Controls.Count; position++)
      {
        if (RSPacks.Controls[position].Equals(sender))
        {
          break;
        }
      }
      lock (PackQueueLock)
      {
        if (BurstQueue.Count > position)
        {
          SolidBrush packBrush = new SolidBrush(Color.DarkGray);
          SolidBrush BurstSpikeBrush = new SolidBrush(Color.DarkViolet);
          SolidBrush stimBrush = new SolidBrush(Color.DarkBlue);
          SolidBrush DeltaBrush = new SolidBrush(Color.Red);
          if (position >= EvokedPacksQueue.Count) return;
          TFltDataPacket PackData = BurstQueue.ElementAt(position).Pack.Data;
          CPack Pack = BurstQueue.ElementAt(position).Pack;
          TTime StimTime = BurstQueue.ElementAt(position).AbsStim;
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

          /*
          if (EvokedPacksQueue.ElementAt(position).average == null)
          {
            for (int idx = 0; idx < Param.PRE_SPIKE; idx++)
            {
              average.AddValueElem(Pack.Data[(int)this.PSelectIndex.Value][idx]);
            }
            average.Calc();
            lock (EvokedPacksLock)
              EvokedPacksQueue.ElementAt(position).average = average;

          }
          else
          {
            average = EvokedPacksQueue.ElementAt(position).average;
          }
          */

          average = BurstQueue.ElementAt(position).average;
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
      int position;
      float HorisontalProportional = 15;
      for (position = 0; position < evBurstPanel.Controls.Count; position++)
      {
        if (evBurstPanel.Controls[position].Equals(sender))
        {
          break;
        }
      }
      lock (PackQueueLock)
      {
        if (evBurstPanel.Controls.Count > position)
        {
          SolidBrush packBrush = new SolidBrush(Color.DarkGray);
          SolidBrush BurstSpikeBrush = new SolidBrush(Color.DarkViolet);
          SolidBrush stimBrush = new SolidBrush(Color.DarkBlue);
          SolidBrush DeltaBrush = new SolidBrush(Color.Red);

          if (position >= EvokedPacksQueue.Count) return;

          TFltDataPacket PackData = EvokedPacksQueue.ElementAt(position).Pack.Data;
          CPack Pack = EvokedPacksQueue.ElementAt(position).Pack;
          TTime StimTime = EvokedPacksQueue.ElementAt(position).AbsStim;

          // Пусть все окно - Время поиска + дельта + 10 мс
          int WindowTimelength = (int)this.PDelayTime.Value * Param.MS + (int)this.PSearchDelta.Value * Param.MS + 10 * Param.MS;

          //Исследуем пачку от момента начала стимула
          //Сдвиг начала отрисовки пачки (если стимул произошел раньше)
          int PackShift = (StimTime < Pack.Start) ? (int)(Pack.Start - StimTime) : 0;

          //Смещение пачки влево (если стимул внутри пачки)
          int StimShift = (StimTime > Pack.Start) ? (int)(StimTime - Pack.Start) : 0;

          //переводит Время(отсчет) в позицию на экране
          float k = (float)e.ClipRectangle.Width / (float)WindowTimelength;

          //DEBUG

          Average average = new Average();
          if (EvokedPacksQueue.ElementAt(position).average == null)
          {
            for (int idx = 0; idx < Param.PRE_SPIKE; idx++)
            {
              average.AddValueElem(Pack.Data[(int)this.PSelectIndex.Value][idx]);
            }
            average.Calc();
            lock (EvokedPacksLock)
              EvokedPacksQueue.ElementAt(position).average = average;

          }
          else
          {
            average = EvokedPacksQueue.ElementAt(position).average;
          }
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
          e.Graphics.DrawString(((Pack.Start - (currentIteration.StartTime + StartTime)) / 25000).ToString() + " sec",
            SystemFonts.MessageBoxFont, BurstSpikeBrush,
            new PointF(0, 0));
          
        }

      }
    }

    private void TrainEvolutionGraph_Click(object sender, EventArgs e)
    {
      TrainEvolutionGraph.Refresh();
    }

    private void FLearnCycle_FormClosing(object sender, FormClosingEventArgs e)
    {
      loopController.OnPackFound -= RecievePackData;
      Filter.RemoveStimulConsumer(RecieveStimData);
      Filter.RemoveDataConsumer(UpdateTime);
    }

    private void FLearnCycle_ResizeEnd(object sender, EventArgs e)
    {
      TrainEvolutionGraph.Refresh();
    }

    private void RSManualButton_Click(object sender, EventArgs e)
    {
      ManualBreak = true;
    }

    private void LernLogTextBox_TextChanged(object sender, EventArgs e)
    {
      if(logFilePath.Length > 10)
      using (StreamWriter swr = new StreamWriter(logFilePath))
      {
        swr.Write(LernLogTextBox.Text);
      }
    }
  }

  public enum ShahafCycleState
  {
    NotStarted,
    RunningStim,
    PostRSStim,
    CoolDown,
    Finished
  }

  public class ShahafCycleIteration
  {
    public int num = 0;
    public int RS = 0;
    public int ElapsedStimTime = 0;
    public int ElapsedCoolDownTime = 0;
    public int ElapsedStimCount = 0;
    public TTime StartTime = 0;
    public TTime StartCoolDown = 0;
    public TTime FinishTime = 0;

    public ShahafCycleIteration()
    {
    }
  }

  public class EvokedPackInfo
  {
    public CPack Pack;
    public TTime AbsStim;
    public Average average;
  }

}
