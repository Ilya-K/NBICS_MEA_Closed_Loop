using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MEAClosedLoop
{
  using TTime = System.UInt64;
  using TData = System.Double;

  public class CLoopController
  {
    // Time of applying of stimulus as a share of period of activity
    private double STIM_TIME_PERCENT = 0.8;
    // Delay of stimulus introduced by signal processing time (in 40 us intervals)
    private Int16 STIM_TIME_DELAY = 100;
    // Minimal allowed period for stimulation
    private const Int16 MIN_STIM_PERIOD = 10000; // 10000 = 0.4s
    // Number of SE to leave before the next expected pack
    private const Int16 N_SE = 2;

    private CInputStream m_inputStream;
    private CStimulator m_stimulator;
    private CFiltering m_filter;
    private CPackDetector m_packDetector;
    private TStimGroup m_stimulus;
    private Thread m_t;
    private volatile bool m_stop = false;
    private System.Timers.Timer m_stimTimer;


    public CLoopController(CInputStream inputStream, CFiltering filter, CStimulator stimulator)
    {
      m_inputStream = inputStream;
      m_stimulator = stimulator;
      m_filter = filter;

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

    private void FeedBackLoop()
    {
      CCalcExpWndSE m_se = new CCalcExpWndSE(10); // Mean over ~30 samples
      
      // Wait one full pack first of all
      CPack prevPack = m_packDetector.WaitPack();
      if (!prevPack.EOP)
      {
        CPack tempPack = m_packDetector.WaitPack();
        prevPack.Length = (Int32)(tempPack.Start - prevPack.Start);
      }

      CPack currPack = prevPack;                  // Dummy assignment, just to shut up the compiler 
      bool insidePack = false;
      TData meanPackPeriod;
      TData sePackPeriod;

      while (!m_stop)
      {
        CPack currSemiPack = m_packDetector.WaitPack();
        // [TODO] May be it would be useful to use timeout and give control back sometimes
        // while (null == (currSemiPack = m_packDetector.WaitPack(500)));

        // Handle the situation when a single pack is divided into two parts: Start and End
        if (currSemiPack.EOP)                     // We've just received a pack with EndOfPack flag
        {
          if (insidePack)                         // End of previously started pack
          {
            currPack.Length = (Int32)(currSemiPack.Start - currPack.Start);
            prevPack = currPack;
            insidePack = false;
            continue;
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
        Int32 stimShift = (Int32)meanPackPeriod - N_SE * (Int32)sePackPeriod - STIM_TIME_DELAY;
        if (stimShift < 0) stimShift = 0;

        TTime nextStimTime = currPack.Start + (TTime)(STIM_TIME_PERCENT * stimShift);

        // Pass the next stimulation time to the StimDetector
        m_stimulus.stimTime = nextStimTime;
        m_filter.StimDetector.SetExpectedStims(m_stimulus);

        // 
        m_stimTimer.Interval = m_inputStream.GetIntervalFromNowInMS(nextStimTime);
        m_stimTimer.Start();

        prevPack = currPack;

      }
    }

    private void StimTimer(object o1, EventArgs e1)
    {
      m_stimulator.Start();
    }
  }
}
