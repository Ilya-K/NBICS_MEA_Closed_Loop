using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MEAClosedLoop.Interfaces;

namespace MEAClosedLoop.Core
{
  class CCamReader : ICamReader
  {
    public event OnCamPacketReadyDelegate OnCamPacketReady;

    public CCamReader() { }


  }
}
