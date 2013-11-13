using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StimDetectorTest
{

  using MSTime = UInt64;

  public static class Helpers
  {
    public static MSTime Int2Time(UInt64 input)
    {
      //100ms = 2500
      MSTime output = input / 25;
      return output;
    }

    public static UInt64 Time2Int(MSTime input)
    {
      UInt64 output = input * 25;
      return output;
    }
  }
}
