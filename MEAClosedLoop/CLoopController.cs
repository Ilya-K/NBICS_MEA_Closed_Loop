﻿//#define DEBUG_SPIKETRAINS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MEAClosedLoop.Common;
namespace MEAClosedLoop
{
  using TTime = System.UInt64;
  using TData = System.Double;

  public class CLoopController
  {
    // Time of applying of stimulus as a share of period of activity
    //private double STIM_TIME_PERCENT = 0.8;
    //[DEBUG]
    private double STIM_TIME_PERCENT = 1.0; 

    // Delay of stimulus introduced by signal processing time (in 40 us intervals)
    private const Int16 STIM_TIME_DELAY = 2 * Param.MS;             // 4 ms

    // Minimal allowed period for stimulation
    private const Int16 MIN_STIM_PERIOD = 400 * Param.MS;           // 0.4s = 10000

    // Number of SE to leave before the next expected pack
    private const Int16 N_SE = 1; //normally 2

    private CInputStream m_inputStream;
    private CStimulator m_stimulator;
    private CFiltering m_filter;
    private CPackDetector m_packDetector;
    private TStimGroup m_stimulus;

    public volatile Int32 ReceivedStimShift = 0;
    public volatile bool DoStim = false;

    private Thread m_t;
    private volatile bool m_stop = false;
    private System.Timers.Timer m_stimTimer;
    public delegate void OnPackFoundDelegate(CPack pack);
    public event OnPackFoundDelegate OnPackFound;
    public delegate void onSetStimDelegate(TStimGroup ExpStim);
    public event onSetStimDelegate OnSetStim;

    public CLoopController(CInputStream inputStream, CFiltering filter, CStimulator stimulator)
    {
      if (inputStream == null) throw new ArgumentNullException("inputStream");
      if (filter == null) throw new ArgumentNullException("filter");
      if (stimulator == null) throw new ArgumentNullException("stimulator");

      m_inputStream = inputStream;
      m_stimulator = stimulator;
      m_filter = filter;

      m_stimulator.DownloadDefaultShape(1, 1, 1, 200000);
      m_stimulus = m_stimulator.GetStimulus();
      m_packDetector = new CPackDetector(m_filter);

      m_stimTimer = new System.Timers.Timer();
      m_stimTimer.Elapsed += StimTimer;

      m_t = new Thread(FeedBackLoop);
      m_t.Start();
    }

    public void Stop()
    {
      m_stop = true;
    }
   
#if DEBUG_SPIKETRAINS
    public Dictionary<int, List<CSpikeTrainFrame>> GetSpikeTrainsDbg()
    {
      return m_packDetector.GetSpikeTrainDbg();
    }
#endif

    private void FeedBackLoop()
    {
      CCalcExpWndSE m_se = new CCalcExpWndSE(10); // Mean over ~30 samples
      
      // Wait one full pack first of all
      CPack prevPack = m_packDetector.WaitPack();
      if (!prevPack.EOP)
      {
        prevPack = m_packDetector.WaitPack();
      }

      CPack currPack = prevPack;                  // Dummy assignment, just to shut up the compiler 
      bool insidePack = false;
      TData meanPackPeriod;
      TData sePackPeriod;

      if (OnPackFound != null) OnPackFound(currPack);
      while (!m_stop)
      {
        CPack currSemiPack = m_packDetector.WaitPack();
        // [TODO] May be it would be useful to use timeout and give control back sometimes
        // while (null == (currSemiPack = m_packDetector.WaitPack(500))) { }; // Just don't know what to do here

        // Handle the situation when a single pack is divided into two parts: Start and End
        // The Start part has already been processed at the previous step
        if (currSemiPack.EOP)                     // We've just received a pack with EndOfPack flag
        {
          // Distribute current pack (with EOP) to consumers
          if (OnPackFound != null) OnPackFound(currSemiPack);
          
          if (insidePack)                         // We're inside of previously started pack
          {
            currPack.Length = currSemiPack.Length;
            prevPack = currPack;
            insidePack = false;
            continue;                             // Start of this pack has already been processed
          }
        }
        else                                      // We've received Start of a long pack
        {
          insidePack = true;
        }
        currPack = currSemiPack;
        
        // Calculate Mean and SE
        sePackPeriod = m_se.SE(currPack.Start - prevPack.Start);
        meanPackPeriod = m_se.Mean;

        // Calculate time of the next stumulation
        //old
        //Int32 stimShift = (Int32)meanPackPeriod - N_SE * (Int32)sePackPeriod - STIM_TIME_DELAY;
        Int32 stimShift = ReceivedStimShift - STIM_TIME_DELAY;
        if (stimShift < 0) stimShift = 0;
        TTime nextStimTime = currPack.Start + (TTime)(STIM_TIME_PERCENT * stimShift);

        // Pass the next stimulation time to the StimDetector
        if (ReceivedStimShift > 0 && DoStim)
        {
          m_stimulus.stimTime = nextStimTime;
          m_filter.StimDetector.SetExpectedStims(m_stimulus);

          //
 
          m_stimTimer.Interval = m_inputStream.GetIntervalFromNowInMS(nextStimTime) + 1; // +1 - just for debug, to avoid null time
          m_stimTimer.Start();
          // Костыль, не будет работать с 
          //OnSetStim(m_stimulus);
        }
        prevPack = currPack;
      }
    }

    private void StimTimer(object o1, EventArgs e1)
    {
      //[DEBUG]
      m_stimulator.Start();
      m_stimTimer.Stop();
    }
  }
}
