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
  public partial class Recorder : Form
  {
    private Experiment currentExperiment = null;
    private string ConnectionString = "";
    public Recorder()
    {
      InitializeComponent();
    }

    private void label1_Click(object sender, EventArgs e)
    {

    }

    private void progress_Scroll(object sender, EventArgs e)
    {

    }
    private void CreateExp_Click(object sender, EventArgs e)
    {
      FormCreateExpOpt m_expcreate = new FormCreateExpOpt();
      m_expcreate.ShowDialog();
    }
    private void OpenDB_Click(object sender, EventArgs e)
    {
      InfoBar.Items[1].Text = "connected";
      
    }

    private void OpenExp_Click(object sender, EventArgs e)
    {
      using (ExpDataContext _db = new ExpDataContext())
      {
        if (_db.Experiments.Count() == 0)
        {
          DialogResult result = MessageBox.Show("no experimnts inside DB\n Would you like to create some?", "error", MessageBoxButtons.OKCancel);
          switch (result)
          {
            case System.Windows.Forms.DialogResult.Cancel:
              InfoBar.Items[3].Text = "disconnected";
              break;
            case System.Windows.Forms.DialogResult.OK:
              FormCreateExpOpt CreateExpForm = new FormCreateExpOpt();
              Experiment experiment_to_add = new Experiment();
              CreateExpForm.Show();
              experiment_to_add.About = CreateExpForm.ExpName.Text;
              experiment_to_add.Author = CreateExpForm.AuthorName.Text;
              experiment_to_add.CreationTime = DateTime.Now;
              experiment_to_add.Target = CreateExpForm.ExpTarget.Text;
              _db.Experiments.Add(experiment_to_add);
              _db.SaveChanges();
              currentExperiment = experiment_to_add;
              InfoBar.Items[3].Text = "Exp ok, id = " + currentExperiment.id.ToString();

              break;
          }
        }
        else
        {
          SelectExperimentForm selectExp = new SelectExperimentForm();
          selectExp.ShowDialog();
          currentExperiment = _db.Experiments.Where(x => x.id == selectExp.SelectedID).FirstOrDefault();
          InfoBar.Items[3].Text = "Exp ok, id = " + currentExperiment.id.ToString();
        }
      }
    }
  }
}
