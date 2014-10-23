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
  public partial class FDataSourceControl : Form
  {
    private FMainWindow mainWindow;
    private CDataFlowController dataFlowController;
    private Timer RefreshTimer;
    public FDataSourceControl(CDataFlowController _dataFlowController, FMainWindow _mainWindow)
    {
      InitializeComponent();
      mainWindow = _mainWindow;
      dataFlowController = _dataFlowController;
      RefreshTimer = new Timer();
      RefreshTimer.Interval = 200;
      RefreshTimer.Tick +=RefreshTimer_Tick;
      RefreshTimer.Start();
    }

    private void RefreshTimer_Tick(object sender, EventArgs e)
    {
      this.RefreshStatus();
    }

    public void RefreshStatus()
    {
      if (mainWindow.LoopController != null)
      {
        StatusBurst.Text = "Active";
      }
      else
      {
        StatusBurst.Text = "off";
      }
      if (mainWindow.Filter != null)
      {
        StatusMea.Text = "Active";
        StatusMeaFlt.Text = "Active";
        StatusStim.Text = "Active";
      }
      else
      {
        StatusMea.Text = "off";
        StatusMeaFlt.Text = "off";
        StatusStim.Text = "off";
      }
      if (mainWindow.EvokedBurstDetector != null)
      {
        StatusEvBurst.Text = "Active";
      }
      else
      {
        StatusEvBurst.Text = "off";
      }

    }

    private void RunEvBusrtButton_Click(object sender, EventArgs e)
    {
      //костыль, нужно сделать метод в главной форме
      mainWindow.EvokedBurstDetector = new CEvokedBurstDetector();
    }
  }
}
