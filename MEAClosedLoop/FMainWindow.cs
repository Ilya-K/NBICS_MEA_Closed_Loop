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
      Form1 form1 = new Form1();
      form1.MdiParent = this;
      form1.Show();
    }
  }
}
