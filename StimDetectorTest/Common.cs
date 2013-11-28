using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StimDetectorTest
{

  using TTime = UInt64;

  public static class Helpers
  {
    public static TTime Int2Time(TTime input)
    {
      //100ms = 2500
      TTime output = input / 25;
      return output;
    }

    public static TTime Time2Int(TTime input)
    {
      TTime output = input * 25;
      return output;
    }
  }
}
