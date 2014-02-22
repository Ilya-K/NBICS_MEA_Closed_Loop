using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MEAClosedLoop.Common
{
  public partial class StimParams : Form
  {
    public bool DoUpdateParams = false;
    public int Time; // in ms
    public StimParams()
    {
      InitializeComponent();
    }
    public StimParams(int _time)
    {
      InitializeComponent();
      WaitPackTime.Text = _time.ToString();
    }

    private void ok_Click(object sender, EventArgs e)
    {
      Time = int.Parse(WaitPackTime.Text);
      DoUpdateParams = true;
      this.Close();
    }
    private void accept_Click(object sender, EventArgs e)
    {
      Time = int.Parse(WaitPackTime.Text);
      DoUpdateParams = true;
    }
    private void cancel_Click(object sender, EventArgs e)
    {
      DoUpdateParams = false;
      this.Close();
    }
  }
}
