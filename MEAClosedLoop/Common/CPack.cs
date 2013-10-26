using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEAClosedLoop
{
  using TTime = System.UInt64;
  using TData = System.Double;

  public class CPack
  {
    private TTime start;
    private Int32 length;
    private TData[] data;
    public TTime Start { get { return start; } }
    public Int32 Length { get { return length; } }
    public TData[] Data { get { return data; } }

    public CPack(TTime _start, Int32 _length, TData[] _data)
    {
      start = _start;
      length = _length;
      data = _data;
    }
  }
}
