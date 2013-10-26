using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEAClosedLoop
{
  class CCalcSE_Block
  {
    private readonly int N_BLOCKS;
    private double meanOld;
    private double se2Old;
    private int N;

    public CCalcSE_Block(int nBlocks)
    {
      N_BLOCKS = nBlocks;
      N = 1;
      meanOld = 0;
      se2Old = 0;
    }

    // se  : Standard Error
    // se2 : Variance = se^2
    // dm  : delta mean
    // N   : Average over N blocks
    public void se(double[] data, out double _mean, out double _se)
    {
      
      double meanNew = data.Average();
      _mean = (meanOld * (N - 1) + meanNew) / N;

      double dm2Old = Math.Pow(_mean - meanOld, 2);
      double dm2New = Math.Pow(_mean - meanNew, 2);
      meanOld = _mean;

      double se2New = 0;
      Array.ForEach(data, x => se2New += Math.Pow(x - meanNew, 2));
      se2New = se2New / data.Length;

      double se2 = ((N - 1) * (se2Old + dm2Old) + se2New + dm2New) / N;
      se2Old = se2;

      _se = Math.Sqrt(se2);

      N = N_BLOCKS;
    }
  }
}
