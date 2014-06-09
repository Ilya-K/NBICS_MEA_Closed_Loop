using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Text;
using System.Runtime.Serialization;
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
  using TCmpData = System.Byte;
  #endregion

  [Table("Experiments")]
  public class Experiment
  {
    [Key]
    public int id { get; set; }
    public string Author { get; set; }
    public string About { get; set; }
    public string Target { get; set; }
    public DateTime CreationTime { get; set; }
    public virtual ICollection<MeasureManager> MeasureParts { get; set; }
  }
  [Table("MeasureManagers")]                                  
  public class MeasureManager
  {
    [Key]
    public int id { get; set; }
    public int? ExperimentID { get; set; }
    public virtual Experiment Experiment { get; set; }
    public string About { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime StartTime { get; set; }

    public virtual ICollection<PackData> Packs { get; set; }
    public virtual ICollection<StimData> Stims { get; set; }
    public virtual ICollection<CmpData> CmpData { get; set; }
    public virtual ICollection<RawData> RawData { get; set; }
  }
  [Table("BinRawData")]
  public class RawData
  {
    [Key]
    public int id { get; set; }
    public int? MeasureID { get; set; }
    public virtual MeasureManager MeasureManager { get; set; }
    byte[] binData { get; set; }
  }
  [Table("BinCompressedData")]
  [SerializableAttribute]
  public class CmpData
  {
    [Key]
    public int id { get; set; }
    public int? MeasureID { get; set; }
    public virtual MeasureManager MeasureManager { get; set; }
    public TCmpData[] CompressedData { get; set; }
    public int CompressRate { get; set; }
    byte[] binData { get; set; }

    /*
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
    */
  }
  [Table("Packs")]
  public class PackData
  {
    [Key]
    public int id { get; set; }
    public int? MeasureID { get; set; }
    public virtual MeasureManager MeasureManager { get; set; }
    public TTime Time { get; set; }
    public int Length { get; set; }
    public PackData(CPack pack)
    {
      Time = pack.Start;
      Length = pack.Length;
    }
  }
  [Table("Stims")]
  public class StimData
  {
    [Key]
    public int id { get; set; }
    public int? MeasureID { get; set; }
    public virtual MeasureManager MeasureManager { get; set; }
    public TTime Time { get; set; }
  }
  public class ExpDataContext : DbContext
  {
    public ExpDataContext()
      : base("somedataConnectionString")
    { }
    public ExpDataContext(string connection_string)
      : base(connection_string)
    { }
    public DbSet<Experiment> Experiments { get; set; }
    public DbSet<MeasureManager> Manager { get; set; }
    public DbSet<CmpData> CompressedData { get; set; }
    public DbSet<RawData> RawData { get; set; }
    public DbSet<PackData> PackData { get; set; }
    public DbSet<StimData> StimData { get; set; }
  }
}
