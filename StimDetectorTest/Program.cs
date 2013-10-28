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

namespace StimDetectorTest
{
    class Program
    {
        List<int> m_channelList;
        CInputStream m_inputStream;
        private int m_selectedDAQ = -1;
        private int m_fileIdx = -1;
        private string m_fileOpened = "";
        private bool m_DAQConfigured = false;
        private CMcsUsbListNet m_usbDAQList = new CMcsUsbListNet();
        Thread m_dataLoopThread;

         
        
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
            Stopwatch sw1 = new Stopwatch();

            
            using (StreamReader strReader = new StreamReader(confName)) //reading config
            {
                string[] ss;
                while (!strReader.EndOfStream)
                {
                    int stimType, stimStart;
                    string fileName;
                    ss = strReader.ReadLine().Split(new Char[] { ' ', ' ' });
                    stimType = Int32.Parse(ss[0]);
                    stimStart = Int32.Parse(ss[1]);
                    fileName = ss[2];

                    //DEBUG
                    Console.WriteLine("\tChannelType:           {0}", stimType);
                    Console.WriteLine("\tTIME:                  {0}", stimStart);
                    Console.WriteLine("\tFilename:              {0}", fileName);
                    Console.WriteLine();
                    Console.ReadKey(true); //getch
                }
            }


        }

        void Init()
        {
            // Check if all necessary components of DAQ have been already created
            if (!m_DAQConfigured)
            {
                // Configure Input Stream and filters here
                if (m_inputStream == null)
                {
                    if (m_selectedDAQ == m_fileIdx)
                    {
                        m_inputStream = new CInputStream(m_fileOpened, m_channelList, 2500);
                    }
                    else
                    {
                        m_inputStream = new CInputStream(m_usbDAQList, 0, m_channelList, 2500);
                    }
                }

                // (int)SpikeFiltOrder.Value, 25000.0, Convert.ToDouble(SpikeLowCut.Value), Convert.ToDouble(SpikeHighCut.Value), DATA_BUF_LEN
                BFParams parBF = new BFParams(2, 25000, 150.0, 2000.0, 2500); // [ToDo] Eliminate data buffer length

                // [TODO] Get rid of thresholds here. Should be calculated in SALPA dynamically
                int[] thresholds = new int[60];
                for (int i = 0; i < 60; i++)
                {
                    thresholds[i] = 1000 * 3;
                }

                m_DAQConfigured = true;
            }

            m_inputStream.Start();
        }
    }

}
