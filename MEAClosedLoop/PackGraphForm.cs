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
  public delegate void DelegateLockUI(bool lockflag);
  public enum OnOffSwitch
  {
    On,
    Off
  }

  public partial class PackGraphForm : Form //TODO: check sync
  {
    uint[] data;
    const int SUBPANEL_SPACE_X = 2;
    const int SUBPANEL_SPACE_Y = 2;

    const int SPB_REFRESH_COOLDOWN = 1; //in seconds
    const int DEFAULT_POINT_COUNT = 100; //in each pannel
    const int MIN_DRAWABLE_POINT_COUNT = 3;
    const int MAX_DATA_SCALE = 10;

    public event LoadSelectionDelegate loadSelection;
    public System.Timers.Timer statCalcTimer;
    public System.Timers.Timer spb_timer;
    public event spbTimerDelegate ItsTimeToMoveSPB;
    public event StatFinishedDelegate statFinished;
    public event DelegateResetProgressBar ResetProgressBar;
    public event DelegateSetStatButtonText SetStatButtonText;
    public event DelegateLockUI DoLockUI;


    const int MAX_DETECTION_TIME = 200; //number of ms
    PackGraph dataGenerator, comp_datagenerator;
    CLoopController m_LoopCtrl;
    public event DelegateSetProgress spb_SetVal;
    OnOffSwitch PackGraphState = OnOffSwitch.Off;

    private Point[][] pointsToDraw;
    int timeUnitSegment;
    Panel[] channelPanels;

    public bool compareMode;

    PackShapeForm PSF;
    
    private object lockSelIndex = new object();
    private int m_selectedIndex = 0;

    public PackGraphForm(List<int> channelList, CLoopController LoopCtrl)
    {
      InitializeComponent();
      this.FormBorderStyle = FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      channelPanels = new Panel[channelList.Count];
      SetStatButtonText += RunStatButtonText;
      ResetProgressBar += spb_Set;
      DoLockUI += LockUI;

      int formWidth = this.Size.Width;
      int formHeight = this.Size.Height;
      int panelWidth = formWidth / 8 - 2;
      int panelHeight = (formHeight - this.groupBox1.Height) / 8 - 2;

      dataGenerator = new PackGraph();
      m_LoopCtrl = LoopCtrl;
      PSF = new PackShapeForm(dataGenerator);

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
        int elName = MEA.IDX2NAME[channel];

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

      StatTypeListBox.SelectionMode = SelectionMode.One;
      this.Invalidate();

      compareMode = false;
    }

    public void EnableCompareMode()
    {
      compareMode = true;
      comp_datagenerator = new PackGraph();
      this.BeginInvoke(DoLockUI, true);
      PackGraphState = OnOffSwitch.Off;
    }

    private PackGraph CurrentDataGenerator(){
      return (compareMode) ? comp_datagenerator : dataGenerator;
    }

    private void RunStatButtonText(string text)
    {
      RunStatButton.Text = text;
    }

    private void spb_Set(int val = 0)
    {
      StatProgressBar.Value = val;
    }

    private void LockStatTypeListBox(bool lockval)
    {
    }

    private void LockMinCountBox(bool lockval)
    {
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

      int currentPanelIndex = MEA.NAME2IDX[Convert.ToInt32((sender as Panel).Name)];

      try
      {
        data = dataGenerator.PrepareData(currentPanelIndex, width, height);
        if (data != null)
        {
          if(data.Max() > 0){
          int dataLength = data.Count<uint>();
          if (dataLength > MIN_DRAWABLE_POINT_COUNT)
          {

            //scaling data
            double dataScale = (double)(height - 1) / data.Max();
            if(dataScale < MAX_DATA_SCALE && dataScale > 0)
              data.Select(dataPoint => dataPoint = (uint)(Math.Abs((double)dataPoint * dataScale)));

            //drawing data
            for (int i = 0; i < dataLength; i++)
            {
              pointsToDraw[currentPanelIndex][i] = new Point(i * width / dataLength, (data[i] < height) ? height - (int)data[i] : height);
            }
            Pen pen = new Pen(Color.DodgerBlue, 1);
            pointsToDraw[currentPanelIndex][dataLength - 1] = new Point(width, 0);
            e.Graphics.DrawLines(pen, pointsToDraw[currentPanelIndex]);

            #region drawing data comparison if needed
            if (compareMode)
            {
              data = comp_datagenerator.PrepareData(currentPanelIndex, width, height);
              if (data != null)
              {

                if (data.Max() > 0)
                {
                  int comp_dataLength = data.Count<uint>();
                  if (comp_dataLength > MIN_DRAWABLE_POINT_COUNT)
                  {

                    //scaling compared data
                    if (dataScale < MAX_DATA_SCALE && dataScale > 0)
                      data.Select(dataPoint => dataPoint = (uint)(Math.Abs((double)dataPoint * dataScale)));

                    //drawing compared data
                    pointsToDraw[currentPanelIndex].PopulateArray<Point>(new Point(0, 0));
                    for (int j = 0; j < comp_dataLength; j++)
                    {
                      pointsToDraw[currentPanelIndex][j] = new Point(j * width / dataLength, (data[j] < height) ? height - (int)data[j] : height);
                    }
                    Pen comp_pen = new Pen(Color.LightPink, 1);
                    pointsToDraw[currentPanelIndex][comp_dataLength - 1] = new Point(width, 0);
                    e.Graphics.DrawLines(comp_pen, pointsToDraw[currentPanelIndex]);
                  }
                }
              }
            }
            #endregion

            //drawing scale
            using (SolidBrush textBrush = new SolidBrush(Color.Green), backgroundBrush = new SolidBrush(Color.White))
            {
              StringFormat sf = new StringFormat();
              double roundedScale = Math.Round(dataScale, 2);
              string scaleString = ((roundedScale != 0) ? "x" +roundedScale.ToString() : ">0.01");
              sf.FormatFlags = StringFormatFlags.NoWrap;
              SizeF ScaleStringSize = new SizeF();
              ScaleStringSize = e.Graphics.MeasureString(scaleString, this.Font, width, sf);
              e.Graphics.FillRectangle(backgroundBrush, 10, 10, ScaleStringSize.Width, ScaleStringSize.Height);
              e.Graphics.DrawString(scaleString, this.Font, textBrush, new Point(10, 10), sf);
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
    }

    private void tmpPanel_Click(object sender, System.EventArgs e)
    {
      string elName = (sender as Panel).Name;
      PSF.Visible = false;
      PSF.SetChannel(MEA.NAME2IDX[Convert.ToInt32(elName)]);
      switch (PSF.ShowDialog()) {
        case System.Windows.Forms.DialogResult.OK:
          MessageBox.Show("канал выбран");
          loadSelection(MEA.NAME2IDX[Convert.ToInt32(elName)]);
          this.Hide();
          break;
        case System.Windows.Forms.DialogResult.Abort:
        break;
      }
    }

    private void PackGraphForm_Load(object sender, EventArgs e)
    {
    }

    private void RunStatButton_Click(object sender, EventArgs e)
    {
      if (PackGraphState == OnOffSwitch.Off)
      {
          PackGraphState = OnOffSwitch.On;
          RunStatButton.BeginInvoke(SetStatButtonText, "остановить");
          this.BeginInvoke(DoLockUI, true);
          ulong totalStatTime = (ulong)MinCountBox.Value * 60 * 1000; //in ms
          int spbRefreshCount = -1 + (int)MinCountBox.Value * 60 / SPB_REFRESH_COOLDOWN;
          ulong spbRefreshTime = totalStatTime * SPB_REFRESH_COOLDOWN / 60;
          this.CurrentDataGenerator().Reset();
          this.CurrentDataGenerator().totalTime = totalStatTime * Param.MS;

              m_LoopCtrl.OnPackFound += this.CurrentDataGenerator().ProcessStat;
              statFinished += StopStat;

          spb_timer.Interval = spbRefreshTime;
          StatProgressBar.Maximum = 59; //it's a kind of magic
          statCalcTimer.Interval = totalStatTime;
          spb_timer.Start();
          statCalcTimer.Start();
      }
      else
      { // OnOffSwitch.On:
          PackGraphState = OnOffSwitch.Off;
          RunStatButton.BeginInvoke(SetStatButtonText, "собрать статистику");
          statCalcTimer.Stop();
          spb_timer.Stop();
          try
          {
            statFinished.Invoke();
          }
          catch
          {
            MessageBox.Show("Предупреждение: сбор статистики остановлен");
          }
      }
    }

    private void LockUI(bool lockstate)
    {
      StatTypeListBox.Enabled = !lockstate;
      MinCountBox.Enabled = !lockstate;
      foreach (Panel p in this.channelPanels)
      {
        p.Enabled = !lockstate;
      }
    }

    public void StopStat()
    {
      m_LoopCtrl.OnPackFound -= this.CurrentDataGenerator().ProcessStat;
      StatEnd();
    }

    public void spb_update()
    {
      StatProgressBar.BeginInvoke(spb_SetVal, null, 1);
    }

    private void StatEnd()
    {
      spb_timer.Stop();
      statCalcTimer.Stop();
      this.BeginInvoke(DoLockUI, false);
      int selIndex;
      lock (lockSelIndex) selIndex = m_selectedIndex;
      drawResult(selIndex);  
      RunStatButton.BeginInvoke(SetStatButtonText, "пересчитать");
      StatProgressBar.BeginInvoke(ResetProgressBar, 0);
      StatProgressBar.Invalidate();
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
    private void drawResult(int statTypeIndex)
    {
        this.CurrentDataGenerator().ProcessPackStat(timeUnitSegment, statTypeIndex);
        foreach (Panel p in channelPanels)
        {
          p.Controls.Clear();
          p.Invalidate();    
        }
    }

    private void StatTypeListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      lock (lockSelIndex) m_selectedIndex = StatTypeListBox.SelectedIndex;
    }
   
  }
}
