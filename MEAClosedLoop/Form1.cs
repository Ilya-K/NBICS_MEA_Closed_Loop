using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Neurorighter;
using UsbNetDll;
using Common;

namespace MEAClosedLoop
{
  using TFltDataPacket = Dictionary<int, System.Double[]>;
  using TData = System.Double;
  using TTime = UInt64;

  public partial class Form1 : Form
  {
    TData[] m_channelData1;
    TData[] m_channelData2;
    private object m_chDataLock1 = new object();
    private object m_chDataLock2 = new object();
    int m_viewChannel1;
    int m_viewChannel2;
    CInputStream m_inputStream;
    CFiltering m_salpaFilter;
    CSpikeDetector m_spikeDetector;
    CRasterPlot m_rasterPlotter;
    CStimulator m_stimulator;
    private volatile bool m_killDataLoop;
    List<int> m_channelList;
    Thread m_dataLoopThread;
    private CMcsUsbListNet m_usbDAQList = new CMcsUsbListNet();
    private CMcsUsbListNet m_usbSTGList = new CMcsUsbListNet();
    private int m_selectedDAQ = -1;
    private int m_selectedStim = -1;
    private bool m_DAQConfigured = false;
    private CLoopController m_closedLoop = null;
    private string m_fileOpened = "";
    private int m_fileIdx = -1;

    // [DEBUG]
    private DateTime m_prevTime = DateTime.Now;
    delegate void AddTextCallback(string text);
    delegate void SetTextCallback(Control ctl, string text);
    private CCalcExpWndSE m_se1 = new CCalcExpWndSE(167); // 3*tau = 20ms (500 samples)
    private CCalcExpWndSE m_seLongTerm1 = new CCalcExpWndSE(16667); // 3*tau = 2s (50000 samples)
    private CCalcExpWndSE m_se2 = new CCalcExpWndSE(167); // 3*tau = 20ms (500 samples)
    private CCalcExpWndSE m_seLongTerm2 = new CCalcExpWndSE(16667); // 3*tau = 2s (50000 samples)
    private TData m_integral1 = 0;
    private TData m_hpf = 0;
    private TData[] m_integral2 = new TData[3000];
    private CMovingSum m_ms = new CMovingSum(250);
    private CExpAvg m_expAvg = new CExpAvg(167);
    private CExpAvg m_expAvgCorr1 = new CExpAvg(167);
    private TData prev2;

    public Form1()
    {
      InitializeComponent();
      prev2 = 0;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      // Look for devices
      comboBox_DAQs_Click(null, null);
      comboBox_Stimulators_Click(null, null);
      SetDefaultChannels();
    }

    private void DataLoop()
    {
      do
      {
        m_rasterPlotter.AddData(m_spikeDetector.WaitData());
      } while (!m_killDataLoop);
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      m_killDataLoop = true;
      if (m_inputStream != null) m_inputStream.Kill();
    }

    void PeekData(TFltDataPacket data)
    {
      lock (m_chDataLock1)
      {
        m_channelData1 = data[m_viewChannel1];
      }
      panel1.Invalidate();
      lock (m_chDataLock2)
      {
        m_channelData2 = data[m_viewChannel2];
      }
      panel2.Invalidate();

      // [DEBUG]
      //label_refreshRate.Text = (1000.0 / (DateTime.Now - m_prevTime).Milliseconds).ToString();
      //SetText(label_refreshRate, m_bandpassFilter.Test().ToString());       
      AddText((1000 / ((DateTime.Now - m_prevTime).Milliseconds + 1)).ToString() + "; ");
      SetText(label_time, (m_inputStream.TimeStamp / (double)Param.DAQ_FREQ).ToString("F1"));

      m_prevTime = DateTime.Now;
    }

