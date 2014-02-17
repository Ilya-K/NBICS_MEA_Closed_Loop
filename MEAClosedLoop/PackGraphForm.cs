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
  using TPackMap = List<uint>;
  using TTime = UInt64;

  public partial class PackGraphForm : Form
  {
    TPackMap data;
   
    const int MAX_DETECTION_TIME = 200; //number of ms

    public PackGraphForm(List<int> channelList, Dictionary<int, bool[]> dict_bool_data, TTime data_start)
    {
      InitializeComponent();
      Panel[] channelPanels = new Panel[channelList.Count];

      int formWidth = this.Size.Width;
      int formHeight = this.Size.Height;
      int panelWidth = formWidth / 8 - 2;
      int panelHeight = formHeight / 8 - 2;

      PackGraph dataGenerator = new PackGraph();
      List<TPack> bool_data = new List<TPack>(); //TODO: generate correct data in bool format

      //getting filtered data
      


        //filling bool_data
      int i;
      TPack tmp = new TPack();
      foreach (int channel in channelList)
      {
        tmp.stimTime = data_start;
        for (i = 0; i < dict_bool_data.Count; i++)
        {
          tmp.data.Add(dict_bool_data[channel][i]);
        }
      }


        data = dataGenerator.ProcessPackStat(bool_data); //тут всё переделывать >_<
        //все данные по 1 каналу -> 1 кусок данных по всем каналам

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
          int dataLength = data.Count();
          Point[] points = new Point[dataLength];
          for (int i = 0; i < dataLength; i++)
          {
            points[i] = new Point(i * width / dataLength, /*(int)(height - i)*/ (int)data[i]);
          }
          Pen pen = new Pen(Color.Blue, 1);
          e.Graphics.DrawLines(pen, points);
       
      }
    }
  }
}
