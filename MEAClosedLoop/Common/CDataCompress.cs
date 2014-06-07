using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace MEAClosedLoop.Common
{
  #region Definitions
  using TData = System.Double;
  using TTime = System.UInt64;
  using TStimIndex = System.Int16;
  using TAbsStimIndex = System.UInt64;
  using TRawData = UInt16;
  using TRawDataPacket = Dictionary<int, ushort[]>;
  using TFltDataPacket = Dictionary<int, System.Double[]>;
  using TCmpDataPacket = Dictionary<int, sbyte[]>;
  using TFltData = System.Double;
  using TCmpData = System.Byte;
  #endregion

  public class CDataCompress
  {
    //8000 - максимальная длина массива binary в базе данных MS SQL Server 2012
    int SplitLength = 4000;

    List<System.Byte[]> RawDataToBinary(TFltDataPacket DataPacket)
    {
      byte[] resultarray;
      byte[] subarray;
      int partNum;

      MemoryStream ms = new MemoryStream();
      BinaryFormatter formatter = new BinaryFormatter();
      formatter.Serialize(ms, DataPacket);
      resultarray = ms.ToArray();

      // разделим массив на части 
      partNum = 0;
      int currentPosition = 0;
      while (true)
      {
        bool endflag = false;

        if (currentPosition + SplitLength < resultarray.Length)
        {
          subarray = new byte[SplitLength];
          Array.Copy(resultarray, currentPosition, subarray, 0, SplitLength);
          currentPosition += SplitLength;
        }
        if (endflag) break;
      }

      return new List<Byte[]>();
    }
  }
}
