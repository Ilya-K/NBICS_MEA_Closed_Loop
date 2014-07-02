using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MEAClosedLoop.Common;
namespace MEAClosedLoop
{
  public partial class FSelectExperiment : Form
  {
    public int SelectedID = -1;
    string connectonString;
    public FSelectExperiment(string ConnectionString)
    {
      connectonString = ConnectionString;
      InitializeComponent();
      using (ExpDataContext _db = new ExpDataContext(connectonString))
      {
        foreach(Experiment exp in _db.Experiments)
        {
          ExpTable.Rows.Add();
          ExpTable.Rows[ExpTable.Rows.Count - 1].Cells[0].Value = exp.id.ToString();
          ExpTable.Rows[ExpTable.Rows.Count - 1].Cells[1].Value = exp.About;
          ExpTable.Rows[ExpTable.Rows.Count - 1].Cells[2].Value = exp.Target;
          ExpTable.Rows[ExpTable.Rows.Count - 1].Cells[3].Value = exp.Author;
        }
      }
    }

    private void SelectButton_Click(object sender, EventArgs e)
    {
      if (ExpTable.SelectedRows.Count != 1)
      {
        MessageBox.Show("Select one row \n");
      }
      else
      {
        int.TryParse(ExpTable.SelectedRows[0].Cells[0].Value.ToString(), out SelectedID);
        this.Close();
      }
    }
  }
}
