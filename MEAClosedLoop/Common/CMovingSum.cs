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
  public class CMovingSum
  {
    private int m_count = 0;
    private Queue<TData> q;
    private TData m_sum = 0;

    public CMovingSum(int n)
    {
      q = new Queue<TData>(n);
      for (int i = 0; i < n; ++i)
      {
        q.Enqueue(0);
      }
    }

    public TData Add(TData value)
    {
      m_sum += value - q.Dequeue();
      q.Enqueue(value);
      return m_sum;
    }
  }
}
