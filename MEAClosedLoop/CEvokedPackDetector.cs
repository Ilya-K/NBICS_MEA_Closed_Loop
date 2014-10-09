using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

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
  public class CEvokedPackDetector : IRecieveBusrt, IRecieveStim
  {
    private List<TAbsStimIndex> StimList = new List<TAbsStimIndex>();
    
    public delegate void OnPackFoundDelegate(CPack pack, TAbsStimIndex stim);
    public event OnPackFoundDelegate OnPackFound;

    private delegate void ProcessBurstDelegate(CPack burst);
    private ProcessBurstDelegate processBurstDelegate;
    private delegate void ProcessStimDelegate(TAbsStimIndex stim);
    private ProcessStimDelegate processStimDelegate;

    public CEvokedPackDetector()
    {
      processBurstDelegate += ProcessBurst;
      processStimDelegate += ProcessStim;
    }

    public void RecieveBurst(CPack pack)
    {
      processBurstDelegate(pack);
    }

    public void RecieveStim(List<TAbsStimIndex> stims)
    {
      foreach (TAbsStimIndex stim in stims)
      processStimDelegate(stim);
    }
    public void ProcessBurst(CPack burst)
    {

    }
    public void ProcessStim(TAbsStimIndex stim)
    {

    }

  }
}
