#define DEBUG_SPIKETRAINS
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
  using TTime = System.UInt64;
  using TData = System.Double;
  using TFltDataPacket = Dictionary<int, System.Double[]>;
  using TStimIndex = System.Int16;
  using TAbsStimIndex = System.UInt64;

  public partial class FStatForm : Form
  {
    private const int DEFAULT_CH = 0;
    private const int STAT_BUF_LEN = 20 * 1000 * Param.MS;
    private const int MIN_PACK_LENGTH = 32;
    private CFiltering m_dataStream;
    private TTime m_startTime = 0;
    private CLoopController m_loopCtrl;
    private TData[] m_data;
    private List<int> m_packList;
    private List<int> m_packList2;
    private TData[] m_SE500;
    private TData[] m_SE1s;
    private TData[] m_avgSE125;
    private TData[] m_SE500SE;
    private TData[] m_SE1sSE;
    private TData[] m_avgSE2;
    private Point[] m_dataPoints;
    private Point[] m_packPoints;
    private Point[] m_packPoints2;
    private Point[] m_SE500Points;
    private Point[] m_SE1sPoints;
    private Point[] m_SE1sSEPoints;
    private Point[] m_avgSE125Points;
    private Point[] m_SE500SEPoints;
    private Point[] m_avgSE2Points;
    private CCalcExpWndSE m_calcSE500 = new CCalcExpWndSE(250);
    private CCalcExpWndSE m_calcSE1s = new CCalcExpWndSE(25000);
    private CCalcExpWndSE m_calcSE125 = new CCalcExpWndSE(62);
    private CCalcExpWndSE m_calcSE500SE = new CCalcExpWndSE(250);
    private CCalcExpWndSE m_calcSE1sSE = new CCalcExpWndSE(250);
    private CExpAvg m_calcSE2Avg = new CExpAvg(1000);
    private CExpAvg m_calcAvgSE125 = new CExpAvg(62);
    private List<CPack> m_packs = new List<CPack>();
#if DEBUG_SPIKETRAINS
    private Dictionary<int, List<CSpikeTrainFrame>> m_dbgSpikeTrains;
#endif

    private int m_channel; // [TODO]: It's not a true channel name, but just an index in a Dic. Correct some day.
    private int m_prevDataPanelWidth;
    private volatile int m_time = 0;
    private int m_drawWidth = 0;
    private volatile bool m_running = false;
    private volatile bool m_refreshPanel2 = false;
    private bool m_packStarted = false;
    private int m_packTime = 0;


    public FStatForm(CFiltering fltStream, CLoopController loopCtrl)
    {
      InitializeComponent();

      m_data = new TData[STAT_BUF_LEN];
      m_packList = new List<int>();
      m_packList2 = new List<int>();
      m_SE500 = new TData[STAT_BUF_LEN];
      m_avgSE125 = new TData[STAT_BUF_LEN];
      m_SE1s = new TData[STAT_BUF_LEN];
      m_SE500SE = new TData[STAT_BUF_LEN];
      m_SE1sSE = new TData[STAT_BUF_LEN];
      m_avgSE2 = new TData[STAT_BUF_LEN];
      m_dataPoints = new Point[panel_Data.Width * 2];
      m_packPoints = new Point[panel_Data.Width * 2];
      m_packPoints2 = new Point[panel_Data.Width * 2];
      m_SE500Points = new Point[panel_Data.Width * 2];
      m_SE1sPoints = new Point[panel_Data.Width * 2];
      m_avgSE125Points = new Point[panel_Data.Width * 2];
      m_SE500SEPoints = new Point[panel_Data.Width * 2];
      m_SE1sSEPoints = new Point[panel_Data.Width * 2];
      m_avgSE2Points = new Point[panel_Data.Width * 2];
      
      m_dataStream = fltStream;
      if (loopCtrl != null)
      {
        m_loopCtrl = loopCtrl;
        m_loopCtrl.OnPackFound += PackCallback;
      }

      foreach (int channel in m_dataStream.ChannelList)
      {
        cb_Channel.Items.Add(channel);
      }

      if (cb_Channel.Items.Count >= DEFAULT_CH)
      {
        cb_Channel.SelectedIndex = DEFAULT_CH;
      }
      m_channel = cb_Channel.SelectedIndex;

      m_dataStream.AddDataConsumer(DataCallback);
      m_dataStream.AddStimulConsumer(ArtifCallback);
      m_prevDataPanelWidth = panel_Data.Width;
    }

    private void PackCallback(CPack pack)
    {
      m_packs.Add(pack);
#if DEBUG_SPIKETRAINS
      m_dbgSpikeTrains = m_loopCtrl.GetSpikeTrainsDbg();
      foreach (var spTrain in m_dbgSpikeTrains[m_channel])
      {
        if (spTrain.Start >= m_startTime)
        {
          lock (m_packList2)
          {
            m_packList2.Add((int)(spTrain.Start - m_startTime) + spTrain.Length);
          }
        }
      }
      if (m_dbgSpikeTrains[m_channel].Count % 2 == 1) m_packList2.RemoveAt(m_packList2.Count - 1);
      UpdateStoredPoints2(0);
#endif
    }
    
    private void DataCallback(TFltDataPacket data)
    {
      if (!m_running) return;
      if (m_startTime == 0) 
        m_startTime = m_dataStream.TimeStamp;

      int dataLength = data[data.Keys.First()].Length;
      if (m_time + dataLength > STAT_BUF_LEN)
      {
        dataLength = STAT_BUF_LEN - m_time;
        m_running = false;
      }

      for (int i = 0; i < dataLength; ++i)
      {
        m_data[m_time] = data[m_channel][i];
        m_SE500[m_time] = m_calcSE500.SE(data[m_channel][i]);
        m_SE1s[m_time] = m_calcSE1s.SE(data[m_channel][i]);
        m_SE1sSE[m_time] = m_calcSE1sSE.SE(m_SE1s[m_time]);
        TData avgSE125 = m_calcSE125.SE(data[m_channel][i]);
        m_avgSE125[m_time] = m_calcAvgSE125.Add(avgSE125);
        TData seOfSE = m_calcSE500SE.SE(m_SE500[m_time]);
        m_SE500SE[m_time] = seOfSE;
        if (m_time > 0)
        {
          if ((seOfSE > 4 * m_avgSE2[m_time - 1]) && (m_avgSE2[m_time - 1] > 2)) // Param.PRECISION 
          {
            if (!m_packStarted)
            {
              lock (m_packList)
              {
                m_packList.Add(m_time);
                m_packList.Add(0);
              }
            }
            m_packStarted = true;
            m_avgSE2[m_time] = m_avgSE2[m_time - 1];
          }
          else
          {
            m_avgSE2[m_time] = m_calcSE2Avg.Add(seOfSE);
          }

          if (seOfSE < m_avgSE2[m_time - 1])
          {
            if (m_packStarted == true)
            {
              lock (m_packList)
              {
                int packLength = m_packList[m_packList.Count - 1] - m_packList[m_packList.Count - 2];
                if (packLength < MIN_PACK_LENGTH)
                {
                  m_packList.RemoveAt(m_packList.Count - 1);
                  m_packList.RemoveAt(m_packList.Count - 1);
                }
              }
            }
            m_packStarted = false;
          }

          if (m_packStarted)
          {
            lock (m_packList) m_packList[m_packList.Count - 1] = m_time;
          }

        }
        else
        {
          m_avgSE2[m_time] = m_calcSE2Avg.Add(m_SE500SE[m_time]);
        }

        m_time++;
      }
      
      int width = panel_Data.Width;
      int height = panel_Data.Height;

      int dataStart = m_time - dataLength;
      UpdateStoredPoints(dataStart);

      double dataPointsPerPixel = (double)STAT_BUF_LEN / width;
      Rectangle updateRegion = new Rectangle((int)(dataStart / dataPointsPerPixel), 0, (int)(dataLength / dataPointsPerPixel) + 1, height);
      panel_Data.Invalidate(updateRegion);
      panel_Stat1.Invalidate();
    }

    private void ArtifCallback(List<TAbsStimIndex> stimul)
    {
      
    }
    
    private void panel_Data_Paint(object sender, PaintEventArgs e)
    {
      if (m_time == 0) return;
      int dataLength = m_time;

      int width = panel_Data.Width;
      int height = panel_Data.Height;
      double dataPointsPerPixel = (double)STAT_BUF_LEN / width;

      if (m_prevDataPanelWidth != width)
      {
        m_dataPoints = new Point[width * 2];
        m_packPoints = new Point[width * 2];
        m_packPoints2 = new Point[width * 2];
        m_SE500Points = new Point[width * 2];
        m_SE1sPoints = new Point[width * 2];
        m_SE500SEPoints = new Point[width * 2];
        m_SE1sSEPoints = new Point[width * 2];
        m_avgSE2Points = new Point[width * 2];
        m_avgSE125Points = new Point[width * 2];
        UpdateStoredPoints(0);

        m_prevDataPanelWidth = width;
        panel_Data.Refresh();
        m_refreshPanel2 = true;
        panel_Stat1.Refresh();
        return;
      }

      int start = e.ClipRectangle.Left;
      int drawLength = Math.Min(e.ClipRectangle.Width, (int)(dataLength / dataPointsPerPixel) - start + 1);
      if (drawLength <= 0) return;

      Point[] dataPoints = new Point[drawLength * 2];
      Point[] se500Points = new Point[drawLength * 2];
      Point[] se1sPoints = new Point[drawLength * 2];
      Point[] se500SEPoints = new Point[drawLength * 2];
      Point[] se1sSEPoints = new Point[drawLength * 2];
      Point[] avgSE2Points = new Point[drawLength * 2];
      Point[] avgSE125Points = new Point[drawLength * 2];
      Array.Copy(m_dataPoints, start * 2, dataPoints, 0, drawLength * 2);
      Array.Copy(m_SE500Points, start * 2, se500Points, 0, drawLength * 2);
      Array.Copy(m_SE1sPoints, start * 2, se1sPoints, 0, drawLength * 2);
      Array.Copy(m_SE500SEPoints, start * 2, se500SEPoints, 0, drawLength * 2);
      Array.Copy(m_SE1sSEPoints, start * 2, se1sSEPoints, 0, drawLength * 2);
      Array.Copy(m_avgSE2Points, start * 2, avgSE2Points, 0, drawLength * 2);
      Array.Copy(m_avgSE125Points, start * 2, avgSE125Points, 0, drawLength * 2);

      Pen penB = new Pen(Color.Blue, 1);
      e.Graphics.DrawLines(penB, dataPoints);
      Pen penP = new Pen(Color.Purple, 1);
      e.Graphics.DrawLines(penP, m_packPoints);
      Pen penR = new Pen(Color.Red, 1);
      e.Graphics.DrawLines(penR, se500Points);
      Pen penK = new Pen(Color.Black, 1);
      e.Graphics.DrawLine(penK, 0, 4 * height / 5, width, 4 * height / 5);
      e.Graphics.DrawLines(penK, se1sPoints);
      Pen penY = new Pen(Color.Yellow, 1);
      e.Graphics.DrawLines(penY, avgSE2Points);
      Pen penG = new Pen(Color.Green, 1);
      e.Graphics.DrawLines(penG, se500SEPoints);
      Pen penLG = new Pen(Color.LightGreen, 1);
      e.Graphics.DrawLines(penLG, se1sSEPoints);
      Pen penCh = new Pen(Color.Chocolate, 1);
      e.Graphics.DrawLines(penCh, avgSE125Points);
    }

    private void panel_Stat1_Paint(object sender, PaintEventArgs e)
    {
      if (m_time == 0) return;
      int dataLength = m_time;

      int width = panel_Data.Width;
      int height = panel_Data.Height;
      double dataPointsPerPixel = (double)STAT_BUF_LEN / width;

      if (m_refreshPanel2)
      {
        UpdateStoredPoints2(0);
        m_refreshPanel2 = false;
        panel_Stat1.Refresh();
        return;
      }

      int start = e.ClipRectangle.Left;
      int drawLength = Math.Min(e.ClipRectangle.Width, (int)(dataLength / dataPointsPerPixel) - start); // +1
      if (drawLength <= 0) return;

      Point[] dataPoints = new Point[drawLength * 2];
      Array.Copy(m_dataPoints, start * 2, dataPoints, 0, drawLength * 2);

      Pen penB = new Pen(Color.Blue, 1);
      e.Graphics.DrawLines(penB, dataPoints);
      Pen penP = new Pen(Color.Purple, 1);
      e.Graphics.DrawLines(penP, m_packPoints2);
    }

    private void UpdateStoredPoints(int dataStart)
    {
      if (m_time <= dataStart) return;
      List<int> packList;
      lock (m_packList) packList = new List<int>(m_packList);

      int dataLength = m_time - dataStart;
      double dataPointsPerPixel = (double)STAT_BUF_LEN / panel_Data.Width;
      int start = (int)(dataStart / dataPointsPerPixel);

      m_drawWidth = (dataLength * panel_Data.Width) / STAT_BUF_LEN + 1;
      int height = panel_Data.Height;

      while ((packList.Count > 0) && (packList[0] < dataStart))
      {
        if (packList[1] > dataStart) break;
        packList.RemoveAt(0);
        packList.RemoveAt(0);
      }
      int x = start;
      bool startNewPoint = (m_dataPoints[2 * x].X == 0);
      for (int i = 0; i < dataLength; ++i)
      {
        int xNew = (int)((dataStart + i) / dataPointsPerPixel);
        int yData = (int)(height / 2 - m_data[dataStart + i] / 15);
        int ySE500 = (int)(height / 2 - m_SE500[dataStart + i] / 5);
        int ySE1s = (int)(height / 2 - m_SE1s[dataStart + i] / 5);
        int ySE125Avg = (int)(height / 2 - m_avgSE125[dataStart + i] / 5);
        int ySE500SE = (int)(4 * height / 5 - m_SE500SE[dataStart + i] / 2);
        int ySE1sSE = (int)(4 * height / 5 - (m_avgSE125[dataStart + i] - m_SE500[dataStart + i]) / 2);
        int ySE2Avg = (int)(4 * height / 7 - m_avgSE2[dataStart + i] / 2);
        if ((xNew > x) || startNewPoint)
        {
          x = xNew;
          startNewPoint = false;
          m_dataPoints[2 * x] = new Point(x, yData);
          m_dataPoints[2 * x + 1] = new Point(x, yData);
          m_SE500Points[2 * x] = new Point(x, ySE500);
          m_SE500Points[2 * x + 1] = new Point(x, ySE500);
          m_SE1sPoints[2 * x] = new Point(x, ySE1s);
          m_SE1sPoints[2 * x + 1] = new Point(x, ySE1s);
          m_SE500SEPoints[2 * x] = new Point(x, ySE500SE);
          m_SE500SEPoints[2 * x + 1] = new Point(x, ySE500SE);
          m_SE1sSEPoints[2 * x] = new Point(x, ySE1sSE);
          m_SE1sSEPoints[2 * x + 1] = new Point(x, ySE1sSE);
          m_avgSE2Points[2 * x] = new Point(x, ySE2Avg);
          m_avgSE2Points[2 * x + 1] = new Point(x, ySE2Avg);
          m_avgSE125Points[2 * x] = new Point(x, ySE125Avg);
          m_avgSE125Points[2 * x + 1] = new Point(x, ySE125Avg);
        }
        else
        {
          m_dataPoints[2 * x].Y = Math.Min(m_dataPoints[2 * x].Y, yData);
          m_dataPoints[2 * x + 1].Y = Math.Max(m_dataPoints[2 * x + 1].Y, yData);
          m_SE500Points[2 * x].Y = Math.Min(m_SE500Points[2 * x].Y, ySE500);
          m_SE500Points[2 * x + 1].Y = Math.Max(m_SE500Points[2 * x + 1].Y, ySE500);
          m_SE1sPoints[2 * x].Y = Math.Min(m_SE1sPoints[2 * x].Y, ySE1s);
          m_SE1sPoints[2 * x + 1].Y = Math.Max(m_SE1sPoints[2 * x + 1].Y, ySE1s);
          m_SE500SEPoints[2 * x].Y = Math.Min(m_SE500SEPoints[2 * x].Y, ySE500SE);
          m_SE500SEPoints[2 * x + 1].Y = Math.Max(m_SE500SEPoints[2 * x + 1].Y, ySE500SE);
          m_SE1sSEPoints[2 * x].Y = Math.Min(m_SE1sSEPoints[2 * x].Y, ySE1sSE);
          m_SE1sSEPoints[2 * x + 1].Y = Math.Max(m_SE1sSEPoints[2 * x + 1].Y, ySE1sSE);
          m_avgSE2Points[2 * x].Y = Math.Min(m_avgSE2Points[2 * x].Y, ySE2Avg);
          m_avgSE2Points[2 * x + 1].Y = Math.Max(m_avgSE2Points[2 * x + 1].Y, ySE2Avg);
          m_avgSE125Points[2 * x].Y = Math.Min(m_avgSE125Points[2 * x].Y, ySE125Avg);
          m_avgSE125Points[2 * x + 1].Y = Math.Max(m_avgSE125Points[2 * x + 1].Y, ySE125Avg);
        }
        if ((packList.Count > 0) && (dataStart + i >= packList[0]))
        {
          m_packPoints[2 * x] = m_dataPoints[2 * x];
          m_packPoints[2 * x + 1] = m_dataPoints[2 * x + 1];
          if (dataStart + i == packList[1])                       // EOP
          {
            packList.RemoveAt(0);
            packList.RemoveAt(0);
          }
        }
      }
    }

    private void UpdateStoredPoints2(int dataStart)
    {
      if (m_time <= dataStart) return;
      List<int> packList2;
      lock (m_packList2) packList2 = new List<int>(m_packList2);

      int dataLength = m_time - dataStart;
      double dataPointsPerPixel = (double)STAT_BUF_LEN / panel_Data.Width;
      int start = (int)(dataStart / dataPointsPerPixel);

      m_drawWidth = (dataLength * panel_Data.Width) / STAT_BUF_LEN + 1;
      int height = panel_Data.Height;

      while ((packList2.Count > 0) && (packList2[0] < dataStart))
      {
        if (packList2[1] > dataStart) break;
        packList2.RemoveAt(0);
        packList2.RemoveAt(0);
      }
      Point dataPoint0 = new Point();
      Point dataPoint1 = new Point();
      int x = start;
      for (int i = 0; i < dataLength; ++i)
      {
        int xNew = (int)((dataStart + i) / dataPointsPerPixel);
        int yData = (int)(height / 2 - m_data[dataStart + i] / 15);
        if (xNew > x)
        {
          x = xNew;
          dataPoint0 = new Point(x, yData);
          dataPoint1 = new Point(x, yData);
        }
        else
        {
          dataPoint0.Y = Math.Min(m_dataPoints[2 * x].Y, yData);
          dataPoint1.Y = Math.Max(m_dataPoints[2 * x + 1].Y, yData);
        }
        if ((packList2.Count > 0) && (dataStart + i >= packList2[0]))
        {
          m_packPoints2[2 * x] = dataPoint0;
          m_packPoints2[2 * x + 1] = dataPoint1;
          if (dataStart + i == packList2[1])                       // EOP
          {
            packList2.RemoveAt(0);
            packList2.RemoveAt(0);
          }
        }
      }
    }

    private void bt_Start_Click(object sender, EventArgs e)
    {
      m_time = 0;
      m_startTime = 0;
      m_packList.Clear();
      m_packList2.Clear();
      m_packPoints.PopulateArray<Point>(new Point());
      m_packPoints2.PopulateArray<Point>(new Point());
      m_packStarted = false;
      m_running = true;
      panel_Data.Refresh();
      panel_Stat1.Refresh();
    }

    private void bt_Stop_Click(object sender, EventArgs e)
    {
      m_running = false;
    }

    private void panel_Data_Resize(object sender, EventArgs e)
    {
      if (panel_Data.Width < m_prevDataPanelWidth)
      {
        panel_Data.Refresh();
        panel_Stat1.Refresh();
      }
    }

    private void StatForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      m_running = false;
      m_dataStream.RemoveDataConsumer(DataCallback);
    }

    private void cb_Channel_SelectedIndexChanged(object sender, EventArgs e)
    {
      m_channel = cb_Channel.SelectedIndex;
    }

  }
}
