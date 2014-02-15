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
  public partial class DebugForm : Form
  {
    public DebugForm(int var1)
    {
      InitializeComponent();
      label1.Text = var1.ToString();
    }
  }
}
