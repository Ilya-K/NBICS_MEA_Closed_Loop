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
  using TData = System.Double;
  using TFltDataPacket = Dictionary<int, System.Double[]>;
  using TStimIndex = System.Int16;

  public partial class StatForm : Form
  {
    private const int DEFAULT_CH = 5;
    private const int STAT_BUF_LEN = 30 * 1000 * Param.MS;
    private CFiltering m_dataStream;
    private TData[] m_data;
    private TData[] m_SE500;
    private TData[] m_sumSE;
    private Point[] m_dataPoints;
    private Point[] m_SE500Points;
    private Point[] m_sumSEPoints;
    private CCalcExpWndSE m_calcSE500 = new CCalcExpWndSE(167);

    private int m_channel; // [TODO]: It's not a true channel name, but just an index in a Dic. Correct some day.
    private int m_prevDataPanelWidth;
    private volatile int m_time = 0;
    private int m_drawWidth = 0;
    private volatile bool m_running = false;


    public StatForm(CFiltering fltStream)
    {
      InitializeComponent();

      m_data = new TData[STAT_BUF_LEN];
      m_SE500 = new TData[STAT_BUF_LEN];
      m_dataPoints = new Point[panel_Data.Width * 2];
      m_SE500Points = new Point[panel_Data.Width * 2];
      m_sumSEPoints = new Point[panel_Data.Width * 2];

      m_dataStream = fltStream;

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

    private void DataCallback(TFltDataPacket data)
    {
      if (!m_running) return;

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
        m_time++;
      }


      int width = panel_Data.Width;
      int height = panel_Data.Height;

      int dataStart = m_time - dataLength;
      UpdateStoredPoints(dataStart);

      double dataPointsPerPixel = (double)STAT_BUF_LEN / width;
      Rectangle updateRegion = new Rectangle((int)(dataStart / dataPointsPerPixel), 0, (int)(dataLength / dataPointsPerPixel) + 1, height);
      panel_Data.Invalidate(updateRegion);
    }

    private void ArtifCallback(List<TStimIndex> stimul)
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
        m_SE500Points = new Point[width * 2];
        m_sumSEPoints = new Point[width * 2];
        UpdateStoredPoints(0);

        m_prevDataPanelWidth = width;
        panel_Data.Refresh();
        return;
      }

      int start = e.ClipRectangle.Left;
      int drawLength = Math.Min(e.ClipRectangle.Width, (int)(dataLength / dataPointsPerPixel) - start + 1);
      Point[] dataPoints = new Point[drawLength * 2];
      Point[] se500Points = new Point[drawLength * 2];
      Array.Copy(m_dataPoints, start * 2, dataPoints, 0, drawLength * 2);
      Array.Copy(m_SE500Points, start * 2, se500Points, 0, drawLength * 2);

      Pen penB = new Pen(Color.Blue, 1);
      e.Graphics.DrawLines(penB, dataPoints);
      Pen penR = new Pen(Color.Red, 1);
      e.Graphics.DrawLines(penR, se500Points);
    }

    private void panel_Stat1_Paint(object sender, PaintEventArgs e)
    {

    }

    private void UpdateStoredPoints(int dataStart)
    {
      if (m_time <= dataStart) return;

      int dataLength = m_time - dataStart;
      double dataPointsPerPixel = (double)STAT_BUF_LEN / panel_Data.Width;
      int start = (int)(dataStart / dataPointsPerPixel);

      m_drawWidth = (dataLength * panel_Data.Width) / STAT_BUF_LEN + 1;
      int height = panel_Data.Height;

      int x = start;
      for (int i = 0; i < dataLength; ++i)
      {
        int xNew = (int)((dataStart + i) / dataPointsPerPixel);
        int yData = (int)(height / 2 - m_data[dataStart + i] / 20);
        int ySE500 = (int)(height / 2 - m_SE500[dataStart + i] / 20);
        if (xNew > x)
        {
          x = xNew;
          m_dataPoints[2 * x] = new Point(x, yData);
          m_dataPoints[2 * x + 1] = new Point(x, yData);
          m_SE500Points[2 * x] = new Point(x, ySE500);
          m_SE500Points[2 * x + 1] = new Point(x, ySE500);

        }
        else
        {
          m_dataPoints[2 * x].Y = Math.Min(m_dataPoints[2 * x].Y, yData);
          m_dataPoints[2 * x + 1].Y = Math.Max(m_dataPoints[2 * x + 1].Y, yData);
          m_SE500Points[2 * x].Y = Math.Min(m_dataPoints[2 * x].Y, ySE500);
          m_SE500Points[2 * x + 1].Y = Math.Max(m_dataPoints[2 * x + 1].Y, ySE500);
        }
      }
    }

    private void bt_Start_Click(object sender, EventArgs e)
    {
      m_time = 0;
      m_running = true;
      panel_Data.Refresh();
    }

    private void bt_Stop_Click(object sender, EventArgs e)
    {
      m_running = false;
    }

    private void panel_Data_Resize(object sender, EventArgs e)
    {
      if (panel_Data.Width < m_prevDataPanelWidth) panel_Data.Refresh();
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
