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
  public partial class FCreateMeasure : Form
  {
    public MeasureManager measure = null;
    public FCreateMeasure()
    {
      InitializeComponent();
    }

    private void CreateButton_Click(object sender, EventArgs e)
    {
      measure = new MeasureManager();
      measure.CreationTime = DateTime.Now;
      measure.About = AboutTextBox.Text;
      this.Close();
    }

    private void CancellButton_Click(object sender, EventArgs e)
    {

      this.Close();
    }
  }
}
