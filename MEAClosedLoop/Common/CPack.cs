using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
namespace MEAClosedLoop
{
  using TTime = System.UInt64;
  using TData = System.Double;
  using TFltDataPacket = Dictionary<int, System.Double[]>;
  using TSpikesBinary = Dictionary<int, bool[]>;
  using TAbsStimIndex = System.UInt64;

  // Pack events can be of two types:
  // S: Start, EOP = false; T: Stop, EOP = true
  // Length > 0 indicates T type of a pack event
  // In this case Start time means time of the pack end.
  // Data contains Param.PRE_SPIKE samples before the first spike and Param.POST_SPIKE samples after the EOP
  // But Start points the real start of the pack, i.e. the absolute time of the first spike in the pack

  public class CPack
  {
    private TTime start;
    private Int32 length;
    private TData[] noiseLevel;
    private TFltDataPacket data;
    public TTime Start { get { return start; } }
    public TData[] NoiseLevel { get { return noiseLevel; } }
    public Int32 Length { get { return length; } set { length = Length; } }
    public TFltDataPacket Data { get { return data; } }
    public bool EOP { get { return length != 0; } }
    public TFltDataPacket description;

    public CPack(TTime _start, Int32 _length, TFltDataPacket _data = null, TData[] _noiseLevel = null)
    {
      start = _start;
      length = _length;
      data = _data;
      noiseLevel = _noiseLevel;
      description = null;
    }
    /// <summary>
    /// Заполняет внутренний масив данными описательной функции
    /// по формуле f[x] = f[x-1] + U[x] / 30 - f[x-1]/Decrease
    /// </summary>
    /// <param name="windowWidth"> Длина окна сглаживания</param>
    /// <param name="Decrease"> Обратная скорость убывания</param>
    public void BuildDescription(int windowWidth = 140, int Decrease = 460)
    {
      Stopwatch w = new Stopwatch();
      w.Start();
      int window = windowWidth;
      if (data.Keys.Count == 0) return;
      double[] averages = new double[Data[Data.Keys.First()].Length];
      foreach (int key in data.Keys)
      //int key = 0;
      {
        Average stat = new Average();
        for (int i = 0; i < 200; i++)
        {
          if (Data[key][i] > 0) stat.AddValueElem(Data[key][i]);
        }
        stat.Calc();
        double[] fxs = new double[Data[key].Length];
        fxs[0] = Math.Abs(Data[key][0]);
        double last;

        for (int i = 1; i < Data[key].Length; i++)
        {
          last = fxs[i - 1];
          fxs[i] = (Data[key][i] > stat.Sigma * 3) ? last + Math.Abs(Data[key][i]) / 30 - last / 460 : last - last / 460;
        }
        
        double average = stat.Sigma * 4;
        for (int i = 0; i < window; i++)
        {
          averages[i] = average;
        }
        for (int i = window; i < Data[key].Length; i++)
        {
          average += (fxs[i] - fxs[i - window]) / (double)window;
          averages[i] = average > 0 ? average : 0;
        }
      }
      w.Stop();

    }
    
  }
  public struct SEvokedPack
  {
    public CPack Burst;
    public TAbsStimIndex stim;
  }
}
