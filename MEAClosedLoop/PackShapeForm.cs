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
  public partial class PackShapeForm : Form
  {
    Panel PackShapeGraph;
    int channel2draw;
    const int X_BORDER_SHIFT = 5, Y_BORDER_SHIFT = 5;
    PackGraph dataGenerator;
    uint[] data;
    private Point[] pointsToDraw;

    public PackShapeForm(PackGraph _dataGenerator)
    {
      InitializeComponent();
      dataGenerator = _dataGenerator;
      data = null;
      PackShapeGraph = new Panel();
      PackShapeGraph.Location = new Point(X_BORDER_SHIFT, Y_BORDER_SHIFT);
      PackShapeGraph.Size = new System.Drawing.Size(this.Size.Width - 2 * X_BORDER_SHIFT, button1.Location.Y - 2 * Y_BORDER_SHIFT);
      PackShapeGraph.BorderStyle = BorderStyle.FixedSingle;
      PackShapeGraph.BackColor = Color.White;
      this.Controls.Add(PackShapeGraph);
      pointsToDraw = new Point[PackShapeGraph.Size.Width];
      
      //this.IsAccessible = false;
      //this.Hide();
      this.Visible = false;
    }

    public void SetChannel(int _channel)
    {
      channel2draw = _channel;
      PackShapeGraph.Paint += PaintShapeGraph;
    }

    private void PaintShapeGraph(object sender, PaintEventArgs e)
    {
      int width = ((Panel)sender).Width;
      int height = ((Panel)sender).Height;
      double dataScale;
      data = dataGenerator.PrepareShape(channel2draw, width, height, out dataScale);

      //drawing data
      for (int i = 0; i < data.Count<uint>(); i++)
      {
        pointsToDraw[i] = new Point(i, (data[i] < height) ? height - (int)data[i] : height);
      }
      Pen pen = new Pen(Color.DodgerBlue, 1);
      e.Graphics.DrawLines(pen, pointsToDraw);

      //drawing scale
      using (SolidBrush textBrush = new SolidBrush(Color.Green), backgroundBrush = new SolidBrush(Color.White))
      {
        StringFormat sf = new StringFormat();
        double roundedScale = Math.Round(dataScale, 2);
        string scaleString = ((roundedScale != 0) ? "x" + roundedScale.ToString() : ">0.01");
        sf.FormatFlags = StringFormatFlags.NoWrap;
        SizeF ScaleStringSize = new SizeF();
        ScaleStringSize = e.Graphics.MeasureString(scaleString, this.Font, width, sf);
        e.Graphics.FillRectangle(backgroundBrush, 10, 10, ScaleStringSize.Width, ScaleStringSize.Height);
        e.Graphics.DrawString(scaleString, this.Font, textBrush, new Point(10, 10), sf);
      }
      data = null;
    }

    private void button1_Click(object sender, EventArgs e) //accept
    {
      this.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.Hide();
    }

    private void button2_Click(object sender, EventArgs e) //abort
    {
      this.DialogResult = System.Windows.Forms.DialogResult.Abort;
      this.Hide();
    }
  }
}
