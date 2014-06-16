using System;
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

    private SqlConnection currentRecordConnection = new SqlConnection();
    private SqlConnectionStringBuilder str_build; //current connection string
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
      using (ExpDataContext _db = new ExpDataContext(str_build.ToString()))
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
      string CreateDB_SQL_command;
      string DbName = "";
      string DbFileName = "";
      string DbLogFileName = "";
      string DbLogName = "";
      SaveFileDialog saveFileDialog = new SaveFileDialog();

      saveFileDialog.Filter = "mdf| *.mdf";
      saveFileDialog.ShowDialog();


      SqlConnection myConn = new SqlConnection(@"Data Source=.\SQLEXPRESS;Integrated security=SSPI;database=master");

      DbName = "ExpData_" + DateTime.Now.Date.ToString().Replace(" ", "_").Replace(".", "_").Replace(":", "_").Replace("-", "_");
      DbLogName = DbName + "_Log";

      DbFileName = saveFileDialog.FileName.ToString();
      DbLogFileName = saveFileDialog.FileName.ToString().Replace(".mdf", "log.ldf");

      CreateDB_SQL_command = "CREATE DATABASE " + DbName + " ON PRIMARY " +
          "(NAME = Exp_Data_" + @DbName + ", " +
          @"FILENAME = '" + @DbFileName + "', " +
          "SIZE = 100MB, MAXSIZE = 10GB, FILEGROWTH = 10%) " +
          "LOG ON (NAME = " + @DbLogName + ", " +
          @"FILENAME = '" + DbLogFileName + "', " +
          "SIZE = 100MB, " +
          "MAXSIZE = 5GB, " +
          "FILEGROWTH = 15%)";

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
        MessageBox.Show(CreateDB_SQL_command);
      }
      finally
      {
        if (myConn.State == ConnectionState.Open)
        {
          myConn.Close();
        }

        // попробуем изменить права на открытие файла

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
      OpenFileDialog open_db_file = new OpenFileDialog();
      open_db_file.Filter = "mdf| *.mdf";

      switch (open_db_file.ShowDialog())
      {
        case System.Windows.Forms.DialogResult.OK:
          if (open_db_file.FileNames.Count() > 1)
          {
            MessageBox.Show("необходимо выбрать только 1 файл", "ошибка");
            return;
          }
          FilePath = open_db_file.FileName;
          break;
        case System.Windows.Forms.DialogResult.Cancel:
          return;
      }

      str_build = new SqlConnectionStringBuilder();
      str_build.DataSource = ".\\SQLEXPRESS";
      str_build.IntegratedSecurity = true;
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

      using (ExpDataContext _db = new ExpDataContext(str_build.ToString()))
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
          FSelectExperiment selectExp = new FSelectExperiment(str_build.ToString());
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
      using (ExpDataContext _db = new ExpDataContext(str_build.ToString()))
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
      if (currentRecordMeasure == null)
      {
        MessageBox.Show("Сначала необходимо создать measure");
        return;
      }
      DoRecieveData = true;
      StartButton.Enabled = false;
      StopButton.Enabled = true;

      DoRecordCmpData.Enabled = false;
      DoRecordCompressData.Enabled = false;
      DoRecordPackData.Enabled = false;
      DoRecordStimData.Enabled = false;

      SelectAllRecords.Enabled = false;
      DeselectAllRecs.Enabled = false;
    }

    private void StopButton_Click(object sender, EventArgs e)
    {
      DoRecieveData = false;
      currentRecordMeasure = null;

      MeasureLength = 0;


      DoRecordCmpData.Enabled = true;
      DoRecordCompressData.Enabled = true;
      DoRecordPackData.Enabled = true;
      DoRecordStimData.Enabled = true;

      StartButton.Enabled = true;
      SelectAllRecords.Enabled = true;
      DeselectAllRecs.Enabled = true;
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
      //[TODO]: необходимо сделать учет длины 1 секунды записи

      data = CDataCompress.BinaryRawToRawData(CDataCompress.RawDataToBinary(data));

      if (DoRecieveData && (DoRecordCmpData.Checked || DoRecordCompressData.Checked))
      {
        MeasureLength += (TTime)(data[data.Keys.FirstOrDefault()].Length);

        if (RecordTimeElapsed.InvokeRequired) 
          RecordTimeElapsed.Invoke(new Action<string>(s => RecordTimeElapsed.Text = s), (MeasureLength / 25000).ToString());
        else 
          RecordTimeElapsed.Text = (MeasureLength / 25000).ToString();

        //RecordTimeElapsed.Text = (MeasureLength / 25000).ToString() + " s";
        using (ExpDataContext _db = new ExpDataContext(str_build.ToString()))
        {

          _db.SaveChanges();
        }
      }

    }

    public void RecievePackData(CPack pack)
    {
      if (DoRecieveData && DoRecordPackData.Checked)
      using (ExpDataContext _db = new ExpDataContext(str_build.ToString()))
      {
        PackData packData = new PackData(pack);
        packData.MeasureID = currentRecordMeasure.id;
        _db.PackData.Add(packData);
        _db.SaveChanges();
      }
    }

    public void RecieveStimData(List<TAbsStimIndex> stims)
    {
      if(DoRecieveData && DoRecordPackData.Checked)
      using (ExpDataContext _db = new ExpDataContext(str_build.ToString()))
      {
        _db.SaveChanges();
      }
    }
  }
  public enum DatabaseConnectionState
  {
    Connected,
    Disconnected,
    Openned
  }
}