    #region Data Display
    private void panel1_Paint(object sender, PaintEventArgs e)
    {
      int width = panel1.Width;
      int height = panel1.Height;
      double max = double.MinValue;
      double min = double.MaxValue;

      lock (m_chDataLock1)
      {
        if (m_channelData1 != null)
        {
          TData[] se = new TData[m_channelData1.Length];
          TData[] seLT = new TData[m_channelData1.Length];
          TData[] hpf = new TData[m_channelData1.Length];
          TData[] ms = new TData[m_channelData1.Length];
          TData[] me = new TData[m_channelData1.Length];
          TData THRESH1 = 0.7;

          for (int i = 0; i < m_channelData1.Length; i++)
          {
            se[i] = m_se1.SE(m_channelData1[i]);
            seLT[i] = m_seLongTerm1.SE(m_channelData1[i]);
            ms[i] = m_ms.Add((se[i] - seLT[i]) / 50.0);
            me[i] = m_expAvg.Add((se[i] - seLT[i]) * 5.0);

            //m_integral1 = m_integral1 + (se[i] - seLT[i]) / 100;
            if (i > 0)
            {
              TData diff = me[i] - me[i - 1];
              if (Math.Abs(diff) > THRESH1)
              {
                hpf[i] = hpf[i - 1] + diff;
              }
              else
              {
                hpf[i] = hpf[i-1] * 0.98;
              }
              m_hpf = hpf[i];
            }
            else
            {
              hpf[0] = m_hpf;
            }

            if (m_channelData1[i] > max)
            {
              max = m_channelData1[i];
            }
            if (m_channelData1[i] < min)
            {
              min = m_channelData1[i];
            }
          }
          double delta = max - min;
          double shift = 0;
          if (delta < 200)
          {
            delta = 200;
            shift = (delta - (max - min)) / 2;
          }
          Point[] points = new Point[m_channelData1.Length];
          for (int i = 0; i < m_channelData1.Length; i++)
          {
            points[i] = new Point(i * width / m_channelData1.Length, (int)(height - (m_channelData1[i] - min + shift + 1) * height / (delta + 2)));
          }
          Pen pen = new Pen(Color.Blue, 1);
          e.Graphics.DrawLines(pen, points);

          // [DEBUG]
          Point[] se_points = new Point[se.Length];
          Point[] se_pointsLT = new Point[seLT.Length];
          Point[] integral_points = new Point[se.Length];
          Point[] ms_points = new Point[se.Length];
          Point[] me_points = new Point[se.Length];
          Point[] hpf_points = new Point[se.Length];

          for (int i = 0; i < se.Length; i++)
          {
            se_points[i] = new Point(i * width / se.Length, (int)(height - (se[i] - min + shift + 1) * height / (delta + 2)));
            se_pointsLT[i] = new Point(i * width / seLT.Length, (int)(height - (seLT[i] - min + shift + 1) * height / (delta + 2)));
            integral_points[i] = new Point(i * width / se.Length, (int)(height - (m_integral1 - min + shift + 1) * height / (delta + 2)));
            ms_points[i] = new Point(i * width / se.Length, (int)(height - (ms[i] - min + shift + 1) * height / (delta + 2)));
            me_points[i] = new Point(i * width / se.Length, (int)(height - (me[i] - min + shift + 1) * height / (delta + 2)));
            hpf_points[i] = new Point(i * width / se.Length, (int)(height - (hpf[i] - min + shift + 1) * height / (delta + 2)));
          }
          pen = new Pen(Color.Red, 1);
          e.Graphics.DrawLines(pen, se_points);
          pen = new Pen(Color.Green, 1);
          e.Graphics.DrawLines(pen, se_pointsLT);
          pen = new Pen(Color.Black, 1);
          e.Graphics.DrawLines(pen, integral_points);
          pen = new Pen(Color.Yellow, 1);
          e.Graphics.DrawLines(pen, me_points);
          pen = new Pen(Color.YellowGreen, 1);
          e.Graphics.DrawLines(pen, ms_points);
          pen = new Pen(Color.Wheat, 1);
          e.Graphics.DrawLines(pen, hpf_points);
          // [/Debug]

          labelAmpl1.Text = (max - min).ToString();
        }
      }
    }

