using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MEAClosedLoop;
using UsbNetDll;
using Neurorighter;
using System.Threading;
using System.Diagnostics;
using System.IO;
using MEAClosedLoop.Properties;

namespace StimDetectorTest
{
  using StimuliList = List<TStimGroup>;
  using MSTime = UInt64;
  using TStimIndex = System.Int16;

  class Program
  {
    List<int> m_channelList;
    //CInputStream m_inputStream;
    private int m_selectedDAQ = -1;
    private int m_fileIdx = -1;
    //private string m_fileOpened = "";  //не нужна
    private bool m_DAQConfigured = false;
    private CMcsUsbListNet m_usbDAQList = new CMcsUsbListNet();
    Thread m_dataLoopThread;
    static StimuliList sl_vary; //with noise
    static List<TStimIndex> sl_indices;
    const MSTime SINGLE_IMP_RANGE = 10000;

    const int STIM_PACK_NUM = 6;
    const MSTime STIM_PACK_IMP_RANGE = 340;
    const MSTime STIM_PACK_PER = 10;

    const MSTime MAX_TIME_NOISE = 10;

    const MSTime DO_HRENA = 800000;
   


    static MSTime GenNoise(MSTime dest, MSTime maxNoise)
    {
      Random random = new Random();
      int randNoise = random.Next(0, Convert.ToInt32(2*maxNoise));
      return dest - maxNoise + Convert.ToUInt64(randNoise);
    }

    static List<TStimIndex> GenStimulList(MSTime start_time, Int32 stimType, MSTime totalTime)
    {
      MSTime timeIterator;
      TStimGroup newStim;
      List<TStimIndex> output = new List<TStimIndex>();
      
      switch (stimType)
      {
        case 1:
          newStim.count = 1;
          newStim.period = 0;
          for (timeIterator = start_time; timeIterator < totalTime; timeIterator += SINGLE_IMP_RANGE)
          {
            newStim.stimTime = Helpers.Time2Int(timeIterator);
            output.Add(Convert.ToInt16(Helpers.Time2Int(timeIterator)%2500));
            //output.Add(newStim);
          }
          break;
        case 2:
          newStim.count = 6;
          newStim.period = Convert.ToUInt16(Helpers.Time2Int(STIM_PACK_PER));
          for (timeIterator = start_time; timeIterator < totalTime; timeIterator += STIM_PACK_IMP_RANGE)
          {
            newStim.stimTime = Helpers.Time2Int(timeIterator);
            output.Add(Convert.ToInt16(Helpers.Time2Int(timeIterator)%2500));
            //output.Add(newStim);
          }
          break;
      }
      return output;

    }

    static StimuliList GenStimulVaryList(MSTime start_time, Int32 stimType, MSTime totalTime)
    {
      MSTime timeIterator;
      TStimGroup newStim;
      StimuliList output = new StimuliList();

      switch (stimType)
      {
        case 1:
          newStim.count = 1;
          newStim.period = 0;
          for (timeIterator = start_time; timeIterator < totalTime; timeIterator += GenNoise(SINGLE_IMP_RANGE, MAX_TIME_NOISE))
          {
            newStim.stimTime = Helpers.Time2Int(timeIterator);
            output.Add(newStim);
          }
          break;
        case 2:
          newStim.count = 6;
          newStim.period = Convert.ToUInt16(Helpers.Time2Int(STIM_PACK_PER));
          for (timeIterator = start_time; timeIterator < totalTime; timeIterator += GenNoise(STIM_PACK_IMP_RANGE, MAX_TIME_NOISE))
          {
            newStim.stimTime = Helpers.Time2Int(timeIterator);
            output.Add(newStim);
          }
          break;
      }
      return output;

    }


    private const string confName = "../../config.cfg";

    // does not work
    /*static string ParseConfigLine(string s, out int stimType, out int stimStart) {
        int CurrentStringPosition = 0;
        string returnValue;
        stimType = Int32.Parse(s); //channel type
        CurrentStringPosition = s.IndexOf(' ') + 1;

        stimStart = Int32.Parse(s.Substring(CurrentStringPosition)); //time x40
        CurrentStringPosition = s.IndexOf(' ', CurrentStringPosition) + 1;
            
        returnValue = s.Substring(CurrentStringPosition); //filename

        Console.WriteLine("\tDEBUG: ChannelType:    {0}", stimType);
        Console.WriteLine("\tTIME:           {0}", stimStart);
        Console.WriteLine("\tFilename:           {0}", returnValue);
        Console.WriteLine();

        return returnValue;
    }*/



    static void Main(string[] args)
    {
      //CInputStream In = new CInputStream(, m_channelList, 2500); //deault
      //int errorRate = 0;
      Stopwatch sw1 = new Stopwatch();
      UInt64 errorRate = 0;


      using (StreamReader strReader = new StreamReader(confName)) //reading config
      {
        string[] ss;
        while (!strReader.EndOfStream)
        {
          Int32 stimType, stimStart;
          string fileName;
          ss = strReader.ReadLine().Split(new Char[] { ' ', ' ' });
          stimType = Int32.Parse(ss[0]);
          stimStart = Int32.Parse(ss[1]);
          fileName = ss[2];

          Console.WriteLine("\tFILE: {0}", fileName);


          sl_indices = GenStimulList(Helpers.Int2Time(Convert.ToUInt64(stimStart)), stimType, DO_HRENA);
          sl_vary = GenStimulVaryList(Helpers.Int2Time(Convert.ToUInt64(stimStart)), stimType, DO_HRENA);


          CDetectorTest tester = new CDetectorTest(fileName, sl_vary);

          sw1.Start();
          errorRate+=tester.RunTest(sl_indices);
          sw1.Stop();
          Console.WriteLine("\tTIME (in milliseconds): " + sw1.ElapsedMilliseconds.ToString());
        }
      }

      Console.WriteLine("TOTAL ERROR: {0}", errorRate);

    }

  }

}
