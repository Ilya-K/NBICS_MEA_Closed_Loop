using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace MEAClosedLoop
{
  using TRawDataPacket = Dictionary<int, ushort[]>;

  public partial class FormCalibrate : Form
  {
    private Thread m_t;
    private volatile bool m_stop = false;
    private CInputStream m_inputStream;
    private CStimulator m_stimulator;
    private List<CInputStream.ConsumerDelegate> m_backList;
    private Stopwatch m_sw1;
    
    // [DEBUG]
    private bool first = true;
    private volatile int counter = 0;
    delegate void SetTextCallback(Control ctl, string text);


    public FormCalibrate(CInputStream inputStream, CStimulator stimulator)
    {
      if (inputStream == null) throw new ArgumentNullException("inputStream");
      if (stimulator == null) throw new ArgumentNullException("stimulator");

      InitializeComponent();
      m_inputStream = inputStream;
      m_inputStream.Stop();
      m_inputStream.Stop();
      m_backList = m_inputStream.ConsumerList;
      m_inputStream.ConsumerList = new List<CInputStream.ConsumerDelegate>();
      
      m_stimulator = stimulator;
      m_stimulator.BackupCurrentShape();

      m_sw1 = new Stopwatch();

      m_t = new Thread(new ThreadStart(Calibrate));
      m_t.Start();
    }

    private void Calibrate()
    {
      // Test of stimulator On/Off latency
      StimulatorOnOffTest();

      m_inputStream.ConsumerList.Add(ReceiveData);
      m_stimulator.DownloadDefaultShape(1, 1, 0, 10000);

      m_sw1.Restart();

      m_inputStream.Start();
      m_stimulator.Start();

      while (!m_stop)
      {
        Thread.Sleep(100);
        label3.Invalidate();
        ++counter; 
      }

      m_inputStream.Stop();
      m_stimulator.Stop();
      m_inputStream.ConsumerList = m_backList;
    }

    private void StimulatorOnOffTest()
    {
      m_stimulator.DownloadLadderShape(1, 1, 0, 10000);

      // Skip first pair, just warming up
      m_stimulator.Start();
      m_stimulator.Stop();

      long sw1 = 0, sw2 = 0;
      int N = 20;

      for (int i = 0; i < N; ++i)
      {
        m_sw1.Restart();
        m_stimulator.Start();
        sw1 += m_sw1.ElapsedTicks;
        m_stimulator.Stop();
        m_sw1.Stop();
        sw2 += m_sw1.ElapsedTicks;
        Thread.Sleep(1);
      }
      sw2 -= sw1;


      SetText(lStimStartLatency, ((double)sw1 / N / Stopwatch.Frequency).ToString());
      SetText(lStimStopLatency, ((double)sw2 / N / Stopwatch.Frequency).ToString());
    }

    private void ReceiveData(TRawDataPacket data)
    {
      m_sw1.Stop();
      if (!m_stop)
      {
        if (first)
        {
          SetText(label1, m_sw1.ElapsedMilliseconds.ToString());
          first = false;
        }
        SetText(label2, m_sw1.ElapsedMilliseconds.ToString());
        m_sw1.Restart();
      }
    }

    private void FormCalibrate_FormClosing(object sender, FormClosingEventArgs e)
    {
      m_stop = true;
      if (m_t.ThreadState == System.Threading.ThreadState.Suspended) m_t.Resume();
      m_t.Join(2000);
      if (m_t.IsAlive)        // Something has gone wrong. Try to terminate thread and restore m_inputStream.ConsumerList
      {
        m_t.Abort();
        m_t.Join(10000);
        if (m_inputStream != null)
        {
          m_inputStream.Stop();
          m_stimulator.Stop();
          m_inputStream.ConsumerList = m_backList;
        }
      }
      m_t = null;
      m_stimulator.RestoreShape();
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

    private void buttonStartStop_Click(object sender, EventArgs e)
    {
      bool running = (m_t.ThreadState != System.Threading.ThreadState.Suspended);
      if (running)
      {
        m_inputStream.Stop();
        m_stimulator.Stop();
        m_t.Suspend();
      }
      else
      {
        m_t.Resume();
        m_inputStream.Start();
        m_stimulator.Start();
      }
      ((Button)sender).Text = running ? "Start" : "Stop";

    }

    private void label3_Paint(object sender, PaintEventArgs e)
    {
      label3.Text = counter.ToString();
    }
  }
}
