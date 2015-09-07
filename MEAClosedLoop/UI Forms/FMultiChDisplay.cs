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
  public delegate void OnPacketRecievedDelegate(TFltDataPacket packet);
  public partial class FMultiChDisplay : Form, IRecieveFltData
  {
    private event OnPacketRecievedDelegate onPacketRecieved;
    private Queue<double> dataQueue = new Queue<double>();
    private Dictionary<int, PointPairList> mchDataQueue = new Dictionary<int, PointPairList>();
    private Queue<TFltDataPacket> unpackedFltDataQueue = new Queue<TFltDataPacket>();
    private object DataQueueLock = new object();
    Timer plotUpdater;
    const uint updateInterval = 1000;
    int currentChNum = 0;
    uint _Duration2 = Param.MS * 1000;
    uint _Amplitude = 600;
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
    public FMultiChDisplay()
    {
      InitializeComponent();
      initPlot();
    }
    void IRecieveFltData.RecieveFltData(TFltDataPacket packet)
    {
      if (onPacketRecieved != null) onPacketRecieved(packet);
      if (plotUpdater.Enabled)
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
    void initPlot()
    {
      // Создаем экземпляр класса MasterPane, который представляет собой область, 
      // на которйо "лежат" все графики (экземпляры класса GraphPane)

      ZedGraph.MasterPane masterPane = zedGraphPlot.MasterPane;
      // По умолчанию в MasterPane содержится один экземпляр класса GraphPane 
      // (который можно получить из свойства zedGraph.GraphPane)
      // Очистим этот список, так как потом мы будем создавать графики вручную
      masterPane.PaneList.Clear();
      masterPane.Margin.All = 0;
      masterPane.Border.IsVisible = false;
      masterPane.InnerPaneGap = 0;
      masterPane.IsAntiAlias = false;
      for (int i = 0; i < 64; i++)
      {
        // Создаем экземпляр класса GraphPane, представляющий собой один график
        GraphPane pane = new GraphPane();
        
        pane.Margin.Top = 0;
        pane.Margin.Left = 1;
        pane.Margin.Right = 1; 
        pane.Margin.Bottom = 0;
        pane.XAxis.Scale.IsSkipFirstLabel = true;
        pane.XAxis.Scale.IsSkipLastLabel = true;
        pane.YAxis.Scale.IsSkipFirstLabel = true;
        pane.YAxis.Scale.IsSkipLastLabel = true;
        pane.XAxis.Scale.FontSpec.Size = 9;
        pane.YAxis.Scale.FontSpec.Size = 9;
        //pane.XAxis.Scale.MajorStep = 0.5;
        //pane.XAxis.Scale.MinorStep = 0.5;
        //pane.YAxis.Scale.MajorStep = 0.5;
        //pane.YAxis.Scale.MinorStep = 0.5;
        pane.YAxis.Scale.Align =  AlignP.Inside;
        pane.YAxis.Scale.AlignH = AlignH.Right;
        //pane.XAxis.Scale.AlignH = AlignH.Left;
        //pane.XAxis.Scale.Align = AlignP.Inside;

        pane.XAxis.Cross = +Amplitude2;
        pane.X2Axis.IsVisible = false;
        pane.XAxis.Title.IsVisible = false;
        pane.BaseDimension = 1.5f;
        pane.XAxis.IsVisible = false;
        pane.Y2Axis.IsVisible = false;
        pane.YAxis.IsVisible = false;
        if (i < 8)
        {
          pane.XAxis.IsVisible = true;
        }
        //pane.Margin.Left = -15;
        if (i % 8 == 0)
        {
          pane.Margin.Left = 0;
          pane.YAxis.IsVisible = true;
        }
        pane.Legend.IsVisible = false;
        pane.Title.IsVisible = false;
        //pane.TitleGap = 0;
        // Добавим новый график в MasterPane
        masterPane.Add(pane);
      }

      // Будем размещать добавленные графики в MasterPane
      using (Graphics g = CreateGraphics())
      {
        // Графики будут размещены в три строки.
        // В первой будет 4 столбца,
        // Во второй - 2
        // В третей - 3
        // Если бы второй аргумент был равен false, то массив из третьего аргумента
        // задавал бы не количество столбцов в каждой строке,
        // а количество строк в каждом столбце
        masterPane.SetLayout(g, true, new int[] { 8, 8, 8, 8, 8, 8, 8, 8 });
      }

      // Обновим оси и перерисуем график
      zedGraphPlot.AxisChange();
      zedGraphPlot.Invalidate();
    }

    private void updatePlot()
    {
      lock (DataQueueLock)
      {
        while (unpackedFltDataQueue.Count > 0)
        {
          
          TFltDataPacket currentPacket = unpackedFltDataQueue.Dequeue();
          int PartsLength ;
          int PartsCount;
          foreach (int key in mchDataQueue.Keys)
          {
            double[] Data = currentPacket[key];

            PartsLength = (Data.Length > 0) ? Data.Length / zedGraphPlot.Width : 0;
            PartsCount = (PartsLength > 0) ? Data.Length / PartsLength : 0;

            double min = double.MaxValue;
            double max = double.MinValue;
            for (int i = 0; i <= PartsCount; i++)
            {
              min = double.MaxValue;
              max = double.MinValue;
              for (int ii = 0; ii < PartsLength && i * PartsLength + ii < Data.Length; ii++)
              {
                if (Data[i * PartsLength + ii] > max) max = Data[i * PartsLength + ii];
                if (Data[i * PartsLength + ii] < min) min = Data[i * PartsLength + ii];
              }
              mchDataQueue[key].Add(i * PartsLength / 25.0, min);
              mchDataQueue[key].Add(i * PartsLength / 25.0, max);
            }
          }
        }
        int length = 0;
        int dequeueCount = 0;
        foreach (int key in mchDataQueue.Keys)
        {
          length = mchDataQueue[key].Count / 2;
          dequeueCount = length - (int)Duration2;
          if (dequeueCount > 0)
            mchDataQueue[key].RemoveRange(0, dequeueCount * 2);
        }
      }
      if (this.IsDisposed)
      {
        plotUpdater.Stop();
        return;
      }
      int debug = Environment.TickCount;
      GraphPane pane = zedGraphPlot.GraphPane;
      pane.CurveList.Clear();
      PaneList paneList = zedGraphPlot.MasterPane.PaneList;
      for (int ch = 0; ch < 59; ch++)
      {
        pane = paneList[ch];
        pane.XAxis.Scale.Min = 0;
        pane.XAxis.Scale.Max = Duration2 / 25.0;
        pane.YAxis.Scale.Min = -Amplitude2;
        pane.YAxis.Scale.Max = +Amplitude2;
        
        LineItem f1_curve = pane.AddCurve("Neuronal Activity", mchDataQueue[ch], Color.Blue, SymbolType.None);
        //LineItem f2_curve = pane.AddCurve("In  tegral", f2_list, Color.Red, SymbolType.None);
        // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
        // В противном случае на рисунке будет показана только часть графика, 
        // которая умещается в интервалы по осям, установленные по умолчанию

      }
      zedGraphPlot.AxisChange();

      string s = (Environment.TickCount - debug).ToString() + " ms";
      if (UpdateTimeLabel.InvokeRequired)
        UpdateTimeLabel.BeginInvoke(new Action<System.Windows.Forms.Label>((lab) => lab.Text = s), UpdateTimeLabel);
      else
        UpdateTimeLabel.Text = s;

      // Обновляем график
      zedGraphPlot.Invalidate();
      //filteredList = new FilteredPointList(new double[0], new double[0]);
      //plotUpdater.Start();
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

    private void FMultiChDisplay_Load(object sender, EventArgs e)
    {
      onPacketRecieved += initCollection;
    }

    private void initCollection(TFltDataPacket packet)
    {
      foreach (int key in packet.Keys)
        mchDataQueue.Add(key, new PointPairList());
      onPacketRecieved -= initCollection;
    }
  }
}
