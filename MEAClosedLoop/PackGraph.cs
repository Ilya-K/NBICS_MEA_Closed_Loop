using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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



  

  public class PackGraph
  {
    public enum state
    {
      Amp,
      Freq
    }
    public double foundPackPercent;
    uint realMaxPackLength;
    //List<TStimIndex> indexData;
    private Queue<CPack> RawAmpData, RawFreqData;
    Timeline processed_data;
    state GraphType;
    public ulong totalTime;
    
    const int MAX_PACK_LENGTH = 12500; //500 ms 
    const int PACK_DETECTED_PERCENT_CRITERION = 50;
    const int STAT_ITERATION_LENGTH = 125; //5 ms
    const double SPIKE_TRESHOLD = 10; //TODO: vary this  parameter



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

          //output.data[packIterator] = ((input.Data[channel])[packIterator] > SPIKE_TRESHOLD);
          output.data.Add((input.Data[channel])[packIterator] > SPIKE_TRESHOLD);
        
      }

      return output;
    }


    public void ProcessPackStat(int timeUnitSegment) //now from all channels
    {
      uint iterationLength = (uint)(totalTime / (ulong)timeUnitSegment);

      //Filling timeline
      if ((RawFreqData.Count > 0) && (RawAmpData.Count > 0))
      {
        //Something went wrong
          throw new Exception("Неверные данные!");
      }
      else
      {
          if (RawAmpData.Count == 0 && RawFreqData.Count == 0)
          {
              throw new Exception("Пачек не найдено");
          }
        if (RawFreqData.Count > 0)
        { //freq stat
          GraphType = state.Freq;
          foreach (CPack current_cpack in RawFreqData)
          {
            ProcessedPack processed_pack_to_add = new ProcessedPack();
            foreach (int channel in current_cpack.Data.Keys)
            {
                processed_pack_to_add.dataMap[channel] = SpikesInPack(CPack2TPack(current_cpack, channel));
            }
            processed_pack_to_add.start = current_cpack.Start;
            processed_data.Enqueue(processed_pack_to_add);
          }
        }
        if (RawAmpData.Count > 0)
        { //amp stat
          GraphType = state.Amp;
        }
      }
    }
    
    public PackGraph()
    {
      foundPackPercent = 0;
      realMaxPackLength = 0;

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
    public Queue<uint> PrepareData(int channel) //TODO: proper time convertions
    {
      Queue<uint> output = new Queue<uint>();
      //uint nextPackTime = (uint)processed_data.Peek().start; //here //minimum 1 pack required
      ProcessedPack currentPack;

      if (processed_data.Count == 0) return output;

      for (int timeIterator = 0; ((ulong)timeIterator < totalTime) && (processed_data.Count > 0); timeIterator++)
      {
        if (timeIterator < (uint)processed_data.Peek().start)
        { //filling free space
          output.Enqueue(0);
          continue;
        }
        else
        {
          currentPack = processed_data.Dequeue();
          timeIterator += currentPack.dataMap.Count;
          foreach (uint dataPoint in currentPack.dataMap[channel])
          {
            output.Enqueue(dataPoint);
          }
        }
      }

      return output;
    }
  }
}
