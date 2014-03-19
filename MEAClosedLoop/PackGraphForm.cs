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
  using TPackMap = List<int>;
  using TTime = UInt64;
  public delegate void LoadSelectionDelegate(int sel);
  public delegate void StatFinishedDelegate();
  public delegate void spbTimerDelegate();

  public partial class PackGraphForm : Form
  {
    Queue<uint> data;
    const int SUBPANEL_SPACE_X = 2;
    const int SUBPANEL_SPACE_Y = 2;

    const int SPB_REFRESH_COOLDOWN = 5; //in seconds

    public event LoadSelectionDelegate loadSelection;

    public System.Timers.Timer statCalcTimer;
    public System.Timers.Timer spb_timer;
    public event spbTimerDelegate ItsTimeToMoveSPB;
    public event StatFinishedDelegate statFinished;

    const int MAX_DETECTION_TIME = 200; //number of ms
    PackGraph dataGenerator;
    CLoopController m_LoopCtrl;
    public event DelegateSetProgress spb_SetVal;

    private Point[][] pointsToDraw;

    public PackGraphForm(List<int> channelList, CLoopController LoopCtrl)
    {
      //TODO: real-time getting data & list of stim indices
      InitializeComponent();
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      Panel[] channelPanels = new Panel[channelList.Count];

      int formWidth = this.Size.Width;
      int formHeight = this.Size.Height;
      int panelWidth = formWidth / 8 - 2;
      int panelHeight = (formHeight - this.groupBox1.Height) / 8 - 2;

      dataGenerator = new PackGraph();
      m_LoopCtrl = LoopCtrl;

      statCalcTimer = new System.Timers.Timer();
      statCalcTimer.Elapsed += StatTimer;
      spb_SetVal += spb_UpdateProgressBar;
      
      spb_timer = new System.Timers.Timer();
      ItsTimeToMoveSPB += spb_update;
      spb_timer.Elapsed += spbTimerReset;

      data = new Queue<uint>();
      /*List<TPack> bool_data = new List<TPack>(); //TODO: generate correct data in bool format

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
      */
      foreach (int channel in channelList)
      {
        int elName = MEA.AR_DECODE[channel];

        int x = elName / 10 - 1;
        int y = elName % 10 - 1;

        Panel tmpPanel = new Panel();
        tmpPanel.Location = new Point(x * panelWidth + SUBPANEL_SPACE_X, y * panelHeight + SUBPANEL_SPACE_Y + this.groupBox1.Height);
        tmpPanel.Size = new System.Drawing.Size(panelWidth, panelHeight);
        tmpPanel.BorderStyle = BorderStyle.FixedSingle;
        tmpPanel.BackColor = Color.White;
        tmpPanel.Paint += channelPanel_Paint;
        tmpPanel.Name = elName.ToString();
        tmpPanel.Click += new EventHandler(tmpPanel_Click);
        this.Controls.Add(tmpPanel);
        channelPanels[channel] = tmpPanel;

      }

      dataGenerator.ProcessPackStat(panelWidth);
      //this.Invalidate();

    }

    void spb_UpdateProgressBar(object sender, int val)
    {
      if (StatProgressBar.Value < StatProgressBar.Maximum )
        StatProgressBar.Value += val;
    }

    private void channelPanel_Paint(object sender, PaintEventArgs e)
    {
      int width = ((Panel)sender).Width;
      int height = ((Panel)sender).Height;

      int currentPanelIndex = MEA.EL_DECODE[Convert.ToInt32((sender as Panel).Name)];
      data = dataGenerator.PrepareData(currentPanelIndex);

      //lock (m_chDataLock1) <- TODO
      {
        // [TODO] указать реальный размер данных
        if (data != null)
        {
          int dataLength = pointsToDraw[currentPanelIndex].Length;
          for (int i = 0; i < dataLength; i++)
          {
            pointsToDraw[currentPanelIndex][i] = new Point(i * width / dataLength, (int)data.Dequeue());
          }
          Pen pen = new Pen(Color.Blue, 1);
          if (pointsToDraw.Count() > 1)
          {
            e.Graphics.DrawLines(pen, pointsToDraw[currentPanelIndex]);
          }
        }
      }
    }

    private void tmpPanel_Click(object sender, System.EventArgs e)
    {
      string elName = (sender as Panel).Name;
      MessageBox.Show("канал выбран");
      loadSelection(MEA.EL_DECODE[Convert.ToInt32(elName)]);
      this.Close();
    }

    private void PackGraphForm_Load(object sender, EventArgs e)
    {

    }

    private void RunStatButton_Click(object sender, EventArgs e)
    {
      int statType = StatTypeListBox.SelectedIndex;
      ulong totalStatTime = (ulong)MinCountBox.Value * 60 * 1000; //in ms
      int spbRefreshCount = -1 + (int)MinCountBox.Value * 60 / SPB_REFRESH_COOLDOWN;
      ulong spbRefreshTime = totalStatTime * SPB_REFRESH_COOLDOWN / 60;

      //StatProgressBar.Maximum = totalStatTime;
      //dataGenerator.CollectStat(totalStatTime);

      switch (statType)
      {
        case 0:
          m_LoopCtrl.OnPackFound += dataGenerator.ProcessAmpStat;
          statFinished += StopAmpStat;
          break;
        case 1:
          m_LoopCtrl.OnPackFound += dataGenerator.ProcessFreqStat;
          statFinished += StopFreqStat;
          break;
      }
      spb_timer.Interval = spbRefreshTime;
      StatProgressBar.Maximum = spbRefreshCount;
      statCalcTimer.Interval = totalStatTime;
      spb_timer.Start();
      statCalcTimer.Start();

    }
    public void StopAmpStat()
    {

      m_LoopCtrl.OnPackFound -= dataGenerator.ProcessAmpStat;
      spb_timer.Stop();
      this.Invalidate();
      MessageBox.Show("подсчёт завершён");
    }

    public void spb_update()
    {
      StatProgressBar.BeginInvoke(spb_SetVal, null, 1);
    }

    public void StopFreqStat()
    {
      m_LoopCtrl.OnPackFound -= dataGenerator.ProcessFreqStat;
      spb_timer.Stop();
      this.Invalidate();

      MessageBox.Show("подсчёт завершён");
    }
    private void StatTimer(object o1, EventArgs e1)
    {
      statFinished();
    }
    private void spbTimerReset(object o1, EventArgs e1) //TODO: redraw progress bar after completion
    {
      try
      {
        ItsTimeToMoveSPB();
      }
      catch
      {
        return;
      }
      if (StatProgressBar.Value < StatProgressBar.Maximum)
      {
        spb_timer.Start();
      }
    }
  }
}
