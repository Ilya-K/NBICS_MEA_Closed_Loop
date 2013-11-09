using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Neurorighter;
using Common;


namespace MEAClosedLoop
{
  using TData = System.Double;
  using TStimIndex = System.Int16;  //TODO: what's this?
  using TRawDataPacket = Dictionary<int, ushort[]>;
  using StimuliList = List<TStimGroup>;

  public class CDetectorTest
  {
    const int DEFAULT_VALUE = 32767;
    const int MIN_PACKET_SIZE = 150;
    const int BLOCK_SIZE = 2500;

    private CInputStream m_inputStream;
    private TRawDataPacket m_prevPacket;
    private CStimDetector m_stimDetector;
    private int m_artifChannel;
    private StimuliList m_expectedStims;
    private OnStreamKillDelegate m_onStreamKill = null;
    public OnStreamKillDelegate OnStreamKill { set { m_onStreamKill = value; } }

    private Int64 m_squareError;

    private volatile bool m_kill;


    public CDetectorTest(string fileName, StimuliList sl)
    {
      List<int> channelList = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59 });

      m_inputStream = new CInputStream(fileName, channelList, BLOCK_SIZE);

      m_inputStream.OnStreamKill = Dismiss;
      m_inputStream.ConsumerList.Add(ReceiveData);


      //m_stimDetector = new CStimDetector(15, 35, 150); //old

      m_artifChannel = m_inputStream.ChannelList[0];
      m_expectedStims = sl;
      m_kill = false;
    }

    public void ReceiveData(TRawDataPacket currPacket)
    {


      // Prepare "previous" packet for the first packet processing
      if (m_prevPacket == null)
      {
        m_prevPacket = new TRawDataPacket(currPacket.Count);
        foreach (int channel in currPacket.Keys) m_prevPacket[channel] = new ushort[MIN_PACKET_SIZE];
        m_prevPacket.Keys.AsParallel().ForAll(channel => Helpers.PopulateArray<ushort>(m_prevPacket[channel], currPacket[channel][0]));
      }
      // [TODO] Check here if we need to call Stim Detector now
      // if(IsStimulusExpected(timestamp, m_expectedStims) {
      
      //List<TStimIndex> stimIndices = m_stimDetector.Detect(currPacket[m_artifChannel], m_expectedStims); //old version
      CStimDetectShift m_stimDetector = new CStimDetector();
      List<TStimIndex> stimIndices = m_CStimDetector.GetShift();
      

      // Calculate error
      // m_squareError += error;

      m_prevPacket = currPacket;
    }

    public UInt64 RunTest()
    {
      m_inputStream.Start();//TODO: what's this?

      //TODO: compare m_expectedStims and results?

      //return squareError;
      return 0;
    }
    private void Dismiss()
    {
      m_kill = true;
      if (m_onStreamKill != null) m_onStreamKill();
    }
  }
}
