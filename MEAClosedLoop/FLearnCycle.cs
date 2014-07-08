using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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

    #endregion

    #region Внутренние данные
    private CFiltering Filter;
    private CLoopController loopController;
    private Form1 MainForm;
    private Queue<CPack> PackQueue;
    private Queue<TTime> StimQueue;

    private Object StimQueueLock = new Object();
    private Object PackQueueLock = new Object();

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

    private void AddPack(CPack pack)
    {
      lock (PackQueueLock)
      {
        if (true)
        {
          PackQueue.Enqueue(pack);
        }
      }
    }
    private bool ChekSpike()
    {
    }
  }
}
