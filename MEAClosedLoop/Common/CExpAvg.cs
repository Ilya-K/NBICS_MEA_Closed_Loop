using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TData = System.Double;

namespace MEAClosedLoop
{
  /// <summary>
  /// Class implements moving sum oveer n samples
  /// </summary>
  public class CExpAvg
  {
    private readonly int TAU;
    private TData m_sum = 0;

    public CExpAvg(int tau)
    {
      TAU = tau;
    }

    public TData Add(TData value)
    {
      m_sum = m_sum - (m_sum - value) / TAU;
      return m_sum;
    }
  }
}
