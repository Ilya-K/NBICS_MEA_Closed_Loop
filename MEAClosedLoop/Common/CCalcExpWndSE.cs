using System;
using System.Text;

namespace MEAClosedLoop
{
  using TData = Double;

  // This class provides a method to calculate moving SE and Mean over exponential window with width = tau
  public class CCalcExpWndSE
  {
    private readonly int TAU;
    private int wuTAU;
    private TData expMean = 0;        // mean(X) in exponential window
    private TData exp2Mean = 0;       // mean(X^2) in exponential window
    private TData se = 0;             // previously calculated SE(X)
    public TData Mean { get { return expMean; } }
    public TData PrevSE { get { return se; } }
    public int Width { get { return TAU; } }

    /// <summary>
    /// Create SE calculator.
    /// Exponential multiplier decays to 1/e in tau samples, to ~0.13 in 2*tau, to 0.05 in 3*tau.
    /// Multiplier to be used in exponential sum, m = (tau-1)/tau.
    /// Consider 2*tau as a width of an exponential window.
    /// </summary>
    /// <param name="p">Number of samples to decay weight function to 1/e</param>
    public CCalcExpWndSE(int tau)
    {
      TAU = tau;
      expMean = 0;
      exp2Mean = 0;
      wuTAU = 1;
    }

    public void WarmedUp(bool warmedUp = true)
    {
      if (warmedUp) se = Math.Sqrt(exp2Mean - expMean * expMean);
      else wuTAU = 1;
    }

    /// <summary>
    /// Warm up SE calculator
    /// </summary>
    /// <param name="nextData">Next data point to take into account</param>
    /// <returns>Current Variance</returns>
    public TData WarmUp(TData nextData)
    {
      expMean = expMean - (expMean - nextData) / wuTAU;
      exp2Mean = exp2Mean - (exp2Mean - nextData * nextData) / wuTAU;
      ++wuTAU;
      return exp2Mean - expMean * expMean;
    }

    /// <summary>
    /// Calculate Standard Error (SE) of sequental data over exponential window with width ~2*tau
    /// </summary>
    /// <param name="nextData">Next data point to take into account</param>
    /// <returns>Current SE</returns>
    public TData SE(TData nextData)
    {
      expMean = expMean - (expMean - nextData) / TAU;
      exp2Mean = exp2Mean - (exp2Mean - nextData * nextData) / TAU;

      // SE = sqrt(mean(X^2) - (mean(X))^2)
      se = Math.Sqrt(exp2Mean - expMean * expMean);
      return se; 
      //return Math.Sqrt(Math.Abs(exp2Avg - expAvg * expAvg));
    }

    /// <summary>
    /// Calculate variance of sequental data over exponential window with width ~2*tau
    /// </summary>
    /// <param name="nextData">Next data point to take into account</param>
    /// <returns>Current Variance</returns>
    public TData Var(TData nextData)
    {
      expMean = expMean - (expMean - nextData) / TAU;
      exp2Mean = exp2Mean - (exp2Mean - nextData * nextData) / TAU;

      // Var = mean(X^2) - (mean(X))^2
      return exp2Mean - expMean * expMean;
    }

    /// <summary>
    /// Calculate Standard Error (SE) of sequental data over exponential window with width ~2*tau
    /// </summary>
    /// <param name="nextData">Next data point to take into account</param>
    /// <returns>Current SE</returns>
    public TData SE(UInt64 nextData)
    {
      expMean = expMean - (expMean - nextData) / TAU;
      exp2Mean = exp2Mean - (exp2Mean - nextData * nextData) / TAU;

      // SE = sqrt(mean(X^2) - (mean(X))^2)

      se = Math.Sqrt(exp2Mean - expMean * expMean); 
      return se; 
      //return Math.Sqrt(Math.Abs(exp2Avg - expAvg * expAvg));
    }

    /// <summary>
    /// Calculate variance of sequental data over exponential window with width ~2*tau
    /// </summary>
    /// <param name="nextData">Next data point to take into account</param>
    /// <returns>Current Variance</returns>
    public TData Var(UInt64 nextData)
    {
      expMean = expMean - (expMean - nextData) / TAU;
      exp2Mean = exp2Mean - (exp2Mean - nextData * nextData) / TAU;

      // Var = mean(X^2) - (mean(X))^2
      return exp2Mean - expMean * expMean;
    }

    public void Reset()
    {
      expMean = 0;
      exp2Mean = 0;
      wuTAU = 1;
    }
  }
}
