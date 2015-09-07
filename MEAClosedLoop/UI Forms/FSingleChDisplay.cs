using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace MEAClosedLoop.UI_Forms
{
  using TData = System.Double;
  using TTime = System.UInt64;
  using TStimIndex = System.Int16;
  using TAbsStimIndex = System.UInt64;
  using TRawDataPacket = Dictionary<int, ushort[]>;
  using TFltDataPacket = Dictionary<int, System.Double[]>;

  public partial class FSingleChDisplay : Form, IRecieveFltData
  {
    private Queue<double> dataQueue = new Queue<double>();
    private Queue<TFltDataPacket> unpackedFltDataQueue = new Queue<TFltDataPacket>();
    private object DataQueueLock = new object();
    Timer plotUpdater;
    const uint updateInterval = 1000;
    int currentChNum = 0;
    uint _Duration2 = Param.MS * 1000;
    uint _Amplitude = 100;
    uint Amplitude2
    {
      get
      {
        return _Amplitude;
      }
      set
      {
        if (value > 10 && value < 3000)
          _Amplitude = value;
      }
    }
    uint Duration2
    {
      get { return _Duration2; }
      set { if (value >= Param.MS * 1000)  _Duration2 = value; }
    }
    public FSingleChDisplay()
    {
      InitializeComponent();
    }
    public FSingleChDisplay(int ChNum)
    {
      InitializeComponent();
    }

    void IRecieveFltData.RecieveFltData(TFltDataPacket packet)
    {
      lock (DataQueueLock)
      {
        unpackedFltDataQueue.Enqueue(packet);
        if (unpackedFltDataQueue.Select(x => x[0].Length).Sum() > Param.MS * 61000)
        {
          unpackedFltDataQueue.Dequeue();
        }
      }
    }

    private void StartButton_Click(object sender, EventArgs e)
    {
      start();
    }

    void plotUpdater_Tick(object sender, EventArgs e)
    {
      Task updatetask = new Task(this.updatePlot);
      updatetask.Start();

    }
    private void updatePlot()
    {
      if (this.IsDisposed)
      {
        plotUpdater.Stop();
        return;
      }
      int debug = Environment.TickCount;
      GraphPane pane = zedGraphPlot.GraphPane;
      zedGraphPlot.MasterPane.Border = new Border(false, Color.White, 0);
      zedGraphPlot.MasterPane.InnerPaneGap = 0;
      //zedGraphPlot.MasterPane.BaseDimension = 3;
      pane.Margin.Top = 0;
      pane.Margin.Left = 0;
      pane.Margin.Right = 0;
      pane.Margin.Bottom = 0;
      pane.XAxis.Title.IsVisible = false;
      pane.YAxis.Title.IsVisible = false;
      pane.Legend.IsVisible = false;
      pane.Title.IsVisible = false;
      pane.TitleGap = 0;
      pane.CurveList.Clear();
      pane.XAxis.IsVisible = false;
      pane.X2Axis.Cross = +Amplitude2;
      pane.BaseDimension = 11;
      pane.X2Axis.IsVisible = true;
      PointPairList f1_list = new PointPairList();

      //data prepear;
      double[] Data;
      lock (DataQueueLock)
      {
        while (unpackedFltDataQueue.Count > 0)
        {
          double[] data = unpackedFltDataQueue.Dequeue()[currentChNum];
          for (int i = 0; i < data.Length; i++)
            dataQueue.Enqueue(data[i]);
          while (dataQueue.Count > Duration2)
          {
            dataQueue.Dequeue();
          }
        }
      }
      Data = dataQueue.ToArray();
      TData[] x = new TData[Data.Length];
      TData[] y = new TData[Data.Length];

      for (int i = 0; i < Data.Length; i++)
      {
        //f1_list.Add(i / 25.0, Data[i]);
        x[i] = i / 25.0;
        y[i] = Data[i];
      }
      int PartsLength;
      PartsLength = (Data.Length > 0) ? Data.Length / zedGraphPlot.Width : 0;
      int PartsCount = (PartsLength > 0) ? Data.Length / PartsLength : 0;
      double min = double.MaxValue;
      double max = double.MinValue;
      if (Duration2 > Param.MS * 1000)
        for (int i = 0; i < PartsCount; i++)
        {
          min = double.MaxValue;
          max = double.MinValue;
          for (int ii = 0; ii < PartsLength; ii++)
          {
            if (Data[i * PartsLength + ii] > max) max = Data[i * PartsLength + ii];
            if (Data[i * PartsLength + ii] < min) min = Data[i * PartsLength + ii];
          }
          f1_list.Add(i * PartsLength / 25.0, min);
          f1_list.Add(i * PartsLength / 25.0, max);
        }
      else
      {
        for (int i = 0; i < Data.Length; i++)
        {
          f1_list.Add(i / 25.0, Data[i]);
        }
      }
      FilteredPointList filteredList = new FilteredPointList(x, y);

      if (Duration2 > Param.MS * 1000 * 3 && x.Length > 0)
        filteredList.SetBounds(x[0], x[x.Length - 1], zedGraphPlot.Width * 5);
      pane.XAxis.Scale.Min = 0;
      pane.XAxis.Scale.Max = Duration2 / 25.0;
      pane.YAxis.Scale.Min = -Amplitude2;
      pane.YAxis.Scale.Max = +Amplitude2;

      LineItem f1_curve = pane.AddCurve("Neuronal Activity", f1_list, Color.Blue, SymbolType.None);
      //LineItem f2_curve = pane.AddCurve("In  tegral", f2_list, Color.Red, SymbolType.None);
      // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
      // В противном случае на рисунке будет показана только часть графика, 
      // которая умещается в интервалы по осям, установленные по умолчанию
      zedGraphPlot.AxisChange();
      string s = (Environment.TickCount - debug).ToString() + " ms";
      if (UpdateTimeLabel.InvokeRequired)
        UpdateTimeLabel.BeginInvoke(new Action<System.Windows.Forms.Label>((lab) => lab.Text = s), UpdateTimeLabel);
      else
        UpdateTimeLabel.Text = s;

      // Обновляем график
      zedGraphPlot.Invalidate();
      filteredList = new FilteredPointList(new double[0], new double[0]);

    }
    private void start()
    {
      plotUpdater = new Timer();
      plotUpdater.Interval = 1000;
      plotUpdater.Tick += plotUpdater_Tick;
      plotUpdater.Start();
    }
    private void stop()
    {
      plotUpdater.Stop();
    }

    private void stopButton_Click(object sender, EventArgs e)
    {
      stop();
    }

    private void AmplitudeChecker_ValueChanged(object sender, EventArgs e)
    {
      Amplitude2 = (uint)(sender as NumericUpDown).Value;
    }

    private void DurationChecker_ValueChanged(object sender, EventArgs e)
    {
      Duration2 = (uint)(sender as NumericUpDown).Value * Param.MS * 1000;
    }

    private void ChNumChecker_ValueChanged(object sender, EventArgs e)
    {
      lock (DataQueueLock)
      {
        dataQueue.Clear();
        currentChNum = (int)(sender as NumericUpDown).Value;
      }
    }
  }
}
