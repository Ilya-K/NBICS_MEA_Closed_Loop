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
      double[] Data = Burst.Data[(int)numericUpDown1.Value];
      for (int i = 0; i < Data.Length; i++)
      {
        f1_list.Add(i / 25.0, Data[i]);
      }
      //f2_list.Add(Data[0], 0);
      int window = 40;
      double[] averages = new double[Data.Length];

      double[] fxs = new double[Data.Length];
      fxs[0] = Data[0] / 5;
      for (int i = 1; i < Data.Length; i++)
      {
        double last = fxs[i - 1];
        fxs[i] = last + Math.Abs(Data[i]) / 5 - last / 60;
      }
      double average = fxs.Take(window).Average() / (double)window;
      for (int i = 0; i < window; i++)
      {
        averages[i] = average;
      }

      for (int i = window; i < Data.Length; i++)
      {
        average += (fxs[i] - fxs[i - window]) / (double)window;
        averages[i] = average;
      }
      for (int i = 1; i < Data.Length; i++)
      {
        f2_list.Add(i / 25.0, averages[i]);
      }

      LineItem f1_curve = pane.AddCurve("Sinc", f1_list, Color.Blue, SymbolType.None);
      LineItem f2_curve = pane.AddCurve("Integral", f2_list, Color.Red, SymbolType.None);
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
