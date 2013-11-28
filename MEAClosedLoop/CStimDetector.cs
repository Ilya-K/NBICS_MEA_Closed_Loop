using System;
using System.Collections.Generic;
using System.Text;
using Common;


namespace MEAClosedLoop
{
  using TRawData = UInt16;
  using TData = Double;
  using TStimIndex = System.Int16;

  using TRawDataPacket = Dictionary<int, ushort[]>;
  //using TStimIndex = System.Int16;

  public class CStimDetector
  {
    //private const int EXP_MEAN_N = 50;
    //private TRawDataPacket m_prevPacket;
    private CCalcSE_N m_calcSE;
    private double m_threshhold;
    private int m_blankWidth;
    private int m_maxSlope;
    private bool m_prevPacketNotFinished = false;
    private bool m_continuePrevPacket = false;
    private TRawData[] m_prevBlock;
    private TData m_prevSE;
    //private double m_meanSE = 0;
    //private int N_MEAN;


    /// <summary>
    /// Stimulus artefact detector
    /// </summary>
    /// <param name="n">SE window width. Should be half of "still" period</param>
    /// <param name="threshhold">How much SE should decrease to consider start of blanking period</param>
    /// <param name="maxSlope">How much should be summary slope of 3 samples after blanking period</param>
    public CStimDetector(int n, TData threshhold = 35, TRawData maxSlope = 150)
    {
      m_calcSE = new CCalcSE_N(n);
      m_prevBlock = new TRawData[n];
      m_blankWidth = n;
      m_threshhold = threshhold;
      m_maxSlope = maxSlope;
      //N_MEAN = 1;
    }

    public List<TStimIndex> Detect(TRawData[] packet, List<TStimGroup> expectedStims)
    {
      List<TStimIndex> detected = new List<TStimIndex>();
      if (expectedStims != null)
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
