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
  #region Definitions
  using TData = System.Double;
  using TTime = System.UInt64;
  using TStimIndex = System.Int16;
  using TAbsStimIndex = System.UInt64;
  using TRawData = UInt16;
  using TRawDataPacket = Dictionary<int, ushort[]>;
  using TFltDataPacket = Dictionary<int, System.Double[]>;
  using TFltData = System.Double;
  #endregion
  public partial class FRecorder : Form
  {
    private Experiment currentRecordExperiment = null;
    private Experiment currentPlayExperiment = null;
    private MeasureManager currentRecordMeasure = null;
    private MeasureManager currentPlayMeasure = null;
    private TTime MeasureLength = 0;
    private string ConnectionString = "";
    private bool DoRecieveData = false;
    public FRecorder()
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
      FCreateExpOpt CreateExpForm = new FCreateExpOpt();
      Experiment experiment_to_add = new Experiment();
      CreateExpForm.ShowDialog();
      using (ExpDataContext _db = new ExpDataContext())
      {
        experiment_to_add.About = CreateExpForm.ExpName.Text;
        experiment_to_add.Author = CreateExpForm.AuthorName.Text;
        experiment_to_add.CreationTime = DateTime.Now;
        experiment_to_add.Target = CreateExpForm.ExpTarget.Text;
        _db.Experiments.Add(experiment_to_add);
        _db.SaveChanges();
        //InfoBar.Items[3].Text = "Exp created, id = " + experiment_to_add.id.ToString();
      }
      DialogResult result = MessageBox.Show(
        "Exp created succesfull. \n Press yes to load it\n or not to do nth",
        "Experiment creation",
        MessageBoxButtons.YesNo);
      switch (result)
      {
        case System.Windows.Forms.DialogResult.Yes:
          currentRecordExperiment = experiment_to_add;
          InfoBar.Items[3].Text = "Exp loaded, id = " + currentRecordExperiment.id.ToString();
          break;
        case System.Windows.Forms.DialogResult.No:
          break;
      }
    }
    private void OpenDB_Click(object sender, EventArgs e)
    {
      // Нужно переделать на выбор файла.
      // ман в этих статьях
      //http://support.microsoft.com/kb/307283/ru
      //http://msdn.microsoft.com/en-us/library/gg696604(v=vs.113).aspx
      try
      {
        using (ExpDataContext _db = new ExpDataContext())
        {
          int expcount = _db.Experiments.Count();
          InfoBar.Items[1].Text = "connected";
        }
      }
      catch(EntitySqlException ex)
      {
        MessageBox.Show(ex.Message.ToString());
      }
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
              FCreateExpOpt CreateExpForm = new FCreateExpOpt();
              Experiment experiment_to_add = new Experiment();
              CreateExpForm.ShowDialog();
              experiment_to_add.About = CreateExpForm.ExpName.Text;
              experiment_to_add.Author = CreateExpForm.AuthorName.Text;
              experiment_to_add.CreationTime = DateTime.Now;
              experiment_to_add.Target = CreateExpForm.ExpTarget.Text;
              _db.Experiments.Add(experiment_to_add);
              _db.SaveChanges();
              currentRecordExperiment = experiment_to_add;
              InfoBar.Items[3].Text = "Exp ok, id = " + currentRecordExperiment.id.ToString();

              break;
          }
        }
        else
        {
          FSelectExperiment selectExp = new FSelectExperiment();
          selectExp.ShowDialog();
          currentRecordExperiment = _db.Experiments.Where(x => x.id == selectExp.SelectedID).FirstOrDefault();
          if (currentRecordExperiment != null)
          {
            InfoBar.Items[3].Text = "Exp ok, id = " + currentRecordExperiment.id.ToString();
          }
          else
          {
            InfoBar.Items[3].Text = "Exp load error, try agane";
          }
        }
      }
    }

    private void CreateDB_Click(object sender, EventArgs e)
    {
      throw new NotImplementedException("на данный момент опция отключена. Делает М.К.Татаринцев");
    }

    private void CreateMeasureButton_Click(object sender, EventArgs e)
    {
      using (ExpDataContext _db = new ExpDataContext())
      {
        FCreateMeasure CreateMeasure = new FCreateMeasure();
        CreateMeasure.ShowDialog();
        if (CreateMeasure.measure != null)
        {
          CreateMeasure.measure.ExperimentID = currentRecordExperiment.id;
          _db.Manager.Add(CreateMeasure.measure);
          try
          {
            _db.SaveChanges();

            MessageBox.Show("Succesful created");
            currentRecordMeasure = CreateMeasure.measure;
          }
          catch (EntityException ex)
          {
            MessageBox.Show(ex.Message);
          }
        }
      }
    }

    private void StartButton_Click(object sender, EventArgs e)
    {
      DoRecieveData = true;
    }

    public void RecieveFltData(TFltDataPacket data)
    {
      using (ExpDataContext _db = new ExpDataContext())
      {

        _db.SaveChanges();
      }

    }

    public void RecievePackData(CPack pack)
    {
      using (ExpDataContext _db = new ExpDataContext())
      {
        _db.SaveChanges();
      }
    }

    public void RecieveStimData(List<TAbsStimIndex> stims)
    {
      using (ExpDataContext _db = new ExpDataContext())
      {
        _db.SaveChanges();
      }
    }

    private void StopButton_Click(object sender, EventArgs e)
    {
      DoRecieveData = false;
      MeasureLength = 0;
    }
  }
}