    private void panel2_Paint(object sender, PaintEventArgs e)
    {
      int width = panel2.Width;
      int height = panel2.Height;
      double max = double.MinValue;
      double min = double.MaxValue;
      lock (m_chDataLock2)
      {
        if (m_channelData2 != null)
        {
          TData[] se = new TData[m_channelData2.Length];
          TData[] seLT = new TData[m_channelData2.Length];
          TData[] corr1 = new TData[m_channelData2.Length];

          for (int i = 0; i < m_channelData2.Length; i++)
          {
            se[i] = m_se2.SE(m_channelData2[i]);
            seLT[i] = m_seLongTerm2.SE(m_channelData2[i]);

            if (i == 0)
            {
              corr1[i] = m_expAvgCorr1.Add(m_channelData2[i] - prev2);
            }
            else
            {
              corr1[i] = m_expAvgCorr1.Add(m_channelData2[i] - m_channelData2[i - 1]);
            }



            if (m_channelData2[i] > max)
            {
              max = m_channelData2[i];
            }
            if (m_channelData2[i] < min)
            {
              min = m_channelData2[i];
            }
          }
          prev2 = m_channelData2[m_channelData2.Length];

          double delta = max - min;
          double shift = 0;
          if (delta < 200)
          {
            delta = 200;
            shift = (delta - (max - min)) / 2;
          }
          Point[] points = new Point[m_channelData2.Length];
          for (int i = 0; i < m_channelData2.Length; i++)
          {
            points[i] = new Point(i * width / m_channelData2.Length, (int)(height - (m_channelData2[i] - min + shift + 1) * height / (delta + 2)));
          }
          Pen pen = new Pen(Color.Blue, 1);
          e.Graphics.DrawLines(pen, points);

          // [DEBUG]
          Point[] se_points = new Point[se.Length];
          Point[] se_pointsLT = new Point[seLT.Length];
          for (int i = 0; i < se.Length; i++)
          {
            se_points[i] = new Point(i * width / se.Length, (int)(height - (se[i] - min + shift + 1) * height / (delta + 2)));
            se_pointsLT[i] = new Point(i * width / seLT.Length, (int)(height - (seLT[i] - min + shift + 1) * height / (delta + 2)));
          }
          pen = new Pen(Color.Red, 1);
          e.Graphics.DrawLines(pen, se_points);
          pen = new Pen(Color.Green, 1);
          e.Graphics.DrawLines(pen, se_pointsLT);
          // [/Debug]

          labelAmpl2.Text = (max - min).ToString();
        }
      }
    }
    #endregion

    private void EnsureDAQIsConfigured()
    {
      // Check if all necessary components of DAQ have been already created
      if (!m_DAQConfigured)
      {
        // Configure Input Stream and filters here
        if (m_inputStream == null)
        {
          if (m_selectedDAQ == m_fileIdx)
          {
            m_inputStream = new CInputStream(m_fileOpened, m_channelList, 2500);
          }
          else
          {
            m_inputStream = new CInputStream(m_usbDAQList, 0, m_channelList, 2500);
          }
        }

        // (int)SpikeFiltOrder.Value, 25000, Convert.ToDouble(SpikeLowCut.Value), Convert.ToDouble(SpikeHighCut.Value), DATA_BUF_LEN
        BFParams parBF = new BFParams(2, Param.DAQ_FREQ, 150.0, 2000.0, Param.DAQ_FREQ / 10); // [TODO] Eliminate data buffer length

        // [TODO] Get rid of thresholds here. Should be calculated in SALPA dynamically
        int[] thresholds = new int[60];
        for (int i = 0; i < 60; i++)
        {
          thresholds[i] = 1000 * 3;
        }
        // length_sams [75], asym_sams [10], blank_sams [75], ahead_sams [5], forcepeg_sams [10], thresholds[]
        SALPAParams parSALPA = new SALPAParams(35, 10, 35, 5, 10, thresholds);
        //m_bandpassFilter = new CFiltering(m_inputStream, null, null);

        // [TODO] Get parameters from the UI and save them in Settings
        CStimDetector m_stimDetector = new CStimDetector(15, 20, 35, 150);

        m_salpaFilter = new CFiltering(m_inputStream, m_stimDetector, parSALPA, null);
        //m_bandpassFilter = new CFiltering(m_inputStream, null, parBF);
        //m_salpaFilter.OnDataAvailable = PeekData;
        m_salpaFilter.AddDataConsumer(PeekData);
        m_spikeDetector = new CSpikeDetector(m_salpaFilter, -4.9);
        m_rasterPlotter = new CRasterPlot(m_panelSpikeRaster, 200, Param.DAQ_FREQ / 10, 2);

        m_DAQConfigured = true;
      }
    }

    private void buttonStartDAQ_Click(object sender, EventArgs e)
    {
      EnsureDAQIsConfigured();

      // Check if DataLoop Thread has been alredy created
      if (m_dataLoopThread == null)
      {
        m_dataLoopThread = new Thread(new ThreadStart(DataLoop));
        m_killDataLoop = false;
        m_dataLoopThread.Start();
      }

      comboBox_DAQs.Enabled = false;
      m_inputStream.Start();
      buttonStop.Enabled = true;
      buttonStartDAQ.Enabled = false;
      if (checkBox_Manual.Checked) m_inputStream.Pause();
    }

    private void buttonStop_Click(object sender, EventArgs e)
    {
      m_inputStream.Stop();
      buttonStartDAQ.Enabled = true;
      buttonClosedLoop.Enabled = true;
      comboBox_DAQs.Enabled = true;
    }

    private void button_Next_Click(object sender, EventArgs e)
    {
      if (checkBox_Manual.Checked)
      {
        if (m_inputStream != null) m_inputStream.Next();
      }
    }

