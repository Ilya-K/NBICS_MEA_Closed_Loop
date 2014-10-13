using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
    #region константы 
    private TTime StimControlDuration = Param.MS * 25; //Время до которого после стимула пачка считается вызванной 
    private TTime StimActualityDuration = Param.MS * 100; // Время, на протяжении которого для стимула ищется пачка
    #endregion
    private List<TAbsStimIndex> StimList = new List<TAbsStimIndex>();
    private object StimQueueLock = new object();
    
    public delegate void OnPackFoundDelegate(CPack pack, TAbsStimIndex stim);
    public event OnPackFoundDelegate OnEvPackFound;

    private delegate void ProcessBurstDelegate(CPack burst);
    private ProcessBurstDelegate processBurstDelegate;
    private delegate void ProcessStimDelegate(TAbsStimIndex stim);
    private ProcessStimDelegate processStimDelegate;

    private TTime CurrentTime = 0;

    public CEvokedPackDetector()
    {
      processBurstDelegate += ProcessBurst;
      processStimDelegate += ProcessStim;
    }

    public void RecieveBurst(CPack pack)
    {
      
    }

    public void RecieveStim(List<TAbsStimIndex> stims)
    {
      foreach (TAbsStimIndex stim in stims)
      processStimDelegate(stim);
    }
    public void ProcessBurst(CPack burst)
    {
      //lock (StimListLock)
      //  foreach (TTime stim in StimList)
      //  {
      //    // стимул должен находится внутри пачки или быть раньшее её не более чем на StimControlDuration.
      //    if (stim + StimControlDuration > pack.Start && stim < pack.Start + (TTime)pack.Length)
      //    {

      //    }
      //  }

    }
    public void ProcessStim(TAbsStimIndex stim)
    {
      lock (StimQueueLock)
      {
        
        StimList.Add(stim);
        StimList = (List<TAbsStimIndex>) StimList.TakeWhile(s => s + StimActualityDuration > CurrentTime);
        //old version
        //for (; StimQueue.ElementAt(0) + StimActualityDuration < CurrentTime; StimQueue.Dequeue()) ;
      }
    }

  }
}
