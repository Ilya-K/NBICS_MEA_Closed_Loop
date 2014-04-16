using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MEAClosedLoop
{
  using TTime = UInt64;
  using TPackMap = List<uint>;
  using TStimIndex = System.Int16;
  using Timeline = Queue<ProcessedPack>;

  public struct TPack
  {
    public List<bool> data;
    public int channel;
  }

  public class ProcessedPack{
    public Dictionary<int, TPackMap> dataMap;  // <channel, data>
    public TTime start;

    public ProcessedPack()
    {
      dataMap = new Dictionary<int, TPackMap>();
      start = 0;
    }
  }

  public enum PackStatType
  {
    Amp,
    Freq,
    Both
  }

  public class PackGraph
  {
    
    public double foundPackPercent;
    private Queue<CPack> RawAmpData, RawFreqData;
    Timeline processed_data;
    PackStatType GraphType;
    public ulong totalTime;
    
    const int MAX_PACK_LENGTH = 12500; //500 ms 
    const int PACK_DETECTED_PERCENT_CRITERION = 50;
    const int STAT_ITERATION_LENGTH = 125; //5 ms



    private TPackMap SpikesInPack(TPack input)
    {
      TPackMap output = new TPackMap();
      uint TimeIterator;

      for (TimeIterator = 0; TimeIterator < input.data.Count(); TimeIterator ++)
      {
        if (input.data[(int)TimeIterator])
          output.Add(TimeIterator);
      }
      

      return output;
    }

    private TPack CPack2TPack(CPack input, int channel)
    {
      TPack output = new TPack();
      output.data = new List<bool>();

      output.channel = channel;
      for (int packIterator = 0; packIterator < input.Length; packIterator++)
      {
          output.data.Add((input.Data[channel])[packIterator] > input.NoiseLevel[channel]);
        
      }

      return output;
    }


    public void ProcessPackStat(int timeUnitSegment) //now from all channels
    {
      uint iterationLength = (uint)(totalTime / (ulong)timeUnitSegment);
      Queue<CPack> dataSource;

      //Filling timeline
      if ((RawFreqData.Count > 0) && (RawAmpData.Count > 0))
      {
        //Something went wrong
        GraphType = PackStatType.Both;
        throw new Exception("Неверные данные!"); //или оба режима сразу...
      }
      else
      {
          if (RawAmpData.Count == 0 && RawFreqData.Count == 0)
          {
            MessageBox.Show("Пачек не найдено");
            return;
          }
          if (RawFreqData.Count > 0)
          { //freq stat
            GraphType = PackStatType.Freq;
            dataSource = RawFreqData;
          }
          else //(RawAmpData.Count > 0)
          { //amp stat
            GraphType = PackStatType.Amp;
            dataSource = RawAmpData;
          }  
        
        TPack boolPack;
          foreach (CPack current_cpack in dataSource)
          {
            ProcessedPack processed_pack_to_add = new ProcessedPack();
            foreach (int channel in current_cpack.Data.Keys)
            {
              boolPack = CPack2TPack(current_cpack, channel);
              processed_pack_to_add.dataMap[channel] = SpikesInPack(boolPack);
            }
            processed_pack_to_add.start = current_cpack.Start;
            processed_data.Enqueue(processed_pack_to_add);
          }
      }
    }

    private uint PackAvgAmp(CPack input, int channel){
      uint output=0;
      uint i;

      for (i = 1; i < input.Data[channel].Length + 1; i++)
      {
        output += Convert.ToUInt16(Math.Abs(input.Data[channel][i - 1]));
      }
      output /= i;
      return output;
    }
    
    public PackGraph()
    {
      this.Reset();
    }

    public void Reset()
    {
      RawAmpData = new Queue<CPack>();
      RawFreqData = new Queue<CPack>();
      processed_data = new Timeline();
    }

    public void ProcessAmpStat(CPack pack_to_add)
    {
      RawAmpData.Enqueue(pack_to_add);
    }

    public void ProcessFreqStat(CPack pack_to_add)
    {
      RawFreqData.Enqueue(pack_to_add);

    }
    private ulong UpdateCursorPosition(ulong prevPosition, UInt64 val, ulong ticksInPoint)
    {
      return (prevPosition + 1) * ticksInPoint > (ulong)val ? prevPosition : prevPosition + 1;
    }
    public uint[] PrepareData(int channel, int panelWidth, int panelHeight)
    {
      uint[] output = null;
      ulong cursorPosition = 0;
      ulong ticksInPoint;
      //double maxOutputVal;
      Timeline local_data;
      uint ampsPerPoint;
      Queue<CPack>.Enumerator ampDataIterator = RawAmpData.GetEnumerator();
      lock (processed_data)
      {
        local_data = new Timeline(processed_data);
      }
      output = new uint[panelWidth];
      output.PopulateArray<uint>(0);
      if (local_data.Count == 0 || panelWidth ==0)
        return output;

      ticksInPoint = totalTime / (ulong)panelWidth;
      int debug_ldc = local_data.Count;
      int debug = 0;
      try
      {
        for (ProcessedPack currentPack = local_data.Dequeue(); local_data.Count > 0; currentPack = local_data.Dequeue())
        {
          debug++;
          if (currentPack.dataMap.ContainsKey(channel))
          {
            if (GraphType == PackStatType.Amp)
            {
              if (!ampDataIterator.MoveNext())
                break;
            }
            ampsPerPoint = 0;
            foreach (uint dataPoint in currentPack.dataMap[channel])
            {
              cursorPosition = UpdateCursorPosition(cursorPosition, dataPoint + currentPack.start, ticksInPoint);
              switch (GraphType)
              {
                case PackStatType.Freq:
                  if (cursorPosition < (ulong)panelWidth)
                    output[cursorPosition]++;
                  else return output;
                  break;
                case PackStatType.Amp: //TODO: calc avg amp and stat here
                  if (cursorPosition < (ulong)panelWidth)
                    output[cursorPosition] += Convert.ToUInt16(Math.Abs(ampDataIterator.Current.Data[channel][dataPoint]));
                  else return output;
                  if (UpdateCursorPosition(cursorPosition, dataPoint + currentPack.start, ticksInPoint) > cursorPosition && ampsPerPoint != 0)
                  {
                    output[cursorPosition] /= ampsPerPoint;
                    ampsPerPoint = 0;
                  }
                  else
                  {
                    ampsPerPoint++;
                  }
                  break;
              }
            }
          }
        }
      }
      catch(Exception ex)
      {
        throw ex;
      }
      return output;
    }
  }
}
