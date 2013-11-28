﻿using System;
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
  using TTime = UInt64;
  using TStimIndex = System.Int16;
  using TAbsStimIndex = System.UInt64;

  class Program
  {
    private const int DAQ_FREQ = 25000;
    private const int TIME_MULT = DAQ_FREQ / 1000;

    private const TTime SINGLE_STIM_PERIOD = 10000 * TIME_MULT;

    private const int MULTI_PACK_NUM = 6;
    private const UInt16 MULTI_INNER_PERIOD = 10 * TIME_MULT;
    private const TTime MULTI_PACK_PERIOD = 300 * TIME_MULT;

    private const UInt16 MAX_TIME_NOISE = 10 * TIME_MULT;

    private const TTime MAX_FILE_LENGTH = 800000 * TIME_MULT;

    List<int> m_channelList;
    //CInputStream m_inputStream;
    private int m_selectedDAQ = -1;
    private int m_fileIdx = -1;
    //private string m_fileOpened = "";  //не нужна
    private bool m_DAQConfigured = false;
    private CMcsUsbListNet m_usbDAQList = new CMcsUsbListNet();
    Thread m_dataLoopThread;
    static List<TStimGroup> sl_vary; //with noise
    static List<TStimGroup> sl_groups; //exact

    static TTime GenNoise(TTime dest, TTime maxNoise)
    {
      Random random = new Random();
      int randNoise = random.Next(0, Convert.ToInt32(2*maxNoise));
      return dest - maxNoise + Convert.ToUInt64(randNoise);
    }

    static List<TStimGroup> GenStimulList(TTime start_time, Int32 stimType, TTime totalTime)
    {
      TTime timeIterator;
      TStimGroup newStim;
      List<TStimGroup> output = new List<TStimGroup>();
      
      switch (stimType)
      {
        case 1:
          newStim.count = 1;
          newStim.period = 0;
          for (timeIterator = start_time; timeIterator < totalTime; timeIterator += SINGLE_STIM_PERIOD)
          {
            // newStim.stimTime = Helpers.Time2Int(timeIterator);
            newStim.stimTime = timeIterator;
            //output.Add(Convert.ToInt16(Helpers.Time2Int(timeIterator)%2500));
            output.Add(newStim);
          }
          break;
        case 2:
          newStim.count = 6;
          // newStim.period = Convert.ToUInt16(Helpers.Time2Int(STIM_PACK_PER));
          newStim.period = MULTI_INNER_PERIOD;
          for (timeIterator = start_time; timeIterator < totalTime; timeIterator += MULTI_PACK_PERIOD)
          {
            newStim.stimTime = Helpers.Time2Int(timeIterator);
            newStim.stimTime = timeIterator;
            //output.Add(Convert.ToInt16(Helpers.Time2Int(timeIterator)%2500));
            output.Add(newStim);
          }
          break;
      }
      return output;
    }

    static List<TStimGroup> GenStimulVaryList(TTime start_time, Int32 stimType, TTime totalTime)
    {
      TTime timeIterator;
      TStimGroup newStim;
      List<TStimGroup> output = new List<TStimGroup>();

      switch (stimType)
      {
        case 1:
          newStim.count = 1;
          newStim.period = 0;
          for (timeIterator = start_time; timeIterator < totalTime; timeIterator += GenNoise(SINGLE_STIM_PERIOD, MAX_TIME_NOISE))
          {
            newStim.stimTime = Helpers.Time2Int(timeIterator);
            output.Add(newStim);
          }
          break;
        case 2:
          newStim.count = 6;
          newStim.period = Convert.ToUInt16(Helpers.Time2Int(MULTI_PACK_PERIOD));
          for (timeIterator = start_time; timeIterator < totalTime; timeIterator += GenNoise(MULTI_PACK_PERIOD, MAX_TIME_NOISE))
          {
            newStim.stimTime = Helpers.Time2Int(timeIterator);
            output.Add(newStim);
          }
          break;
      }
      return output;

    }

    static List<TAbsStimIndex> Groups2Indices(List<TStimGroup> input)
    {
      List<TAbsStimIndex> output = new List<TAbsStimIndex>();
      int localCount;
      TAbsStimIndex newIndex;

      foreach (TStimGroup groupIterator in input)
      {
        for (localCount = 0; localCount < groupIterator.count; localCount++)
        {
          newIndex = groupIterator.stimTime + (UInt64)(localCount * groupIterator.period);
        }
      }



      return output;
    }


    private const string confName = "config.cfg";

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
      long timeElapsed = 0;
      UInt64 errorRate = 0;
      int countOverhead = 0;

      using (StreamReader strReader = new StreamReader(confName)) //reading config
      {
        string[] ss;
        while (!strReader.EndOfStream)
        {
          Int32 stimType;
          UInt64 stimStart;
          string fileName;
          string str = strReader.ReadLine();
          if (str[0] == '#') continue;
          ss = str.Split(new Char[] { ' ', '\t' });
          stimType = Int32.Parse(ss[0]);
          stimStart = UInt64.Parse(ss[1]);
          fileName = ss[2];

          Console.WriteLine("\tFILE: {0}", fileName);

          if (stimType != 0)
          {
            //sl_groups = GenStimulList(Helpers.Int2Time(Convert.ToUInt64(stimStart)), stimType, MAX_FILE_LENGTH);
            //sl_vary = GenStimulVaryList(Helpers.Int2Time(Convert.ToUInt64(stimStart)), stimType, MAX_FILE_LENGTH);
            sl_groups = GenStimulList(stimStart, stimType, MAX_FILE_LENGTH);
            sl_vary = GenStimulVaryList(stimStart, stimType, MAX_FILE_LENGTH);
          }
          else
          {
            sl_vary = null;
            sl_groups = null;
          }
          
          CDetectorTest tester = new CDetectorTest(fileName, sl_vary);

          List<TAbsStimIndex> foundStimIndices = tester.RunTest();

          List<TAbsStimIndex> realStimIndices = Groups2Indices(sl_groups);

          if (realStimIndices == null)
      {
        errorRate = Convert.ToUInt64(foundStimIndices.Count());
      }
      else
      {
        int commonStimsCount = foundStimIndices.Count();
        if (commonStimsCount > realStimIndices.Count())
        {
          // [TODO] CountOverhead считается не точно, т.к. объём realStimIndices сильно завышен.
          // Но это не страшно, т.к. в случае, если стимулов найдётся больше чем заказывали, то сильно увеличится суммарная ошибка.
          countOverhead = commonStimsCount - realStimIndices.Count();
          commonStimsCount = realStimIndices.Count();
        }
        
        for (int i = 0; i < commonStimsCount; i++)
        {
          // errorRate += Helpers.Int2Time(Convert.ToUInt64(Math.Abs(foundStimIndices[i] - realStimIndices[i])));
          if (foundStimIndices[i] > realStimIndices[i])
          {
            errorRate += foundStimIndices[i] - realStimIndices[i];
          }
          else
          {
            errorRate += realStimIndices[i] - foundStimIndices[i];
          }

        }
      }



          timeElapsed += tester.TimeElapsed;
          countOverhead += tester.NumberExceeded;

          Console.WriteLine("\tTIME (in milliseconds): " + timeElapsed.ToString());
          if (countOverhead > 0)
          {
            Console.WriteLine("\tThe maximum expected number of artifacts has been exceeded by: " + countOverhead.ToString());
          }
        }
      }

      Console.WriteLine("TOTAL ERROR: {0}", errorRate);
      Console.ReadKey();
    }

  }

}