    private void checkBox_Manual_CheckedChanged(object sender, EventArgs e)
    {
      if (m_inputStream != null)
      {
        if (checkBox_Manual.Checked)
        {
          m_inputStream.Pause();
          button_Next.Enabled = true;
        }
        else
        {
          m_inputStream.Resume();
          button_Next.Enabled = false;
        }
      }
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
      m_viewChannel1 = (int)((ComboBox)sender).SelectedItem;
    }

    private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
    {
      m_viewChannel2 = (int)((ComboBox)sender).SelectedItem;
    }

    private void buttonOpen_Click(object sender, EventArgs e)
    {
      OpenFileDialog ofd = new OpenFileDialog();
      // string fileName = @"C:\Work\NBIC\Data\29.11.11-3022_cfos0006_header.raw";
      // string fileName = @"D:\MC_Rack_Data\Export\22.11.11-balb-STS0006.raw";

      //ofd.InitialDirectory = @"C:\Work\NBIC\Data";
      ofd.InitialDirectory = @"D:\MC_Rack_Data\Export";
      // ofd.FileName = @"C:\Work\NBIC\Data\29.11.11-3022_cfos0006_header.raw";
      // ofd.FileName = @"22.11.11-balb-STS0006.raw";
      ofd.FileName = @"25_09_2012_3033 no stim skip100s_h.raw";
      //ofd.FileName = @"27_09_2012_3028 HFS0001_skip215s.raw";

      ofd.SupportMultiDottedExtensions = true;

      if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        try
        {
          if (m_inputStream != null) m_inputStream.Kill();
          // m_inputStream = new CInputStream(ofd.FileName, new List<int>(new int[]{0, 1, 2, 3, 4, 5, 6, 10, 12, 15, 17, 20, 23, 25, 27, 30, 35, 40, 50, 55, 59}), 2500);
          m_inputStream = new CInputStream(ofd.FileName, m_channelList, 2500);
          //m_inputStream = new CInputStream(m_usbDAQList, 0, m_channelList, 2500);

          
          m_fileOpened = ofd.FileName;
          m_selectedDAQ = comboBox_DAQs.Items.Add(Path.GetFileNameWithoutExtension(m_fileOpened));
          comboBox_DAQs.SelectedIndex = m_selectedDAQ;
          m_fileIdx = m_selectedDAQ;
          m_DAQConfigured = false;
          buttonStartDAQ.Enabled = true;
          //buttonStop.Enabled = true;
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
          m_fileOpened = "";
          m_fileIdx = -1;
        }
      }
    }

