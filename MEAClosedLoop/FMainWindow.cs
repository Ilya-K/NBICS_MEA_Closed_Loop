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
  public partial class FMainWindow : Form
  {
    public Form1 MainManager;

    private FRecorder _Recorder;
    private CFiltering _Filter;
    private CLoopController _LoopController;

    public FRecorder Recorder 
    {
      get 
      {
        return _Recorder;
      }
      set
      {
        // необходимо проверить список
        _Recorder = value;
      }
    }
    public CFiltering Filter {get; set;}
    public CLoopController LoopController { get; set; }

    List<IRecieveBusrt> BurstrRecievers = new List<IRecieveBusrt>();
    List<IRecieveFltData> FltDataRecievers = new List<IRecieveFltData>();
    List<IRecieveStim> StimRecievers = new List<IRecieveStim>();

    //public delegate 

    public FMainWindow()
    {
      InitializeComponent();
      MainManager = new Form1();
      MainManager.MdiParent = this;
      MainManager.MainWindow = this;
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void newManagerToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (MainManager == null || MainManager.IsDisposed)
      {
        MainManager = new Form1();
        MainManager.MdiParent = this;
        MainManager.Show();
      }
      else
      {
        MainManager.WindowState = FormWindowState.Normal;
      }
    }

    #region Recorder
    private void recorderToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (MainManager == null || MainManager.IsDisposed)
      {
        MessageBox.Show("Ошибка, менеджер закрыт или не запущен");
        return;
      }
      if (Recorder != null && Recorder.IsDisposed)
      {
        Recorder = null;
      }
      if (Recorder == null)
      {
        Recorder = new FRecorder();
        if (MainManager.m_salpaFilter != null)
        {
          MainManager.m_salpaFilter.AddDataConsumer(Recorder.RecieveFltData);
          MainManager.m_salpaFilter.AddStimulConsumer(Recorder.RecieveStimData);
          Recorder.StimDataConnected = true;
          Recorder.RawDataConnected = true;
        }
        else
        {
          switch (MessageBox.Show("Не запущен фильтр данных, \nзапись данных невозможна", "предупреждение", MessageBoxButtons.OKCancel))
          {
            case System.Windows.Forms.DialogResult.OK:
              break;
            case System.Windows.Forms.DialogResult.Cancel:
              Recorder = null;
              return;
          }
        }
        if (MainManager.m_closedLoop != null)
        {
          MainManager.m_closedLoop.OnPackFound += Recorder.RecievePackData;
          Recorder.PackDataConnected = true;
        }
        else
        {
          switch (MessageBox.Show("Не запущена петля эксперимента, \nзапись данных о пачках невозможна", "предупреждение", MessageBoxButtons.OKCancel))
          {
            case System.Windows.Forms.DialogResult.OK:
              break;
            case System.Windows.Forms.DialogResult.Cancel:
              Recorder = null;
              return;
          }
        }
        Recorder.Show();
      }
      else
      {
        Recorder.Show();
      }
    }
    #endregion

    private void FMainWindow_Load(object sender, EventArgs e)
    {
      //открываем панель главного менеджера
      MainManager.StartPosition = FormStartPosition.Manual;
      MainManager.Location = new System.Drawing.Point(0, 0);
      MainManager.Show();
    }

    private void findGoodChannels_Click(object sender, EventArgs e)
    {
      FChSorter ChSorterForm = new FChSorter();
      ChSorterForm.Show();
    }

    public void AddConsumer<ObjType>(ObjType Obj)
    {//одписывание интерфейсов на раздачу потоков данных
      if ((from obj in BurstrRecievers where obj.Equals(Obj) select obj).Count() == 0) return;

      if (Obj is IRecieveBusrt)
      {
        BurstrRecievers.Add(Obj as IRecieveBusrt);
      }
      if (Obj is IRecieveFltData)
      {
        throw new NotImplementedException("in develop:");
      }
      if (Obj is IRecieveStim)
      {
        StimRecievers.Add(Obj as IRecieveStim);
      }
    }
    public void RemoveConsumer<ObjType>(ObjType Obj)
    {//одписывание интерфейсов на раздачу потоков данных
      if (Obj is IRecieveBusrt)
      {
        throw new NotImplementedException("in develop:");
      }
      if (Obj is IRecieveFltData)
      {
        throw new NotImplementedException("in develop:");
      }
      if (Obj is IRecieveStim)
      {
        throw new NotImplementedException("in develop:");
      }
    }

  }

}
