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
    uint Duration2
    {
      get { return _Duration2; }
      set { _Duration2 = value; }
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
      GraphPane pane = zedGraphPlot.GraphPane;
      pane.CurveList.Clear();

      PointPairList f1_list = new PointPairList();

      // Создадим список точек для кривой f2(x)
      PointPairList f2_list = new PointPairList();

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
      Data = dataQueue.ToArray(); ;
      for (int i = 0; i < Data.Length; i++)
      {
        f1_list.Add(i / 25.0, Data[i]);
      }
      //int window = 40;
      //double[] averages = new double[Data.Length];

      //double[] fxs = new double[Data.Length];
      //fxs[0] = Data[0] / 5;
      //for (int i = 1; i < Data.Length; i++)
      //{
      //  double last = fxs[i - 1];
      //  fxs[i] = last + Math.Abs(Data[i]) / 5 - last / 60;
      //}
      //double average = fxs.Take(window).Average() / (double)window;
      //for (int i = 0; i < window; i++)
      //{
      //  averages[i] = average;
      //}

      //for (int i = window; i < Data.Length; i++)
      //{
      //  average += (fxs[i] - fxs[i - window]) / (double)window;
      //  averages[i] = average;
      //}
      //for (int i = 1; i < Data.Length; i++)
      //{
      //  f2_list.Add(i / 25.0, averages[i]);
      //}

      LineItem f1_curve = pane.AddCurve("Sinc", f1_list, Color.Blue, SymbolType.None);
      //LineItem f2_curve = pane.AddCurve("In  tegral", f2_list, Color.Red, SymbolType.None);
      // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
      // В противном случае на рисунке будет показана только часть графика, 
      // которая умещается в интервалы по осям, установленные по умолчанию
      zedGraphPlot.AxisChange();

      // Обновляем график
      zedGraphPlot.Invalidate();
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
  }
}