    private void SetDefaultChannels()
    {
      // m_channelList = new List<int>(new int[] { 0, 1, 3, 5, 10, 12, 15, 20, 25, 30, 35, 40, 45, 50, 55, 59 });
      // m_channelList = new List<int>(new int[] { 0, 1, 3 });
      m_channelList = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59 });
      comboBox1.Items.Clear();
      comboBox1.Items.AddRange(Array.ConvertAll(m_channelList.ToArray(), ch => (object)ch));
      comboBox1.SelectedIndex = 0;
      comboBox2.Items.Clear();
      comboBox2.Items.AddRange(Array.ConvertAll(m_channelList.ToArray(), ch => (object)ch));
      comboBox2.SelectedIndex = 1;
    }

    private void comboBox_DAQs_Click(object sender, EventArgs e)
    {
      m_DAQConfigured = false;
      if (m_inputStream != null)
      {
        m_inputStream.Kill();
        m_inputStream = null;
      }
      comboBox_DAQs.Items.Clear();
      m_usbDAQList.Initialize(DeviceEnumNet.MCS_MEA_DEVICE);
      for (uint i = 0; i < m_usbDAQList.GetDeviceNumber(); i++)
      {
        comboBox_DAQs.Items.Add(m_usbDAQList.GetDeviceName(i) + " / " + m_usbDAQList.GetSerialNumber(i));
      }
      if (m_fileOpened != "")
      {
        m_fileIdx = comboBox_DAQs.Items.Add(Path.GetFileNameWithoutExtension(m_fileOpened));
      }
      if (comboBox_DAQs.Items.Count > 0)
      {
        comboBox_DAQs.SelectedIndex = ((m_selectedDAQ >= 0) && (m_selectedDAQ < comboBox_DAQs.Items.Count)) ? m_selectedDAQ : 0;
        buttonStartDAQ.Enabled = true;
        //buttonStop.Enabled = true;
      }
    }

    private void comboBox_Stimulators_Click(object sender, EventArgs e)
    {
      comboBox_Stimulators.Items.Clear();
      m_usbSTGList.Initialize(DeviceEnumNet.MCS_STG_DEVICE);
      for (uint i = 0; i < m_usbSTGList.GetDeviceNumber(); i++)
      {
        comboBox_Stimulators.Items.Add(m_usbSTGList.GetDeviceName(i) + " / " + m_usbSTGList.GetSerialNumber(i));
      }
      if (comboBox_Stimulators.Items.Count > 0)
      {
        comboBox_Stimulators.SelectedIndex = ((m_selectedStim >= 0) && (m_selectedStim < comboBox_Stimulators.Items.Count)) ? m_selectedStim : 0;
        //btConnect.Enabled = true;
      }

    }

    private void comboBox_DAQs_SelectedIndexChanged(object sender, EventArgs e)
    {
      m_selectedDAQ = comboBox_DAQs.SelectedIndex;
      /*
      int sel = comboBox_DAQs.SelectedIndex;
      if (sel != m_selectedDAQ)
      {
        if (m_inputStream != null)
        {
          m_inputStream.Kill();
          m_inputStream = null;
        }
        m_selectedDAQ = sel;
      }
      */ 

      // If a DAQ device has been selected, enable the sampling buttons
      buttonStartDAQ.Enabled = (m_selectedDAQ >= 0);
      buttonStop.Enabled = false;
    }

    private void comboBox_Stimulators_SelectedIndexChanged(object sender, EventArgs e)
    {
      int sel = comboBox_Stimulators.SelectedIndex;
      if (sel != m_selectedStim)
      {
        if (m_stimulator != null)
        {
          m_stimulator = null;
        }
        m_selectedStim = sel;
      }
    }

    private void buttonCalibrate_Click(object sender, EventArgs e)
    {
      if ((m_selectedDAQ < 0) || (m_selectedStim < 0))
      {
        //MessageBox.Show("Some of the devices are not configured properly!", "Error");
        //return;
      }

      if (m_inputStream == null)
      {
        // Configure Input Stream here
        m_channelList = new List<int>(new int[] { 0, 1, 3 });
        m_inputStream = new CInputStream(m_usbDAQList, (uint)m_selectedDAQ, m_channelList, 2500);
      }
      
      if (m_stimulator == null)
      {
        // Configure Stimulator here
        m_stimulator = new CStimulator(m_usbSTGList, (uint)m_selectedStim);
      }

      try
      {
        FormCalibrate formCalib = new FormCalibrate(m_inputStream, m_stimulator);
        formCalib.ShowDialog();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void buttonClosedLoop_Click(object sender, EventArgs e)
    {
      buttonStop.Enabled = true;
      if (m_closedLoop == null)
      {
        EnsureDAQIsConfigured();

        if (m_stimulator == null)
        {
          // [TODO] Configure Stimulator
          // [DEBUG]
          // Fake stimulator
          m_stimulator = new CStimulator();
          // [/DEBUG] 
        }

        m_closedLoop = new CLoopController(m_inputStream, m_salpaFilter, m_stimulator);
      }

      m_inputStream.Start();
      buttonClosedLoop.Enabled = false;
      if (checkBox_Manual.Checked) m_inputStream.Pause();

      //buttonClosedLoop.Text = 
    }

    // [DEBUG]
    private void AddText(string text)
    {
      // InvokeRequired required compares the thread ID of the
      // calling thread to the thread ID of the creating thread.
      // If these threads are different, it returns true.
      if (this.textBox_DeviceInfo.InvokeRequired)
      {
        this.Invoke(new AddTextCallback(AddText), new object[] { text });
      }
      else
      {
        this.textBox_DeviceInfo.Text += text;
      }
    }

    // [DEBUG]
    private void SetText(Control ctl, string text)
    {
      // InvokeRequired required compares the thread ID of the
      // calling thread to the thread ID of the creating thread.
      // If these threads are different, it returns true.
      if (ctl.InvokeRequired)
      {
        try
        {
          this.Invoke(new SetTextCallback(SetText), new object[] { ctl, text });
        }
        catch (ObjectDisposedException ex) { };
      }
      else
      {
        ctl.Text = text;
      }
    }

    private void button_integral0_Click(object sender, EventArgs e)
    {
      m_integral1 = 0;
    }

    Dictionary<int, bool[]> GetBoolData()
    {
      Dictionary<int, bool[]> output = new Dictionary<int, bool[]>();
      //TODO: make proper output
      return output;
    }

    private void buttonFindWindow_Click(object sender, EventArgs e)
    {
      TTime start_data = 0; //TODO: get time from CFiltering
      PackGraphForm formShowWindows = new PackGraphForm(m_channelList, GetBoolData(), start_data);
      formShowWindows.Show();
    }

  }
}
