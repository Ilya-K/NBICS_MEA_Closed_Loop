using System;
using System.Collections.Generic;

namespace MEAClosedLoop
{
  using TData = System.Double;

  public delegate void OnStreamKillDelegate();
  public delegate void OnDataAvailableDelegate(Dictionary<int, System.Double[]> data);

  public static class Param
  {
    public const int DAQ_FREQ = 25000;
    public const int DEF_PACKET_LENGTH = 2500;        
    public const int MS = DAQ_FREQ / 1000;            // Number of ticks in 1 ms. Multiplier to convert ms to ticks
    public const int PRE_SPIKE = 8 * MS;              // (200)  How many points to store before the first spike in a train
    public const int POST_SPIKE = 200 * MS;           // (5000) How many points to store after the last spike in a train
    public const TData PRECISION = 1e-15;
  }

  public static class Helpers
  {
    public static void PopulateArray<T>(this T[] arr, T value)
    {
      for (int i = 0; i < arr.Length; i++)
      {
        arr[i] = value;
      }
    }
  }

  public static class MEA
  {
    public const int MAX_CHANNELS = 60;

    // Range of correct electrode names. But some names are invalid within this range (19, 20, 29, etc.)
    public const int ELECTRODE_FIRST = 12;
    public const int ELECTRODE_LAST = 87;

    // Map of electrode names [12..87] to array indicies [0..63]. Invalid names correspond to -1
    /*
    public static readonly int[] NAME2IDX = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                                              -1, -1,  0,  1,  2,  3,  4,  5, -1, -1,
                                              -1,  6,  7,  8,  9, 10, 11, 12, 13, -1,
                                              -1, 14, 15, 16, 17, 18, 19, 20, 21, -1,
                                              -1, 22, 23, 24, 25, 26, 27, 28, 29, -1,
                                              -1, 30, 31, 32, 33, 34, 35, 36, 37, -1,
                                              -1, 38, 39, 40, 41, 42, 43, 44, 45, -1,
                                              -1, 46, 47, 48, 49, 50, 51, 52, 53, -1,
                                              -1, -1, 54, 55, 56, 57, 58, 59, -1, -1 };
    */
    /*
    public static readonly int[] NAME2IDX = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                                              -1, -1,  6, 14, 22, 30, 38, 46, -1, -1,
                                              -1,  0,  7, 15, 23, 31, 39, 47, 54, -1,
                                              -1,  1,  8, 16, 24, 32, 40, 48, 55, -1,
                                              -1,  2,  9, 17, 25, 33, 41, 49, 56, -1,
                                              -1,  3, 10, 18, 26, 34, 42, 50, 57, -1,
                                              -1,  4, 11, 19, 27, 35, 43, 51, 58, -1,
                                              -1,  5, 12, 20, 28, 36, 44, 52, 59, -1,
                                              -1, -1, 13, 21, 29, 37, 45, 53, -1, -1 };
    */
    //                                         0   1   2   3   4   5   6   7   8   9
    public static readonly int[] NAME2IDX = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,   // 0
                                              -1, -1, 20, 18, 15, 14, 11,  9, -1, -1,   // 1
                                              -1, 23, 21, 19, 16, 13, 10,  8,  6, -1,   // 2
                                              -1, 25, 24, 22, 17, 12,  7,  5,  4, -1,   // 3
                                              -1, 28, 29, 27, 26,  3,  2,  0,  1, -1,   // 4
                                              -1, 31, 30, 32, 33, 56, 57, 59, 58, -1,   // 5
                                              -1, 34, 35, 37, 42, 47, 52, 54, 55, -1,   // 6
                                              -1, 36, 38, 40, 43, 46, 49, 51, 53, -1,   // 7
                                              -1, -1, 39, 41, 44, 45, 48, 50, -1, -1 }; // 8

   
    public static readonly int[] IDX2NAME = {     47, 48, 46, 45, 38, 37,
                                              28, 36, 27, 17, 26, 16, 35, 25,
                                              15, 14, 24, 34, 13, 23, 12, 22,
                                              33, 21, 32, 31, 44, 43, 41, 42,
                                              52, 51, 53, 54, 61, 62, 71, 63,
                                              72, 82, 73, 83, 64, 74, 84, 85,
                                              75, 65, 86, 76, 87, 77, 66, 78,
                                                  67, 68, 55, 56, 58, 57 };
    // Map of array indicies [0..63] to electrode names [12..87]
    /*
    public static readonly int[] IDX2NAME = {     12, 13, 14, 15, 16, 17,
                                              21, 22, 23, 24, 25, 26, 27, 28,
                                              31, 32, 33, 34, 35, 36, 37, 38,
                                              41, 42, 43, 44, 45, 46, 47, 48,
                                              51, 52, 53, 54, 55, 56, 57, 58,
                                              61, 62, 63, 64, 65, 66, 67, 68,
                                              71, 72, 73, 74, 75, 76, 77, 78,
                                                  82, 83, 84, 85, 86, 87 };
    */
    /*
    public static readonly int[] IDX2NAME = {     21, 31, 41, 51, 61, 71,
                                              12, 22, 32, 42, 52, 62, 72, 82,
                                              13, 23, 33, 43, 53, 63, 73, 83,
                                              14, 24, 34, 44, 54, 64, 74, 84,
                                              15, 25, 35, 45, 55, 65, 75, 85,
                                              16, 26, 36, 46, 56, 66, 76, 86,
                                              17, 27, 37, 47, 57, 67, 77, 87,
                                                  28, 38, 48, 58, 68, 78 };
    */
  }

  public struct TStimGroup
  {
    public UInt64 stimTime;
    public UInt16 period;
    public UInt16 count;
  }

}