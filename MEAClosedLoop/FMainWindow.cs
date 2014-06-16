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
  public partial class FMainWindow : Form
  {
    public Form1 MainManager;
    public FRecorder Recorder;
    public FMainWindow()
    {
      InitializeComponent();
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void newManagerToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (MainManager == null || MainManager.IsDisposed)
      {
        MainManager = new Form1();
        MainManager.MdiParent = this;
        MainManager.Show();
      }
      else
      {
        MainManager.WindowState = FormWindowState.Normal;
      }
    }

    private void recorderToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (MainManager == null || MainManager.IsDisposed)
      {
        MessageBox.Show("Ошибка, менеджер закрыт или не запущен");
        return;
      }
      if (Recorder != null && Recorder.IsDisposed)
      {
        Recorder = null;
      }
      if (Recorder == null)
      {
        Recorder = new FRecorder();
        if (MainManager.m_salpaFilter != null)
        {
          MainManager.m_salpaFilter.AddDataConsumer(Recorder.RecieveFltData);
          MainManager.m_salpaFilter.AddStimulConsumer(Recorder.RecieveStimData);
          Recorder.StimDataConnected = true;
          Recorder.RawDataConnected = true;
        }
        else
        {
          switch (MessageBox.Show("Не запущен фильтр данных, \nзапись данных невозможна", "предупреждение", MessageBoxButtons.OKCancel))
          {
            case System.Windows.Forms.DialogResult.OK:
              break;
            case System.Windows.Forms.DialogResult.Cancel:
              Recorder = null;
              return;
          }
        }
        if (MainManager.m_closedLoop != null)
        {
          MainManager.m_closedLoop.OnPackFound += Recorder.RecievePackData;
          Recorder.PackDataConnected = true;
        }
        else
        {
          switch (MessageBox.Show("Не запущена петля эксперимента, \nзапись данных о пачках невозможна", "предупреждение", MessageBoxButtons.OKCancel))
          {
            case System.Windows.Forms.DialogResult.OK:
              break;
            case System.Windows.Forms.DialogResult.Cancel:
              Recorder = null;
              return;
          }
        }
        Recorder.Show();
      }
      else
      {
        Recorder.Show();
      }
    }
    
  }
}
