using System;
using System.Text;

namespace MEAClosedLoop
{
  using TData = Double;

  // This class provides a method to calculate moving SE and Mean over exponential window with width = tau
  public class CCalcExpWndSE
  {
    private readonly int TAU;
    private TData expMean = 0;       // mean(X) in exponential window
    private TData exp2Mean = 0;      // mean(X^2) in exponential window
    public TData Mean { get { return expMean; } }
    public int Width { get { return TAU; } }
    private bool first = true;

    /// <summary>
    /// Create SE calculator.
    /// Exponential multiplier decays to 1/e in tau samples, to 0.05 in 3*tau.
    /// Multiplier to be used in exponential sum, m = (tau-1)/tau.
    /// Consider 3*tau as a width of an exponential window.
    /// </summary>
    /// <param name="p">Number of samples to decay weight function to 1/e</param>
    public CCalcExpWndSE(int tau)
    {
      TAU = tau;
    }

    /// <summary>
    /// Calculate Standard Error (SE) of sequental data over exponential window with width ~3*tau
    /// </summary>
    /// <param name="nextData">Next data point to take into account</param>
    /// <returns>Current SE</returns>
    public TData SE(TData nextData)
    {
      if (first)
      {
        expMean = nextData;
        exp2Mean = nextData * nextData;
        first = false;
      }
      expMean = expMean - (expMean - nextData) / TAU;
      exp2Mean = exp2Mean - (exp2Mean - nextData * nextData) / TAU;

      // SE = sqrt(mean(X^2) - (mean(X))^2)
      return Math.Sqrt(exp2Mean - expMean * expMean);
      //return Math.Sqrt(Math.Abs(exp2Avg - expAvg * expAvg));
    }

    /// <summary>
    /// Calculate Standard Error (SE) of sequental data over exponential window with width ~3*tau
    /// </summary>
    /// <param name="nextData">Next data point to take into account</param>
    /// <returns>Current SE</returns>
    public TData SE(UInt64 nextData)
    {
      if (first)
      {
        expMean = nextData;
        exp2Mean = nextData * nextData;
        first = false;
      }
      expMean = expMean - (expMean - nextData) / TAU;
      exp2Mean = exp2Mean - (exp2Mean - nextData * nextData) / TAU;

      // SE = sqrt(mean(X^2) - (mean(X))^2)
      return Math.Sqrt(exp2Mean - expMean * expMean);
      //return Math.Sqrt(Math.Abs(exp2Avg - expAvg * expAvg));
    }

    public void Reset()
    {
      first = true;
    }
  }
}
