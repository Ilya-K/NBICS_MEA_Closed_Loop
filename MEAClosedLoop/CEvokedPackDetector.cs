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
  public class CEvokedBurstDetector : IRecieveBusrt, IRecieveStim
  {
    // Простой, вспомогательный класс для отсеивания непосредственно вызванных пачек.
    // 

    #region константы 
    private TTime StimControlDuration = Param.MS * 25; //Время до которого после стимула пачка считается вызванной 
    private TTime StimActualityDuration = Param.MS * 100; // Время, на протяжении которого для стимула ищется пачка
    #endregion
    private List<TAbsStimIndex> StimList = new List<TAbsStimIndex>();
    private object StimQueueLock = new object();
    
    public delegate void OnEvPackFoundDelegate(SEvokedPack evPack);
    public OnEvPackFoundDelegate OnEvPackFound;

    private delegate void ProcessBurstDelegate(CPack burst);
    private ProcessBurstDelegate processBurstDelegate;
    private delegate void ProcessStimDelegate(TAbsStimIndex stim);
    private ProcessStimDelegate processStimDelegate;

    private TTime CurrentTime = 0;

    public CEvokedBurstDetector()
    {
      processBurstDelegate += ProcessBurst;
      processStimDelegate += ProcessStim;
    }

    #region External Recieve Methods
    public void RecieveBurst(CPack pack)
    {
      processBurstDelegate(pack);
    }

    public void RecieveStim(List<TAbsStimIndex> stims)
    {
      foreach (TAbsStimIndex stim in stims)
      processStimDelegate(stim);
    }
    #endregion


    public void ProcessBurst(CPack burst)
    {
      CurrentTime = burst.Start;
      lock (StimQueueLock)
        foreach (TTime stim in StimList)
        {
          // стимул должен находится внутри пачки или быть раньшее её не более чем на StimControlDuration.
          if (stim + StimControlDuration > burst.Start && stim < burst.Start + (TTime)burst.Length)
          {
            SEvokedPack evPack;
            evPack.Burst = burst;
            evPack.stim = stim;
            if(OnEvPackFound != null)OnEvPackFound(evPack);
          }
        }
    }
    public void ProcessStim(TAbsStimIndex stim)
    {
      lock (StimQueueLock)
      {
        StimList.Add(stim);
        StimList.RemoveAll(s => s + StimActualityDuration < CurrentTime);
        //StimList = (List<TAbsStimIndex>) StimList.TakeWhile(s => s + StimActualityDuration > CurrentTime);
      }
    }

  }
}
