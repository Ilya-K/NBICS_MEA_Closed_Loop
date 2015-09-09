using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
namespace MEAClosedLoop
{
  public partial class FBurstDescription : Form, IRecieveBusrt
  {
    public FBurstDescription()
    {
      InitializeComponent();
    }
    void IRecieveBusrt.RecieveBurst(CPack Burst)
    {
      GraphPane pane = PlotGraph.GraphPane;
      pane.CurveList.Clear();

      PointPairList f1_list = new PointPairList();

      // Создадим список точек для кривой f2(x)
      PointPairList f2_list = new PointPairList();
      // Создадим список точек для кривой f2(x)
      PointPairList f3_list = new PointPairList();
      double[] Data = Burst.Data[(int)numericUpDown1.Value];
      for (int i = 0; i < Data.Length; i++)
      {
        f1_list.Add(i / 25.0, Data[i]);
      }
      //f2_list.Add(Data[0], 0);
      int window = 120;
      double[] averages = new double[Data.Length];

      Average stat = new Average();
      for (int i = 0; i < 200; i++)
      {
        if (Data[i] > 0) stat.AddValueElem(Data[i]);
      }
      stat.Calc();
      double[] fxs = new double[Data.Length];
      fxs[0] = Math.Abs(Data[0]);
      double last;

      for (int i = 1; i < Data.Length; i++)
      {
        last = fxs[i - 1];
        fxs[i] = (Data[i] > stat.Sigma * 3) ? last + Math.Abs(Data[i]) / 30 - last / 460 : last - last / 460;
      }

      double average = stat.Sigma * 4; // fxs.Take(window).Average() / (double)window;
      for (int i = 0; i < window; i++)
      {
        averages[i] = average;
      }

      for (int i = window; i < Data.Length; i++)
      {
        average += (fxs[i] - fxs[i - window]) / (double)window;
        averages[i] = average > 0 ? average : 0;
      }
      for (int i = 1; i < Data.Length; i++)
      {
        f2_list.Add(i / 25.0, averages[i]);
        f3_list.Add(i / 25.0, fxs[i]);
      }
      LineItem f2_curve = pane.AddCurve("Integral after Mooveing Avarage", f2_list, Color.Red, SymbolType.None);
      LineItem f3_curve = pane.AddCurve("Integral", f3_list, Color.Green, SymbolType.None);
      LineItem f1_curve = pane.AddCurve("Raw Burst", f1_list, Color.Blue, SymbolType.None);
      // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
      // В противном случае на рисунке будет показана только часть графика, 
      // которая умещается в интервалы по осям, установленные по умолчанию
      PlotGraph.AxisChange();

      // Обновляем график
      PlotGraph.Invalidate();
    }

    private void FBurstDescription_Load(object sender, EventArgs e)
    {
      PlotGraph.GraphPane.YAxis.Scale.Min = -(int)numericUpDown2.Value;
      PlotGraph.GraphPane.YAxis.Scale.Max = (int)numericUpDown2.Value;
    }

    private void numericUpDown2_ValueChanged(object sender, EventArgs e)
    {
      PlotGraph.GraphPane.YAxis.Scale.Min = -(int)numericUpDown2.Value;
      PlotGraph.GraphPane.YAxis.Scale.Max = (int)numericUpDown2.Value;
    }

  }
  public enum DecsriptionWay
  {
    Integral,
    MoovingAverage,
    FlowFreq
  }
}
