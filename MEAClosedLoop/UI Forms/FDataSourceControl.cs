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
    public FDataSourceControl(CDataFlowController _dataFlowController, FMainWindow _mainWindow)
    {
      InitializeComponent();
      mainWindow = _mainWindow;
      dataFlowController = _dataFlowController;

    }
    public void Refresh()
    {
      if (mainWindow.LoopController != null)
      {
        StatusBurst.Text = "Active";
      }
      else
      {
      }
      if (mainWindow.Filter != null)
      {
      }

    }

    private void RunEvBusrtButton_Click(object sender, EventArgs e)
    {

    }
  }
}
