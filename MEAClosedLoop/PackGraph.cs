using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEAClosedLoop
{
  using TTime = UInt64;
  using TPackMap = List<uint>;

  public struct TPack
  {
    public List<bool> data;
    public TTime stimTime;
  }

  public class PackGraph
  {
    public double foundPackPercent;
    uint realMaxPackLength;
    
    const int MAX_PACK_LENGTH = 12500; //500 ms 
    const int PACK_DETECTED_PERCENT_CRITERION = 50;
    const int STAT_ITERATION_LENGTH = 125; //5 ms

    private TPackMap SpikesInPack(TPack input)
    {
      TPackMap output = new TPackMap();
      uint TimeIterator, TimePartIterator;
      uint packCount;

      for (TimeIterator = 0; TimeIterator < input.data.Count(); TimeIterator += STAT_ITERATION_LENGTH)
      {
        packCount = 0;
        for (TimePartIterator = TimeIterator; TimePartIterator <= TimeIterator + STAT_ITERATION_LENGTH; TimePartIterator++)
        {
          if (input.data[(int)TimePartIterator])
            packCount++;
        }
        output.Add(packCount);
      }
      
      if (TimeIterator > realMaxPackLength)
      {
        realMaxPackLength = TimeIterator;
      }

      return output;
    }


    public TPackMap ProcessPackStat(List<TPack> all_packs) //now from all channels
    {
      TPackMap current_spikes, output = new TPackMap(MAX_PACK_LENGTH);
      double foundPackIterator = 100.0 / (double)all_packs.Count();

      
      foreach (TPack PackIterator in all_packs)
      {
        current_spikes = SpikesInPack(PackIterator);
        if (current_spikes.Count() == 0) //no pack after stimulation
        {
          continue;
        }
        foundPackPercent += foundPackIterator;
        for(int i=0; i<current_spikes.Count(); i++){
          output[i] += current_spikes[i];
        }
      }

      if (realMaxPackLength == 0)
      {
        //something goes wrong
      }
      return output;
    }
    
    public PackGraph()
    {
      foundPackPercent = 0;
      realMaxPackLength = 0;
    }
  }
}
