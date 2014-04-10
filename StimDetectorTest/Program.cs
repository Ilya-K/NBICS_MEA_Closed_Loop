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
  using TTime = UInt64;
  using TStimIndex = System.Int16;
  using TAbsStimIndex = System.UInt64;

  class Program
  {
    private const int DAQ_FREQ = 25000;
    private const int TIME_MULT = DAQ_FREQ / 1000; //!< Коэффициент перевода времени в милисекунды

    private const TTime SINGLE_STIM_PERIOD = 10000 * TIME_MULT; //!< Период между одиночными стимулами (stimType = 1)

    private const int MULTI_PACK_NUM = 6; //!< Количество стимулов в пачке
    private const UInt16 MULTI_INNER_PERIOD = 10 * TIME_MULT; //!< Период между соседними стимулами внутри пачки
    private const UInt16 MULTI_PACK_PERIOD = 300 * TIME_MULT; //!< Период между пачками

    private const UInt16 MAX_TIME_NOISE = 9 * TIME_MULT; //!< Максимальный разброс приблизительного времени стимуляции

    private const TTime MAX_FILE_LENGTH = 800000 * TIME_MULT; //!< Максимальная длина входного файла


    private CMcsUsbListNet m_usbDAQList = new CMcsUsbListNet();
    static List<TStimGroup> sl_vary; //!< Приблизительный список моментов стимуляции
    static List<TStimGroup> sl_groups; //!< Точный список моментов стимуляции



    static TTime GenNoise(TTime dest, TTime maxNoise, Random m_random)
    {
      int randNoise = m_random.Next(0, Convert.ToInt32(2 * maxNoise));
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
            newStim.stimTime = timeIterator;
            output.Add(newStim);
          }
          break;
        case 2:
          newStim.count = 6;
          newStim.period = MULTI_INNER_PERIOD;
          for (timeIterator = start_time; timeIterator < totalTime; timeIterator += MULTI_PACK_PERIOD)
          {
            newStim.stimTime = timeIterator;
            output.Add(newStim);
          }
          break;
      }
      return output;
    }

    static List<TStimGroup> GenStimulVaryList(TTime start_time, Int32 stimType, TTime totalTime, Random m_random)
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
            newStim.stimTime = timeIterator + GenNoise(SINGLE_STIM_PERIOD, MAX_TIME_NOISE, m_random);
            output.Add(newStim);
          }
          break;
        case 2:
          newStim.count = MULTI_PACK_NUM;
          newStim.period = MULTI_INNER_PERIOD;
          for (timeIterator = start_time; timeIterator < totalTime; timeIterator += MULTI_PACK_PERIOD)
          {
            newStim.stimTime = timeIterator + GenNoise(MULTI_PACK_PERIOD, MAX_TIME_NOISE, m_random);
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
          output.Add(newIndex);
        }
      }



      return output;
    }


    private const string confName = "../../config.cfg";


    static void Main(string[] args)
    {
      long timeElapsed = 0;
      UInt64 errorRate = 0;
      int countOverhead = 0;

      using (StreamReader strReader = new StreamReader(confName)) //reading config
      {
        string[] ss;
        Random random = new Random(DateTime.Now.Millisecond);
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
            Console.Write("Building exact stimuli list... ");
            sl_groups = GenStimulList(stimStart, stimType, MAX_FILE_LENGTH);
            Console.WriteLine("Done.");
            Console.Write("Building noisy stimuli list... ");
            sl_vary = GenStimulVaryList(stimStart, stimType, MAX_FILE_LENGTH, random);
            Console.WriteLine("Done.");
          }
          else
          {
            sl_vary = null;
            sl_groups = null;
          }

          CDetectorTest tester = new CDetectorTest(fileName, sl_vary);

          Console.WriteLine("Test progress: ");
          List<TAbsStimIndex> foundStimIndices = tester.RunTest();
          Console.WriteLine("\nTest is completed");

          // Generate Stimuli list again, now with the real record length
          if (stimType != 0)
          {
            sl_groups = GenStimulList(stimStart, stimType, tester.RecordLength);
          }

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
              // [DONE] It seems to be fixed by regenerating of the stimuli list after the test
              countOverhead = commonStimsCount - realStimIndices.Count();
              commonStimsCount = realStimIndices.Count();
            }
            List<TAbsStimIndex> ExcessStimIndices = new List<TAbsStimIndex>();
            List<TAbsStimIndex> NotfoundStimIndices = new List<TAbsStimIndex>();
            /*
            for (int i = 0; i < foundStimIndices.Count(); i++)
            {
              if (!realStimIndices.Contains(foundStimIndices[i]) && !realStimIndices.Contains(foundStimIndices[i] + 1))
              {
                ExcessStimIndices.Add(foundStimIndices[i]);
                Console.WriteLine("excess abs stim index: {0}", foundStimIndices[i]);
              }

            }
            for (int i = 0; i < realStimIndices.Count(); i++)
            {
              if (!foundStimIndices.Contains(realStimIndices[i]) && !foundStimIndices.Contains(realStimIndices[i] - 1))
              {
                NotfoundedStimIndices.Add(realStimIndices[i]);
                Console.WriteLine("not found abs stim index: {0}", foundStimIndices[i]);
              }
            }
             */
            for (int i = 0; i < foundStimIndices.Count(); i++)
            {
              bool flag = false;
              for (int j = 0; j < realStimIndices.Count; j++)
              {
                if (Math.Abs((double)foundStimIndices[i] - (double)realStimIndices[j]) < 600)
                {
                  flag = true;
                  break;
                }
              }
              if (!flag)
              {
                ExcessStimIndices.Add(foundStimIndices[i]);
              }
            }
            for (int i = 0; i < realStimIndices.Count(); i++)
            {
              bool flag = false;
              for (int j = 0; j < foundStimIndices.Count; j++)
              {
                if (Math.Abs((double)foundStimIndices[j] - (double)realStimIndices[i]) < 240)
                {
                  flag = true;
                  break;
                }
              }
              if (!flag)
              {
                NotfoundStimIndices.Add(realStimIndices[i]);
              }
            }
            using (System.IO.StreamWriter exessly_found_file = new System.IO.StreamWriter("D:\\MCS_Data\\ExFound.txt"))
            {
              for (int i = 0; i < ExcessStimIndices.Count; i++)
              {
                Console.WriteLine("Exessly Founded Stim Index {0} : {1}", i, ExcessStimIndices[i]);
                exessly_found_file.WriteLine("{0}", ExcessStimIndices[i]);
              }
            }
            using (System.IO.StreamWriter not_found_file = new System.IO.StreamWriter("D:\\MCS_Data\\NotFound.txt"))
            {
              for (int i = 0; i < NotfoundStimIndices.Count; i++)
              {
                Console.WriteLine("NotFounded Stim Index {0} : {1}", i, NotfoundStimIndices[i]);
                not_found_file.WriteLine("{0}", NotfoundStimIndices[i]);
              }
            }

            for (int i = 0; i < commonStimsCount; i++)
            {
              TAbsStimIndex CurrentError = 0;
              if (foundStimIndices[i] > realStimIndices[i])
              {
                CurrentError = foundStimIndices[i] - realStimIndices[i];
                errorRate += CurrentError;
              }
              else
              {
                CurrentError = realStimIndices[i] - foundStimIndices[i];
                errorRate += CurrentError;
              }
              //Console.WriteLine("index " + i.ToString() + ": error: " + CurrentError.ToString());
            }
            Console.WriteLine("stims expected:  {0}", realStimIndices.Count());
            Console.WriteLine("stims found:     {0}", foundStimIndices.Count());
            Console.WriteLine("stims not found: {0}", realStimIndices.Count() - commonStimsCount);

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
      Console.ReadLine();
      Console.ReadKey();
    }

  }

}
