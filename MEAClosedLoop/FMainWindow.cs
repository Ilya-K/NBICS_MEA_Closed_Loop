using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MEAClosedLoop.Common;
using MEAClosedLoop.UI_Forms;
namespace MEAClosedLoop
{
  public partial class FMainWindow : Form
  {
    public Form1 MainManager;

    private FRecorder _Recorder;
    private CFiltering _Filter;
    private CLoopController _LoopController;
    private CEvokedBurstDetector _EvokedBurstDetector;
    public CDataFlowController dataFlowController = new CDataFlowController();

    public FDataSourceControl dataSourceControl;

    public FRecorder Recorder
    {
      get
      {
        return _Recorder;
      }
      set
      {
        // необходимо проверить список
        _Recorder = value;
      }
    }

    public CLoopController LoopController
    {
      get
      {
        return _LoopController;
      }
      set
      {
        value.OnPackFound += dataFlowController.RecieveBurstData;
        _LoopController = value;
      }
    }

    public CFiltering Filter
    {
      get
      {
        return _Filter;
      }
      set
      {
        value.AddDataConsumer(dataFlowController.RecieveFltData);
        value.AddStimulConsumer(dataFlowController.RecieveStim);
        _Filter = value;
      }
    }

    public CEvokedBurstDetector EvokedBurstDetector
    {
      get
      {
        return _EvokedBurstDetector;
      }
      set
      {
        dataFlowController.AddConsumer(value);
        _EvokedBurstDetector = value;
        _EvokedBurstDetector.OnEvPackFound += dataFlowController.RecieveEvPack;
      }
    }

    public FMainWindow()
    {
      InitializeComponent();
      MainManager = new Form1();
      MainManager.MdiParent = this;
      MainManager.MainWindow = this;

      dataSourceControl = new FDataSourceControl(dataFlowController, this);
      dataSourceControl.MdiParent = this;
      
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

    #region Recorder
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
    #endregion

    private void FMainWindow_Load(object sender, EventArgs e)
    {
      //открываем панель главного менеджера
      MainManager.StartPosition = FormStartPosition.Manual;
      MainManager.Location = new System.Drawing.Point(0, 0);
      MainManager.Show();
      // панель контроля источников данных
      dataSourceControl.StartPosition = FormStartPosition.Manual;
      dataSourceControl.Location = new System.Drawing.Point(0, MainManager.Height + 10);
      dataSourceControl.Show();
    }

    private void findGoodChannels_Click(object sender, EventArgs e)
    {
      FChSorter ChSorterForm = new FChSorter();
      ChSorterForm.MdiParent = this;
      dataFlowController.AddConsumer(ChSorterForm);
      ChSorterForm.Show();
    }

    private void dataControlToolStripMenuItem_Click(object sender, EventArgs e)
    {

    }
    
    private void burstDescMethodsDevelopToolStripMenuItem_Click(object sender, EventArgs e)
    {
      FBurstDescription form = new FBurstDescription();
      form.MdiParent = this;
      dataFlowController.AddConsumer(form);
      form.Show();
    }

    private void singleChannelViewToolStripMenuItem_Click(object sender, EventArgs e)
    {
      FSingleChDisplay form = new FSingleChDisplay();
      //form.MdiParent = this;
      dataFlowController.AddConsumer(form);
      form.Show();
    }

    private void multiChannelToolStripMenuItem_Click(object sender, EventArgs e)
    {
      FMultiChDisplay form = new FMultiChDisplay();
      dataFlowController.AddConsumer(form);
      form.Show();
    }
  }

}
