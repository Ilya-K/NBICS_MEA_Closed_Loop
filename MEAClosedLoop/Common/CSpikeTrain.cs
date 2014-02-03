using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEAClosedLoop
{
  using TTime = System.UInt64;
  using TData = System.Double;

  // Spike-train events can be of the two types:
  // S: Start, T: Tail (Stop)
  // data == null indicates S type of a pack event
  // Start means the start of a spike-train in the absolute time. 
  // PRE_SPIKE points are stored in data before the start of a spike-train
  // POST_SPIKE points are stored in data after the end of a spike-train

  public class CSpikeTrain
  {
    private TTime start;
    private TData[] data;
    private Int16 channel;
    public TTime Start { get { return start; } }
    public Int32 Length { get { return data.Length; } }
    public TData[] Data { get { return data; } set { data = value; } }
    public Int16 Channel { get { return channel; } }

    public bool EOP { get { return data != null; } }

    public CSpikeTrain(Int16 _channel, TTime _start, TData[] _data = null)
    {
      channel = _channel;
      start = _start;
      data = _data;
    }
  }

  // SpikeTrainFrame determines events the two types:
  // S: Start - EOP = false; T: Tail (Stop) - EOP = true
  // SpikeTrainFrame doesn't contain data
  // Start means the start of a spike-train in the absolute time
  // Length == 0 means that spike-train hasn't been finished yet
  // PRE_SPIKE points are stored in data before the start of a spike-train
  // POST_SPIKE points are stored in data after the end of a spike-train

  public class CSpikeTrainFrame
  {
    private TTime start;
    private Int32 length;
    private Int16 channel;
    public TTime Start { get { return start; } }
    public Int32 Length { get { return length; } }
    public Int16 Channel { get { return channel; } }

    public bool EOP { get { return length > 0; } }

    public CSpikeTrainFrame(Int16 _channel, TTime _start)
    {
      channel = _channel;
      start = _start;
      length = 0;
    }
  }

}
