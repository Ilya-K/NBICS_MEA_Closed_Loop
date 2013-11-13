using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEAClosedLoop
{
  using TTime = System.UInt64;
  using TData = System.Double;

  // Pack events can be of two types:
  // S: Start, T: Stop
  // Length < 0 indicates T type of a pack event
  // In this case Start means end of a pack.

  public class CPack
  {
    private TTime start;
    private Int32 length;
    private TData[] data;
    public TTime Start { get { return start; } }
    public Int32 Length { get { return length; } set { length = Length; } }
    public TData[] Data { get { return data; } }
    public bool EOP { get { return length != 0; } }

    public CPack(TTime _start, Int32 _length, TData[] _data)
    {
      start = _start;
      length = _length;
      data = _data;
    }
  }
}
