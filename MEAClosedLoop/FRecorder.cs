﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
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
    public CDataCompress compressor = new CDataCompress();

    public bool RawDataConnected = false;
    public bool StimDataConnected = false;
    public bool PackDataConnected = false;

    private Experiment currentRecordExperiment = null;
    private Experiment currentPlayExperiment = null;
    private MeasureManager currentRecordMeasure = null;
    private MeasureManager currentPlayMeasure = null;
    private TTime MeasureLength = 0;
    private string ConnectionString = "";
    private bool DoRecieveData = false;

    SqlConnection currentRecordConnection = new SqlConnection();
    //Delegate Section;

    public FRecorder()
    {
      InitializeComponent();
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
    private void CreateDB_Click(object sender, EventArgs e)
    {
      String CreateDB_SQL_command;
      SqlConnection myConn = new SqlConnection("Data Source=GRAPH\\SQLEXPRESS;Integrated security=SSPI;database=master");

      CreateDB_SQL_command = "CREATE DATABASE Exp_data ON PRIMARY " +
          "(NAME = Exp_Data1, " +
          @"FILENAME = 'C:\MCS_Data\ExpData.mdf', " +
          "SIZE = 100MB, MAXSIZE = 10GB, FILEGROWTH = 10%) " +
          "LOG ON (NAME = MyDatabase_Log, " +
          @"FILENAME = 'C:\MCS_Data\ExpDataLog.ldf', " +
          "SIZE = 100MB, " +
          "MAXSIZE = 5GB, " +
          "FILEGROWTH = 10%)";

      SqlCommand myCommand = new SqlCommand(CreateDB_SQL_command, myConn);
      try
      {
        myConn.Open();
        myCommand.ExecuteNonQuery();
        MessageBox.Show("DataBase is Created Successfully", "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (System.Exception ex)
      {
        MessageBox.Show(ex.ToString(), "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      finally
      {
        if (myConn.State == ConnectionState.Open)
        {
          myConn.Close();
        }
      }
      //throw new NotImplementedException("на данный момент опция отключена. Делает М.К.Татаринцев");
    }
    private void OpenDB_Click(object sender, EventArgs e)
    {
      // Нужно переделать на выбор файла.
      // ман в этих статьях

      //http://support.microsoft.com/kb/307283/ru
      //http://msdn.microsoft.com/en-us/library/gg696604(v=vs.113).aspx

      string FilePath = "";
      OpenFileDialog dialog = new OpenFileDialog();

      switch (dialog.ShowDialog())
      {
        case System.Windows.Forms.DialogResult.OK:
          if (dialog.FileNames.Count() > 1)
          {
            MessageBox.Show("необходимо выбрать только 1 файл", "ошибка");
            return;
          }
          FilePath = dialog.FileName;
          break;
        case System.Windows.Forms.DialogResult.Cancel:
          return;
      }

      SqlConnectionStringBuilder str_build = new SqlConnectionStringBuilder();
      str_build.DataSource = "GRAPH\\SQLEXPRESS";
      str_build.Encrypt = true;
      str_build.InitialCatalog = FilePath;
      str_build.MultipleActiveResultSets = true;
     


      using (ExpDataContext _db = new ExpDataContext(str_build.ToString()))
      {
        try
        {
          int expcount = _db.Experiments.Count();
          InfoBar.Items[1].Text = "connected";
        }
        catch (ProviderIncompatibleException ex)
        {
          MessageBox.Show(ex.Message.ToString());
        }
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

    private void CreateMeasureButton_Click(object sender, EventArgs e)
    {
      using (ExpDataContext _db = new ExpDataContext())
      {
        FCreateMeasure CreateMeasure = new FCreateMeasure();
        CreateMeasure.ShowDialog();
        if (CreateMeasure.measure != null)
        {
          CreateMeasure.measure.ExperimentID = currentRecordExperiment.id;
          CreateMeasure.measure.StartTime = DateTime.Now;
          _db.Manager.Add(CreateMeasure.measure);

          try
          {
            _db.SaveChanges();
            currentRecordMeasure = CreateMeasure.measure;
            MessageBox.Show("Succesful created");
            currentRecordMeasure = CreateMeasure.measure;
          }
          catch (DbUpdateException ex)
          {
            MessageBox.Show(ex.ToString());
          }
        }
      }
    }

    private void StartButton_Click(object sender, EventArgs e)
    {
      DoRecieveData = true;
      StartButton.Enabled = false;
      StopButton.Enabled = true;

      DoRecordCmpData.Enabled = false;
      DoRecordCompressData.Enabled = false;
      DoRecordPackData.Enabled = false;
      DoRecordStimData.Enabled = false;
    }

    private void StopButton_Click(object sender, EventArgs e)
    {
      DoRecieveData = false;
      MeasureLength = 0;
      DoRecordCmpData.Enabled = true;
      DoRecordCompressData.Enabled = true;
      DoRecordPackData.Enabled = true;
      DoRecordStimData.Enabled = true;
    }

    private void SelectAllRecords_Click(object sender, EventArgs e)
    {
      DoRecordCmpData.Checked = true;
      DoRecordCompressData.Checked = true;
      DoRecordPackData.Checked = true;
      DoRecordStimData.Checked = true;
    }

    private void DeselectAllRecs_Click(object sender, EventArgs e)
    {
      DoRecordCmpData.Checked = false;
      DoRecordCompressData.Checked = false;
      DoRecordPackData.Checked = false;
      DoRecordStimData.Checked = false;
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
  }
}
