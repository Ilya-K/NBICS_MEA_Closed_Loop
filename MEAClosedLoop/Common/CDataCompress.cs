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

  public static class CDataCompress
  {

    public static System.Byte[] RawDataToBinary(TFltDataPacket DataPacket)
    {
      byte[] resultarray;
      
      MemoryStream ms = new MemoryStream();
      BinaryFormatter formatter = new BinaryFormatter();
      formatter.Serialize(ms, DataPacket);
      resultarray = ms.ToArray();
      return resultarray;
    }
    public static System.Byte[] RawDataToCmpBinary(TFltDataPacket DataPacket, int CompressRate)
    {
      byte[] resultarray;

      MemoryStream ms = new MemoryStream();
      BinaryFormatter formatter = new BinaryFormatter();
      TCmpDataPacket cmpDataPacket = new TCmpDataPacket();
      CompressRate = CompressRate > 4 ? 5 : CompressRate; // а нафига козе баян?
      foreach (int key in DataPacket.Keys)
      {
        
        sbyte[] CmpArray = new sbyte[DataPacket[key].Length];
        for (int i = 0; i < CmpArray.Length; i++)
        {
          if (DataPacket[key][i] > sbyte.MaxValue * CompressRate)
          {
            CmpArray[i] = sbyte.MaxValue;
            continue;
          }
          if (DataPacket[key][i] < sbyte.MinValue * CompressRate)
          {
            CmpArray[i] = sbyte.MinValue;
            continue;
          }
          CmpArray[i] = (sbyte)(DataPacket[key][i] / CompressRate);
          //CmpArray[i] = (DataPacket[key][i] > sbyte.MaxValue * CompressRate) ? (sbyte) 127 * CompressRate : (sbyte)(DataPacket[key][i] / CompressRate);

        }
        cmpDataPacket.Add(key, CmpArray);
      } 
      formatter.Serialize(ms, cmpDataPacket);
      resultarray = ms.ToArray();
      return resultarray;
    }
    public static TFltDataPacket BinaryCmpToRawData(int compressRate)
    {
      return new TFltDataPacket();
    }
    public static TFltDataPacket BinaryRawToRawData(Byte[] data)
    {
      MemoryStream ms = new MemoryStream(data, false);
      TFltDataPacket result = new TFltDataPacket();
      BinaryFormatter formatter = new BinaryFormatter();
      
      //System.Runtime.Remoting.Messaging.Header header = new System.Runtime.Remoting.Messaging.Header();
      
      result = (TFltDataPacket)formatter.Deserialize(ms);

      return result;
    }
  }
}
