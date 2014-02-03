using System;
using System.Collections.Generic;
using System.Text;
using Common;


namespace MEAClosedLoop
{
  using TRawData = UInt16;
  using TData = Double;
  using TTime = UInt64;
  using TStimIndex = System.Int16;
  using TRawDataPacket = Dictionary<int, ushort[]>;

  public class CStimDetector
  {
    private const TTime INFINITY = TTime.MaxValue;
    //private const int EXP_MEAN_N = 50;
    //private TRawDataPacket m_prevPacket;
    private CCalcSE_N m_calcSE;
    private double m_threshhold;
    private int m_blankWidth;
    private int m_maxSlope;
    // Maximum possible deviation of stimulus with respect to the expected stimulus
    private uint m_maxJitter;
    private bool m_prevPacketNotFinished = false;
    private bool m_continuePrevPacket = false;
    private TRawData[] m_prevBlock;
    private TData m_prevSE;
    private List<TStimGroup> m_expectedStims;
    private object lockStimList = new object();
    private TStimGroup m_nextExpectedStim;
    private int m_artifChannel = -1;
    public int ArtifactChannel { set { m_artifChannel = value; } }
    bool m_stimExpectedNow = false;


    //private double m_meanSE = 0;
    //private int N_MEAN;


    /// <summary>
    /// Stimulus artefact detector
    /// </summary>
    /// <param name="n">SE window width. Should be equal to a half of the blanking period</param>
    /// <param name="maxJitter">Maximum possible deviation of the real stimulus with respect to the expected one</param>
    /// <param name="threshhold">How much SE should decrease to consider start of blanking period</param>
    /// <param name="maxSlope">How much should be summary slope of 3 samples after blanking period</param>
    public CStimDetector(int n, int maxJitter = 20 * Param.MS, TData threshhold = 35, TRawData maxSlope = 150)
    {
      m_calcSE = new CCalcSE_N(n);
      m_prevBlock = new TRawData[n];
      m_blankWidth = n;
      m_threshhold = threshhold;
      m_maxSlope = maxSlope;
      m_maxJitter = (uint)maxJitter;
      m_expectedStims = new List<TStimGroup>();
      m_nextExpectedStim.stimTime = INFINITY;
      //N_MEAN = 1;
    }

    public void SetExpectedStims(TStimGroup nextStimGroup)
    {
      lock (lockStimList)
      {
        if (m_nextExpectedStim.stimTime == INFINITY)
        {
          m_nextExpectedStim = nextStimGroup;
        }
        else
        {
          m_expectedStims.Add(nextStimGroup);
        }
      }
    }

    public bool IsDataRequired(TTime eopTimestamp)
    {
      lock (lockStimList)
      {
        if (m_expectedStims.Count > 0) return false;
        return m_stimExpectedNow = (eopTimestamp + m_maxJitter >= m_nextExpectedStim.stimTime); //m_expectedStims[0].stimTime);
      }
    }

    public List<TStimIndex> Detect(TRawDataPacket fullPacket)
    {
      List<TStimIndex> detected = new List<TStimIndex>();
      TRawData[] packet = fullPacket[m_artifChannel];

      if (m_expectedStims != null)
      {
        // [TODO] Code to take expectedStims into account should be placed here


      }
      else
      {
        int shift = -m_calcSE.PartialN;
        TData[] se = m_calcSE.SE(packet);
        int partialN = m_calcSE.PartialN;
        int seSize = se.Length;
        for (int i = 0; i < seSize; ++i)
        {
          TData prevSE = (i == 0) ? m_prevSE : se[i - 1];
          TData nextSE = (i + 1 == seSize) ? 0 : se[i + 1];
          if (prevSE * 10 + 1 < se[i] + nextSE)
          {
            TStimIndex idx = 0;
            // [TODO] Add correct processing of m_prevBlock
            int offset = shift + i * m_blankWidth;
            offset = (offset < 0) ? 0 : offset;
            if (FindRightEdge(packet, offset, ref idx))
            {
              detected.Add(idx);
              i += 2;                             // Skip 2 subsequent periods, since they cannot contain new blanking artifact
            }
          }
        }
        int t = packet.Length - partialN;
        for (int i = 0; i < partialN; ++i)
        {
          m_prevBlock[i] = packet[t++];
        }
        m_prevSE = se[seSize - 1];
      }
      return detected;
    }

    // offset must be < packet.Length - 1 to leave at least 2 samples at the end
    private bool FindRightEdge(TRawData[] packet, int offset, ref TStimIndex stimStart)
    {
      int end = offset + m_blankWidth;
      if (end > packet.Length - 1) end = packet.Length - 1;            // Leave 1 sample before the end of packet

      for (int i = offset; i < end; ++i)
      {
        if (Math.Abs((int)packet[i + 1] - packet[i]) > m_maxSlope)
        {
          stimStart = (TStimIndex)i;
          return true;
        }
      }
      return false;
    }

  }
}

/*
          // Wrong idea. It seems to be better to compare noise level with the fixed threshold rather than relative.
          // m_meanSE = (m_meanSE * (N_MEAN - 1) + se[i]) / N_MEAN;
          // N_MEAN = EXP_MEAN_N;
          // if (se[i] * m_threshhold < m_meanSE)
          
          if ((se[i] < m_threshhold) || m_continuePrevPacket)
          {
            if (m_continuePrevPacket)
            {
              --i;
              m_continuePrevPacket = false;
            }
            while (++i < seSize)
            {
              if (se[i] > m_threshhold) break;
            }
            TStimIndex idx = 0;
            if (i == seSize)
            {
              m_continuePrevPacket = true;
              if (partialN > 0)
              {
                if (FindRightEdge(packet, packet.Length - partialN - 1, ref idx))
                  detected.Add(idx);
                else 
                  m_continuePrevPacket = true;
                break;
              }
            }
            // [TODO] Add correct processing of m_prevBlock
            int offset = shift + i * m_blankWidth;
            offset = (offset < 0) ? 0 : offset;
            if (FindRightEdge(packet, offset, ref idx))
            {
              detected.Add(idx);
              i += 2;                             // Skip 2 subsequent periods, since they cannot contain new blanking artefact
            }
          }
*/
