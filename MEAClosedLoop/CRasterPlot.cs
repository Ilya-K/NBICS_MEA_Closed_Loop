using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Common;

namespace MEAClosedLoop
{
  class CRasterPlot
  {
    private const int SAMPLING_F = Param.DAQ_FREQ;
    private Timer m_refreshTimer;
    private Panel m_panel;
    private Queue<Spike> m_data;
    private int m_binSize;
    private int m_length;
    private UInt64 m_rightTimestamp;
    private Int64 m_leftTimestamp;
    private Spike m_currentBin;


    /// <summary>
    /// Creates Raster Plot for Spike Events
    /// </summary>
    /// <param name="panel">System.Windows.Forms.Panel to draw plot</param>
    /// <param name="length">Length of plot in bins</param>
    /// <param name="binSize">Bin size in 1/Sampling_Frequency</param>
    /// <param name="refreshRate">Refresh Rate in Hz</param>
    public CRasterPlot(Panel panel, int length, int binSize, int refreshRate)
    {
      m_panel = panel;
      m_binSize = binSize;
      m_length = length;
      m_rightTimestamp = 0;
      m_leftTimestamp = -m_binSize * m_length;
      m_currentBin = new Spike(0, 0);
      m_data = new Queue<Spike>(length);
      //m_data.Enqueue(m_currentBin);
      m_panel.Paint += new PaintEventHandler(OnPanelPaint);
      m_refreshTimer = new Timer();
      m_refreshTimer.Interval = 1000 / refreshRate;
      m_refreshTimer.Tick += new EventHandler(RefreshPanel);
      m_refreshTimer.Start();
    }

    public void AddData(Spike newData)
    {
      if (newData == null) return;
      if (newData.timestamp > m_currentBin.timestamp + (UInt64)m_binSize)
      {
        lock (m_data)
        {
          if (m_data.Count >= m_length)
          {
            m_data.Dequeue();
          }
          m_currentBin = newData;
          m_data.Enqueue(m_currentBin);

          Spike test = m_data.Peek();
          if ((int)test.timestamp < m_leftTimestamp)
          {
            m_data.Dequeue();
          }
        }
      }
      else
      {
        m_currentBin.meaBits |= newData.meaBits;
      }
      
      m_rightTimestamp = newData.timestamp;
      m_leftTimestamp = (Int64)newData.timestamp - m_binSize * m_length;
    }

    private void OnPanelPaint(object sender, PaintEventArgs e)
    {
      Pen pen = new Pen(Color.Black, 1);
//      GraphicsPath gp = new GraphicsPath();

      lock (m_data)
      {
        foreach (Spike spike in m_data)
        {
          if ((int)spike.timestamp < m_leftTimestamp) continue;
          int x = (int)((Int64)spike.timestamp - m_leftTimestamp) / m_binSize;
          while (x >= m_length)
          {
            m_leftTimestamp += m_binSize;
            x = (int)((Int64)spike.timestamp - m_leftTimestamp) / m_binSize;
          }

          for (int i = 0; i < 60; ++i)
          {
            if ((spike.meaBits & (1UL << i)) != 0)
            {
              //gp.AddLine(x, 3 * i, x, 3 * i + 1);
              e.Graphics.DrawLine(pen, x, 3 * i, x, 3 * i + 1);
            }
          }
        }
      }
      //e.Graphics.DrawPath(pen, gp);
    }

    private void RefreshPanel(object o, EventArgs e)
    {
      m_panel.Invalidate();
      m_panel.Refresh();
      int delta = SAMPLING_F * m_refreshTimer.Interval / 1000;
      m_rightTimestamp += (UInt64)delta;
      m_leftTimestamp += delta - 100;
      // m_rightTimestamp += (UInt64)m_binSize;
      // m_leftTimestamp += m_binSize - 100;
    }
  }
}
