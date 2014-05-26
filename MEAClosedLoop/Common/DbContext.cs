﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Text;
using MEAClosedLoop;
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
  #endregion
  public class ExpDataContext : DbContext
  {
    public ExpDataContext()
      : base("somedataConnectionString")
    { }
    public DbSet<Experiment> Experiments { get; set; }
    public DbSet<RawDataList> RawDataList { get; set; }
    public DbSet<MeasureManager> Manager { get; set; }
  }
  [Table("Experiments")]
  public class Experiment
  {
    [Key]
    public int id {get; set;}
    public string Author {get; set;}
    public string About {get; set;}
    public string Target {get; set;}
    public DateTime CreationTime { get; set; }
  }
  [Table("MeasureManagers")]
  public class MeasureManager
  {
    [Key]
    public int id { get; set;}
    public int? ExperimentID { get; set; }
    public string About { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime StartTime { get; set; }
  }
  [Table("RawData")]
  public class RawDataList
  {
    [Key]
    public int id { get; set; }
    public int? MeasureID { get; set; }
    public MeasureManager Measure { get; set; }
    public byte Value { get; set; }
    public TFltDataPacket CompressedData { get; set; }
  }
  [Table("CompressedData")]
  public class CmpData
  {
    [Key]
    public int id { get; set; }
    public int? MeasureID { get; set; }
    public MeasureManager Measure { get; set; }
    public TCmpDataPacket CompressedData { get; set; }
    public int CompressRate { get; set; }
    public CmpData(TFltDataPacket DataPacket)
    {
      CompressRate = 4;
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
//          CmpArray[i] = (DataPacket[key][i] > sbyte.MaxValue * CompressRate) ? (sbyte) 127 * CompressRate : (sbyte)(DataPacket[key][i] / CompressRate);
          
        }
//        CompressedData;
      }
    }
  }
  [Table("Packs")]
  public class PackData
  {
    [Key]
    public int id { get; set; }
    public int MeasureID { get; set; }
    public TTime Time { get; set; }
    public int Length { get; set; }
    public PackData(CPack pack)
    {
      Time = pack.Start;
      Length = pack.Length;
    }
  }
  [Table("Stims")]
  public class Stim
  {
    [Key]
    public int id { get; set; }
    public int MeasureID { get; set; }
    public TTime Time { get; set; }
  }
}
