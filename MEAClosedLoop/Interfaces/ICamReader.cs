using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEAClosedLoop.Interfaces
{
  public interface ICamReader
  {
    public event OnCamPacketReadyDelegate OnCamPacketReady;
  }
}
