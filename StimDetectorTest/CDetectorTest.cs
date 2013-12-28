using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Neurorighter;
using MEAClosedLoop;


namespace StimDetectorTest
{
  using TData = System.Double;
  using TTime = System.UInt64;
  using TStimIndex = System.Int16;
  using TAbsStimIndex = System.UInt64;
  using TRawDataPacket = Dictionary<int, ushort[]>;
  using TFltDataPacket = Dictionary<int, System.Double[]>;

  public class CDetectorTest
  {
    const int DEFAULT_VALUE = 32767;
    const int MIN_PACKET_SIZE = 150;
    const int BLOCK_SIZE = 2500;

    private CInputStream m_inputStream;
    private List<TStimGroup> m_expectedStims;
    public List<TAbsStimIndex> m_stimIndices;
    private TStimGroup m_nextExpectedStim;
    private OnStreamKillDelegate m_onStreamKill = null;
    public OnStreamKillDelegate OnStreamKill { set { m_onStreamKill = value; } }

    // Count overhead in the number of the expected stimuli artifacts
    private int m_numberExceeded = 0;
    public int NumberExceeded { get { return m_numberExceeded; } }

    private Stopwatch sw1;
    private long m_timeElapsed = 0;
    public long TimeElapsed { get { lock (sw1) return m_timeElapsed; } }

    private readonly object lockRecordLen = new object();
    private TTime m_recordLength;
    public TTime RecordLength { get { lock (lockRecordLen) return m_recordLength; } }

    private Int64 m_squareError;
    private CStimDetector m_stimDetector;
    private CStimDetectShift m_stimDetectorShift;
    private int slowdownCount = 0;

    public CDetectorTest(string fileName, List<TStimGroup> sl)
    {
      List<int> channelList = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59 });

      m_inputStream = new CInputStream(fileName, channelList, BLOCK_SIZE);

      m_inputStream.OnStreamKill = Dismiss;
      m_inputStream.ConsumerList.Add(ReceiveData);

      sw1 = new Stopwatch();
      sw1.Reset();

      m_stimIndices = new List<TAbsStimIndex>();
      m_stimDetector = new CStimDetector(15) { ArtifactChannel = 2 }; // Any channel with data
      m_stimDetectorShift = new CStimDetectShift();

      if (sl != null)
      {
        m_expectedStims = sl;
        m_stimDetectorShift.SetExpectedStims(sl[0]);
        sl.RemoveAt(0);
      }
    }

    public void ReceiveData(TRawDataPacket currPacket)
    {
      m_inputStream.Next();

      // Show progress
      if (++slowdownCount % 50 == 0) Console.Write("\tProcessing {0}s\r", m_inputStream.TimeStamp / 25000);
      
      int currPacketLength = currPacket[currPacket.Keys.ElementAt(0)].Length;
      lock(lockRecordLen) m_recordLength = m_inputStream.TimeStamp + (TTime)currPacketLength;
      List<TStimIndex> stimIndices = null;

      sw1.Start();

      // Check here if we need to call the Stimulus Artifact Detector for the current packet
      //if (m_stimDetector.IsDataRequired(m_inputStream.TimeStamp + (TTime) currPacketLength))
      //{

      //  stimIndices = m_stimDetector.Detect(currPacket);
      //}
      if (m_stimDetectorShift.IsDataRequired(m_inputStream.TimeStamp + (TTime)currPacketLength))
      {
        stimIndices = m_stimDetectorShift.GetStims(currPacket);
      }
      sw1.Stop();

      if (stimIndices != null)
      {
        foreach (TStimIndex stimIdx in stimIndices)
        {
          m_stimIndices.Add(m_inputStream.TimeStamp + (TAbsStimIndex)stimIdx);
          //Console.WriteLine("\tstim add: " + m_stimIndices[m_stimIndices.Count() - 1].ToString());
        }
      }

      // Emulate addition of the new expected stimuli
      if (m_expectedStims[0].stimTime < m_inputStream.TimeStamp + (TTime)currPacketLength * 3)
      {
        m_stimDetectorShift.SetExpectedStims(m_expectedStims[0]);
        m_expectedStims.RemoveAt(0);
      }
    }
    
    public void ReceiveData_old(TRawDataPacket currPacket)  //TODO: make absolute stimIndices
    {
      m_inputStream.Next();
      TTime endOfPacket = m_inputStream.TimeStamp + (TTime)currPacket[currPacket.Keys.ElementAt(0)].Length;
      //m_stimDetector.GetStims(currPacket[m_artifChannel], m_nextExpectedStim);
      if (m_nextExpectedStim.stimTime < endOfPacket)
      {
        sw1.Start();

        /* List<TStimIndex> currentList = m_stimDetector.GetStims(currPacket[m_artifChannel], m_prevPacket[m_artifChannel], m_nextExpectedStim);
        foreach (TStimIndex stimIdx in currentList)
        {
          m_stimIndices.Add(m_inputStream.TimeStamp + (TAbsStimIndex)stimIdx);
          Console.WriteLine("stim add: " + m_stimIndices[m_stimIndices.Count() - 1].ToString());
        } */
        m_nextExpectedStim = m_expectedStims[0];
        m_expectedStims.RemoveAt(0);

        sw1.Stop();
      }
      /* // [ILYA_K] I believe it's not necessarily to run Stimulus Detector on the "empty" space
       * //may be needed on empty file test
      else
      {
        m_stimIndices.AddRange(m_stimDetector.GetStims(currPacket[m_artifChannel]));
      }
      */
      // Calculate error
      // m_squareError += error;
    }

    public List<TAbsStimIndex> RunTest()
    {

      m_inputStream.Pause();
      m_inputStream.Start();
      m_inputStream.Next();
      m_inputStream.WaitEOF();
      m_inputStream.Kill();
      m_inputStream = null;

      lock (sw1) m_timeElapsed = sw1.ElapsedMilliseconds;

      //comparing m_stimIndices with realStimIndices moved to Program.cs

      return m_stimIndices;
    }

    private void Dismiss()
    {
      if (m_onStreamKill != null) m_onStreamKill();
    }
  }
}
