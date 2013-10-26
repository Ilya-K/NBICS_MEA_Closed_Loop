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
    private CInputStream m_inputStream;
    private CStimulator m_stimulator;
    private CFiltering m_filter;
    private CPackDetector m_packDetector;
    private Thread m_t;
    private volatile bool stop = false;

    public CLoopController(CInputStream inputStream, CFiltering filter, CStimulator stimulator)
    {
      m_inputStream = inputStream;
      m_stimulator = stimulator;
      m_filter = filter;
      m_packDetector = new CPackDetector(m_filter);

      m_t = new Thread(FeedBackLoop);
      m_t.Start();
    }

    public void Stop()
    {
      stop = true;
    }

    private void FeedBackLoop()
    {
      CCalcExpWndSE m_se = new CCalcExpWndSE(33);
      CPack currPack; 
      CPack prevPack = m_packDetector.WaitPack();
      TTime prevPackTime = prevPack.Start;
      TTime currPackTime;
      TData meanInterPack;
      TData seInterPack;

      while (!stop)
      {
        currPack = m_packDetector.WaitPack();
        currPackTime = currPack.Start;
        TTime currInterPack = currPackTime - prevPackTime;
          
        seInterPack = m_se.SE(currInterPack);
        meanInterPack = m_se.Mean;

        /*
        meanInterPack = (TTime)(meanInterPack * 0.9 + currInterPack * 0.1);
        mean2InterPack = (TTime)(mean2InterPack * 0.9 + currInterPack * currInterPack * 0.1);

        TTime seInterPack = (TTime)Math.Sqrt(mean2InterPack - meanInterPack * meanInterPack);
        */

      }
    }

  }
}
