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
  public partial class FLearnCycle : Form
  {
    private CFiltering Filter;
    private CLoopController loopController;
    private Form1 MainForm; 
    public FLearnCycle(CLoopController _LoopController, CFiltering _Filter)
    {
      InitializeComponent();

      Filter = _Filter;
      loopController = _LoopController;
    }

    private void pictureBox1_Paint(object sender, PaintEventArgs e)
    {
      int padding_left = 5;
      int padding_bottom = 5;
      SolidBrush AxisBrush = new SolidBrush(Color.Black);
      Pen AxisPen = new Pen(AxisBrush, 1);
      // Отрисовка осей
      e.Graphics.DrawLine(AxisPen, 
        new Point(0, e.ClipRectangle.Height -padding_bottom ),
        new Point(e.ClipRectangle.Width, e.ClipRectangle.Height - padding_bottom));
    }
  }
}
