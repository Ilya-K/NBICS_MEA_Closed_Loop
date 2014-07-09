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
    
    public int ChannelIdx = 1;
    #endregion

    #region Внутренние данные
    private CFiltering Filter;
    private CLoopController loopController;
    private Form1 MainForm;
    private Queue<CPack> PackQueue;
    private Queue<TTime> StimQueue;

    private Object StimQueueLock = new Object();
    private Object PackQueueLock = new Object();

    private TTime CurrentTime = 0;

    #endregion

    public FLearnCycle(CLoopController _LoopController, CFiltering _Filter)
    {
      InitializeComponent();

      Filter = _Filter;
      loopController = _LoopController;
      //DEBUG
      PictureBox SomePack = new PictureBox();
      SomePack.Location = new Point(20, 20);
      SomePack.BackColor = Color.White;
      SomePack.Size = new Size(400, 100);
      this.RSPacks.Controls.Add(SomePack);
      SomePack = new PictureBox();
      SomePack.Location = new Point(20, 140);
      SomePack.BackColor = Color.White;
      SomePack.Size = new Size(200, 50);
      this.RSPacks.Controls.Add(SomePack);
      // END DEBUG

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
        foreach (TTime stim in stimlist)
        {
          StimQueue.Enqueue(stim);
        }
    }

    private void RecievePackData(CPack pack)
    {
      lock (PackQueueLock)
      {
        if (true)
        {
          PackQueue.Enqueue(pack);
        }
      }
    }
    public void UpdateTime(TFltDataPacket data) //Recieve Flt Data
    {
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
      //[TO DO] более правильный выбор начала поиска (часть, когда pack.Start > StimTime необходимо уточнить)
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
    }

    private void FinishCycle_Click(object sender, EventArgs e)
    {
      loopController.OnPackFound -= RecievePackData;
      Filter.RemoveStimulConsumer(RecieveStimData);
      Filter.RemoveDataConsumer(UpdateTime);
    }
    
  }
}
