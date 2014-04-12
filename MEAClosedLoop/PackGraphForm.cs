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
  public delegate void DelegateSetStatButtonText(string text);
  public delegate void DelegateResetProgressBar(int val = 0);

  public partial class PackGraphForm : Form
  {
    uint[] data;
    const int SUBPANEL_SPACE_X = 2;
    const int SUBPANEL_SPACE_Y = 2;

    const int SPB_REFRESH_COOLDOWN = 5; //in seconds
    const int DEFAULT_POINT_COUNT = 100; //in each pannel
    const int MIN_DRAWABLE_POINT_COUNT = 3;

    public event LoadSelectionDelegate loadSelection;
    public System.Timers.Timer statCalcTimer;
    public System.Timers.Timer spb_timer;
    public event spbTimerDelegate ItsTimeToMoveSPB;
    public event StatFinishedDelegate statFinished;
    public event DelegateResetProgressBar ResetProgressBar;
    public event DelegateSetStatButtonText SetStatButtonText;

    const int MAX_DETECTION_TIME = 200; //number of ms
    PackGraph dataGenerator;
    CLoopController m_LoopCtrl;
    public event DelegateSetProgress spb_SetVal;

    private Point[][] pointsToDraw;
    int timeUnitSegment;
    Panel[] channelPanels;

    public PackGraphForm(List<int> channelList, CLoopController LoopCtrl)
    {
      //TODO: real-time getting data & list of stim indices
      InitializeComponent();
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      channelPanels = new Panel[channelList.Count];
      SetStatButtonText += RunStatButtonText;
      ResetProgressBar += spb_Set;

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

      data = null;

      pointsToDraw = new Point[channelList.Max() + 1][];
      timeUnitSegment = panelWidth;
      
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
        pointsToDraw[channel] = new Point[DEFAULT_POINT_COUNT];
        tmpPanel.Paint += channelPanel_Paint;
        tmpPanel.Name = elName.ToString();
        tmpPanel.Click += new EventHandler(tmpPanel_Click);
        this.Controls.Add(tmpPanel);
        channelPanels[channel] = tmpPanel;

      }

      
      this.Invalidate();

    }

    private void RunStatButtonText(string text)
    {
      RunStatButton.Text = text;
    }

    private void spb_Set(int val = 0)
    {
      StatProgressBar.Value = val;
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
      int debug = 0;

      int currentPanelIndex = MEA.EL_DECODE[Convert.ToInt32((sender as Panel).Name)];
      debug = 1;

      try
      {
        data = dataGenerator.PrepareData(currentPanelIndex, width, height); //TODO: check data preparation
        debug = 2;
        if (data != null)
        {
          if(data.Max() > 0){
          int dataLength = data.Count<uint>();
          if (dataLength > MIN_DRAWABLE_POINT_COUNT)
          {

            //scaling data
            double dataScale = (double)(height - 1) / data.Max();
            data.Select(dataPoint => dataPoint = (uint)(Math.Abs((double)dataPoint * dataScale)));
            debug = 3;

            //drawing data
            for (int i = 0; i < dataLength; i++)
            {
              pointsToDraw[currentPanelIndex][i] = new Point(i * width / dataLength, (data[i] < height) ? height - (int)data[i] : height);
            }
            debug = 4;
            Pen pen = new Pen(Color.DodgerBlue, 1);
            pointsToDraw[currentPanelIndex][dataLength - 1] = new Point(width, 0);
            e.Graphics.DrawLines(pen, pointsToDraw[currentPanelIndex]);

            //drawing scale
            using (SolidBrush br = new SolidBrush(Color.SpringGreen))
            {
              StringFormat sf = new StringFormat();
              sf.FormatFlags = StringFormatFlags.NoWrap;
              e.Graphics.DrawString("x" + Math.Round(dataScale, 2).ToString(), this.Font, br, new Point(10, 10), sf);
            }
          }
        }
        data = null;
      }
        }
      catch (Exception ex)
      {
        throw ex;
      }
      finally{
        ;
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
      dataGenerator.Reset();
      dataGenerator.totalTime = totalStatTime * Param.MS;
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
      StatEnd();
    }

    public void spb_update()
    {
      StatProgressBar.BeginInvoke(spb_SetVal, null, 1);
    }

    public void StopFreqStat()
    {
      m_LoopCtrl.OnPackFound -= dataGenerator.ProcessFreqStat;
      StatEnd();
    }
    private void StatEnd()
    {
      spb_timer.Stop();
      statCalcTimer.Stop();
      drawResult();
      RunStatButton.BeginInvoke(SetStatButtonText, "пересчитать");
      StatProgressBar.BeginInvoke(ResetProgressBar, 0);
      StatProgressBar.Invalidate();
      //TODO: allow result saving
      statFinished = null;
      MessageBox.Show("подсчёт завершён");
    }

    private void StatTimer(object o1, EventArgs e1)
    {
      statFinished();
    }
    private void spbTimerReset(object o1, EventArgs e1)
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
    private void drawResult()
    {
        dataGenerator.ProcessPackStat(timeUnitSegment);
        foreach (Panel p in channelPanels)
        {
          p.Controls.Clear();
          p.Invalidate();    
        }
    }
  }
}
