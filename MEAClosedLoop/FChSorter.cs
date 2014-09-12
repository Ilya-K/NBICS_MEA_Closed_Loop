using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MEAClosedLoop;

namespace MEAClosedLoop
{
  #region Definitions
  using TTime = System.UInt64;
  using TData = System.Double;
  using TFltDataPacket = Dictionary<int, System.Double[]>;
  using TStimIndex = System.Int16;
  using TAbsStimIndex = System.UInt64;
  using TRawData = UInt16;
  using TRawDataPacket = Dictionary<int, ushort[]>;
  #endregion
  public partial class FChSorter : Form, IRecieveBusrt, IRecieveStim 
  {
    private delegate void ProcessBurstDelegate(CPack burst);
    private ProcessBurstDelegate processBurstDelegate;
    private delegate void ProcessStimDelegate(TAbsStimIndex stim);
    private ProcessStimDelegate processStimDelegate;


    private List<int> Keys = new List<int>();
    
    public FChSorter()
    {
      InitializeComponent();
    }

    public void RecieveBurst(CPack pack)
    {
      BeginInvoke(processBurstDelegate, pack);
    }

    public void RecieveStim(TAbsStimIndex stim)
    {
      BeginInvoke(processStimDelegate, stim);
    }
    public void ProcessBurst(CPack burst)
    {

    }
    public void ProcessStim(TAbsStimIndex stim)
    {

    }
    private void StartButton_Click(object sender, EventArgs e)
    {
      processBurstDelegate += ProcessBurst;
      processStimDelegate += ProcessStim;
    }

  }
}
