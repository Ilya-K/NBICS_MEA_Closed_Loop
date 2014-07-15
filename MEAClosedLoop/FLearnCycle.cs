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
    public int ChannelIdx = 1;
    #endregion

    #region Внутренние данные
    private CFiltering Filter;
    private CLoopController loopController;
    //private Form1 MainForm;
    private Queue<CPack> PackQueue = new Queue<CPack>();
    private Queue<TTime> StimQueue = new Queue<TTime>();
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
    }

    public void RecieveStimData(List<TAbsStimIndex> stimlist)
    {
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
      lock (PackQueueLock)
      {
        // Нам подходят только вызванные пачки
        // Совместим пачки со стимулами
        lock (StimQueueLock)
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
              PackQueue.Enqueue(pack);
            }
          }
        }
        if (true)
          PackQueue.Enqueue(pack);
        //10 - максимальное число S в отношении R/S

        // Очистка переполненных очередей
        for (; PackQueue.Count > 10; PackQueue.Dequeue()) ;
        for (; EvokedPacksQueue.Count > this.PRSCount.Value; EvokedPacksQueue.Dequeue()) ;
      }
      foreach (Control picturebox in RSPacks.Controls)
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
          if (currentIteration.ElapsedStimTime >= (int)(this.PStimLength.Value)
            || false)
          {
            loopController.DoStim = false;
            CycleState = ShahafCycleState.CoolDown;
            currentIteration.StartCoolDown = CurrentTime;
          }
          break;
        case ShahafCycleState.CoolDown:
          //добавляем время отдыха
          currentIteration.ElapsedCoolDownTime = (int)(CurrentTime - currentIteration.StartCoolDown);
          //если отдых пройден полностью - завершаем итерацию.
          // + Запускаем следующую итерацию цикла
          if (currentIteration.ElapsedCoolDownTime >= (int)(this.PCoolDownLength.Value * Param.MS))
          {
            CycleInfo.Add(currentIteration);
            currentIteration = new ShahafCycleIteration();

          }
          break;
      }
      // Если цикл длится слишком долго.
      if (CurrentTime - StartTime > this.PExpMaxLength.Value
        && (CycleState == ShahafCycleState.RunningStim || CycleState == ShahafCycleState.CoolDown))
      {
        CycleState = ShahafCycleState.Finished;
        CycleInfo.Add(currentIteration);
      }
    }

    private void RunNewCycleIteration()
    {
      currentIteration = new ShahafCycleIteration();
      currentIteration.StartTime = CurrentTime;
      CycleState = ShahafCycleState.RunningStim;
    }

    //Pack Start Time && Stim Time may be absolute but Center && Delta mast be relative
    private bool ChekSpike(CPack pack, TTime StimTime, TTime CenterTime, TTime Delta)
    {
      //Пачка началась сильно позже стимула? т.е. не считалась вызванной
      if (pack.Start > StimTime + StimControlDuration)
        return false;
      //Пачка закончилась раньше начала стимула
      if (pack.Start + (TTime)pack.Length < StimTime)
        return false;
      TTime StartSearchTime = (pack.Start <= StimTime) ? StimTime - pack.Start + CenterTime - Delta : pack.Start - StimTime + CenterTime - Delta;
      Average average = new Average();
      for (int i = 0; i < pack.NoiseLevel.Length; i++)
      {
        average.AddValueElem(Math.Abs(pack.NoiseLevel[i]));
      }
      average.Calc();
      for (TTime i = StartSearchTime; i < StartSearchTime + 2 * Delta && i < (TTime)pack.Length; i++)
      {
        if (Math.Abs(pack.Data[ChannelIdx][i]) > average.TripleSigma)
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
      StartTime = Filter.TimeStamp;
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
        SomePack.Location = new Point(20, 20 + 108 * i);
        SomePack.BackColor = Color.White;
        SomePack.Size = new Size(400, 100);
        SomePack.Paint += SomePack_Paint;
        this.RSPacks.Controls.Add(SomePack);
        SomePack.Refresh();
      }
    }

    void SomePack_Paint(object sender, PaintEventArgs e)
    {
      int i;
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

          
          
          //отрисовка пачки
          //[TODO]: Сделать оптимизацию (отрисовывать только входяющую в окно часть пачки)
          for (int idx = 0; idx < PackData[ChannelIdx].Length - 1 /*&& idx < 110 * Param.MS*/; idx++)
          {

            e.Graphics.DrawLine(new Pen(packBrush),
              new Point((int)((idx - StimShift + PackShift) * k) - 60, (int)PackData[ChannelIdx][idx] / 10 + e.ClipRectangle.Height / 2),
              new Point((int)((idx - StimShift + PackShift) * k) - 60, (int)PackData[ChannelIdx][idx + 1] / 10 + e.ClipRectangle.Height / 2 + 1) // + 1 - фикс для отрисовки линии, равной нулю.
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
        }
          
      }
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
