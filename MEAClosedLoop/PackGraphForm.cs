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
  public partial class PackGraphForm : Form
  {
    public PackGraphForm(List<int> channelList)
    {
      InitializeComponent();
      Panel[] channelPanels = new Panel[channelList.Count];

      int formWidth = this.Size.Width;
      int formHeight = this.Size.Height;
      int panelWidth = formWidth / 8 - 2;
      int panelHeight = formHeight / 8 - 2;

      foreach (int channel in channelList)
      {
        int elName = MEA.AR_DECODE[channel];

        int x = elName / 10 - 1;
        int y = elName % 10 - 1;

        Panel tmpPanel = new Panel();
        tmpPanel.Location = new Point(x * panelWidth + 2, y * panelHeight + 2);
        tmpPanel.Size = new System.Drawing.Size(panelWidth, panelHeight);
        tmpPanel.BorderStyle = BorderStyle.FixedSingle;
        tmpPanel.BackColor = Color.White;
        tmpPanel.Paint += channelPanel_Paint;
        this.Controls.Add(tmpPanel);

       
        channelPanels[channel] = tmpPanel;

      }

      this.Invalidate();
    }

    private void channelPanel_Paint(object sender, PaintEventArgs e)
    {
      int width = ((Panel)sender).Width;
      int height = ((Panel)sender).Height;



//      lock (m_chDataLock1)
      {
          // [TODO] указать реальный размер данных
          int dataLength = 20;
          Point[] points = new Point[dataLength];
          for (int i = 0; i < dataLength; i++)
          {
            points[i] = new Point(i * width / dataLength, (int)(height - i));//linear function for testing
          }
          Pen pen = new Pen(Color.Blue, 1);
          e.Graphics.DrawLines(pen, points);
       
      }
    }
  }
}
