using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEAClosedLoop
{
  #region Definitions
  using TTime = System.UInt64;
  using TData = System.Double;
  using TFltDataPacket = Dictionary<int, System.Double[]>;
  using TStimIndex = System.Int16;
  using TAbsStimIndex = System.UInt64;
  using TRawData = UInt16;
  using TRawDataPacket = Dictionary<int, ushort[]>;
  #endregion
  public interface IRecieveBusrt
  {
     void RecieveBurst(CPack Burst);
  }
  public interface IRecieveFltData
  {
     void RecieveFltData(TFltDataPacket packet);
  }
  public interface IRecieveStim
  {
     void RecieveStim(TAbsStimIndex stim);
  }
}
