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
    private int m_channel; // [TODO]: It's not a true channel name, but just an index in a Dic. Correct some day.
    private int m_prevDataPanelWidth;
    private volatile int m_time = 0;
    private volatile bool m_running = false;

    public StatForm(CFiltering fltStream)
    {
      InitializeComponent();

      m_data = new TData[STAT_BUF_LEN];
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
        m_data[m_time++] = data[m_channel][i];
      }

      int width = panel_Data.Width;
      int height = panel_Data.Height;
      double dotsPerPixel = (double)STAT_BUF_LEN / width;
      Rectangle updateRegion = new Rectangle((int)((m_time - dataLength) / dotsPerPixel), 0, (int)(dataLength / dotsPerPixel) + 1, height);
      panel_Data.Invalidate(updateRegion);
    }

    private void ArtifCallback(List<TStimIndex> stimul)
    {
      
    }
    
    private void panel_Data_Paint(object sender, PaintEventArgs e)
    {
      if (m_time == 0) return;
     
      int width = panel_Data.Width;
      int height = panel_Data.Height;
      
      int drawLength = e.ClipRectangle.Width;
      int start = e.ClipRectangle.Left;

      if (m_prevDataPanelWidth != width)
      {
        start = 0;
        drawLength = width;
        m_prevDataPanelWidth = width;
        panel_Data.Refresh();
        return;
      }

      int dataStart = (start * STAT_BUF_LEN) / width;

      if (dataStart > m_time) return;
      int dataLength = m_time - dataStart;
      dataLength = Math.Min(dataLength, (drawLength * STAT_BUF_LEN) / width);
      drawLength = Math.Min(drawLength, (dataLength * width) / STAT_BUF_LEN + 1);
      Point[] points = new Point[drawLength * 2];

      int x = -1;
      for (int i = 0; i < dataLength; ++i)
      {
        int xNew = (i * width) / STAT_BUF_LEN;
        int y = (int)(height / 2 - m_data[dataStart + i] / 20);
        if (xNew > x)
        {
          x = xNew;
          points[2 * x] = new Point(start + x*3, y);
          points[2 * x + 1] = new Point(start + x*3, y);
        }
        else
        {
          points[2 * x].Y = Math.Min(points[2 * x].Y, y);
          points[2 * x + 1].Y = Math.Max(points[2 * x + 1].Y, y);
        }
      }

      Pen pen = new Pen(Color.Blue, 1);
      e.Graphics.DrawLines(pen, points);
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

  }
}
