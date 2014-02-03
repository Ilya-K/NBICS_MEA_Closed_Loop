using System;
using MEAClosedLoop;

using TData = System.Double;

namespace Neurorighter
{
  public class BFParams
  {
    public int filterOrder;
    public double samplingFreq;
    public double lowCutFreq;
    public double highCutFreq;
    public int dataBufLength;

    public BFParams(int filterOrder = 2, double samplingFreq = Param.DAQ_FREQ, double lowCutFreq = 150.0, double highCutFreq = 2000.0, int dataBufLength = Param.DAQ_FREQ / 10)
    {
      this.filterOrder = filterOrder;
      this.samplingFreq = samplingFreq;
      this.lowCutFreq = lowCutFreq;
      this.highCutFreq = highCutFreq;
      this.dataBufLength = dataBufLength;
    }
  }

  public class SALPAParams
  {                           // NR Defaults | My Defaults
    public int length_sams;   // 75          | 35
    public int asym_sams;     // 10          | 10
    public int blank_sams;    // 75          | 35
    public int ahead_sams;    // 5           | 5
    public int forcepeg_sams; // 10          | 10
    public int PRE { get { return 2 * length_sams; } }
    public int POST { get { return 2 * length_sams + 1 + ahead_sams; } } 
    public TData railLow;
    public TData railHigh;
    public int[] thresh;
    public const TData RAIL_LOW = -1E6;
    public const TData RAIL_HIGH = 1E6;

    public SALPAParams(int length_sams = 75,
                       int asym_sams = 10,
                       int blank_sams = 75,
                       int ahead_sams = 5,
                       int forcepeg_sams = 10,
                       int[] thresh = null,
                       TData railLow = RAIL_LOW,
                       TData railHigh = RAIL_HIGH)
    {
      this.length_sams = length_sams;
      this.asym_sams = asym_sams;
      this.blank_sams = blank_sams;
      this.ahead_sams = ahead_sams;
      this.forcepeg_sams = forcepeg_sams;
      this.railHigh = railHigh;
      this.railLow = railLow;
      this.thresh = thresh;
    }
  }
}
