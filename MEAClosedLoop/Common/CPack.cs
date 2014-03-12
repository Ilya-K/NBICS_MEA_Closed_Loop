using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEAClosedLoop
{
  using TTime = System.UInt64;
  using TData = System.Double;
  using TFltDataPacket = Dictionary<int, System.Double[]>;
  using TSpikesBinary =  Dictionary<int, bool[]>;
  
  // Pack events can be of two types:
  // S: Start, EOP = false; T: Stop, EOP = true
  // Length > 0 indicates T type of a pack event
  // In this case Start time means time of the pack end.

  public class CPack
  {
    private TTime start;
    private Int32 length;
    private TFltDataPacket data;
    public TTime Start { get { return start; } }
    public Int32 Length { get { return length; } set { length = Length; } }
    public TFltDataPacket Data { get { return data; } }
    public bool EOP { get { return length != 0; } }

    public CPack(TTime _start, Int32 _length, TFltDataPacket _data)
    {
      start = _start;
      length = _length;
      data = _data;
    }
  }
}
