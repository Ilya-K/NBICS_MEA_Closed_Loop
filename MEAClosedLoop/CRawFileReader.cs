using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading;
using UsbNetDll;

namespace MEAClosedLoop
{
  using TRawDataPacket = Dictionary<int, ushort[]>;

  public class CRawFileReader : IRawDataProvider
  {
    private const int CRLF = 2;                     // Length of CRLF in Windows = 2
    private const int CYCLE_QUEUE_SIZE = 10;
    private const int START_CHANNEL_LIST = 13;
    private const int DEFAULT_SAMPLE_RATE = Param.DAQ_FREQ;
    private const int DEFAULT_ZERO_LEVEL = 32768;
    private const int DEFAULT_BLOCK_SIZE = Param.DAQ_FREQ / 10;

    private readonly bool WITH_HEADER;
    private int N_CHANNELS_IN_FILE;       // if (!WITH_HEADER) N_CHANNELS_IN_FILE = min(|m_channelsToRead|, MAX_CHANNELS)
    private int N_CHANNELS_TO_READ;

    private string m_fileName;
    private long m_startOfData;
    private bool m_paused = false;
    private AutoResetEvent waitEOF;

    // Callback functions
    private OnChannelData m_onChannelData;
    private OnError m_onError;

    public int m_cbHandle = 0;
    private BinaryReader m_binReader;
    private ConcurrentQueue<TRawDataPacket> m_readyQueue;
    private ConcurrentQueue<TRawDataPacket> m_freePacketPool;

    private int[] m_channelsToRead;
    private int[] m_channelsInFile;
    private int m_blockSize;
    private int m_sampleRate;
    public int SampleRate { get { return m_sampleRate; } }
    private int m_zeroLevel;
    public int ZeroLevel { get { return m_zeroLevel; } }
    private System.Timers.Timer m_readoutTimer = new System.Timers.Timer();

    // [DEBUG]
    // private long[] log;

    // Public methods ===================================================================================================
    // Constructor ======================================================================================================
    public CRawFileReader(string fileName, OnChannelData onChannelData, OnError onError)
    {
      m_fileName = fileName;
      m_onChannelData = onChannelData;
      m_onError = onError;
      m_blockSize = DEFAULT_BLOCK_SIZE;

      // Try to read header from file
      WITH_HEADER = ParseHeader(m_fileName, ref m_sampleRate, ref m_zeroLevel, ref m_startOfData, ref m_channelsInFile);

      if (!WITH_HEADER)
      {
        // Set default values for parameters
        m_sampleRate = DEFAULT_SAMPLE_RATE;
        m_zeroLevel = DEFAULT_ZERO_LEVEL;
        m_startOfData = 0;
        m_channelsInFile = new int[MEA.MAX_CHANNELS];
        for (int i = 0; i < MEA.MAX_CHANNELS; ++i) m_channelsInFile[i] = i;
      }

      N_CHANNELS_IN_FILE = m_channelsInFile.Sum(x => (x >= 0) ? 1 : 0);
      N_CHANNELS_TO_READ = N_CHANNELS_IN_FILE;
      m_channelsToRead = m_channelsInFile;

      m_readoutTimer.Elapsed += TimerProc;
      UpdateTimer(m_blockSize, m_sampleRate);

      m_readyQueue = new ConcurrentQueue<TRawDataPacket>();
      m_freePacketPool = new ConcurrentQueue<TRawDataPacket>();
      FillPool(ref m_freePacketPool, m_channelsToRead, m_blockSize);

      waitEOF = new AutoResetEvent(false);
      // [DEBUG]
      //log = new long[10000];

    }

    // Set Sample Rate ==================================================================================================
    public uint SetSampleRate(int rate)
    {
      m_sampleRate = rate;
      UpdateTimer(m_blockSize, m_sampleRate);
      return 0;
    }

    // Select Channels to Readout =======================================================================================
    // selectedChannels : set of channels to readout
    // queuesize        : seems to be max length of readout buffer per channel. Should be ~10 times more than "threshold"
    // threshold        : desired size of packet. Packet size will be >= threshold
    // samplesize       : 16
    public void SetSelectedChannelsQueue(bool[] selectedChannels, int queuesize, int threshold, CMcsUsbDacqNet.SampleSize samplesize)
    {
      m_blockSize = threshold;
      UpdateTimer(m_blockSize, m_sampleRate);

      Array.Resize(ref selectedChannels, MEA.MAX_CHANNELS);

      m_channelsToRead = Array.ConvertAll(m_channelsInFile, x => selectedChannels[x] ? x : -1);

      N_CHANNELS_TO_READ = m_channelsToRead.Sum(x => (x >= 0) ? 1 : 0);
      if (!WITH_HEADER) N_CHANNELS_IN_FILE = N_CHANNELS_TO_READ;

      FillPool(ref m_freePacketPool, m_channelsToRead, m_blockSize);
    }

    // Start Acquisition ================================================================================================
    public void StartDacq()
    {
      if (N_CHANNELS_TO_READ == 0)
      {
        m_onError("No one channel selected!", 0);
        return;
      }
      if ((m_binReader != null) || m_readoutTimer.Enabled)
      {
        m_onError("Device has been already started!", 0);
        return;
      }
      m_binReader = new BinaryReader(File.Open(m_fileName, FileMode.Open, FileAccess.Read));
      m_binReader.BaseStream.Position = m_startOfData;
      m_cbHandle = 0;
      if (!m_paused) m_readoutTimer.Start();
    }

    // Read Data ========================================================================================================
    // This function is called from separate thread so we should take care of data sychronization
    // That's why we use ConcurrentQueue here
    public Dictionary<int, ushort[]> ChannelBlock_ReadFramesDictUI16(int cbHandle, int numSamples, out int retSize)
    {
      TRawDataPacket data = null;
      retSize = 0;
      if (m_readyQueue.TryDequeue(out data))
      {
        m_freePacketPool.Enqueue(data);
        retSize = m_blockSize * N_CHANNELS_IN_FILE * sizeof(ushort);
      }
      return data;
    }

    // Stop Acquisition =================================================================================================
    public void StopDacq()
    {
      m_readoutTimer.Stop();
      m_paused = false;
      if (m_binReader != null) 
        lock (m_binReader)
        {
          m_binReader.Close();
          m_binReader = null;
        }
    }
    
    // [DEBUG]
    // Pause Acquisition ================================================================================================
    public void PauseDacq()
    {
      m_readoutTimer.Stop();
      m_paused = true;
    }
    // [DEBUG]
    // Resume Acquisition ===============================================================================================
    public void ResumeDacq()
    {
      if (m_paused == true)
      {
        m_paused = false;
        if (m_binReader != null) m_readoutTimer.Start();
      }
    }
    // [DEBUG]
    // Give one packet ==================================================================================================
    public void Next()
    {
      if (m_paused == true)
      {
        object o1 = null;
        EventArgs e1 = null;
        TimerProc(o1, e1);
      }
    }
    // [DEBUG]
    // Wait till End of File ============================================================================================
    public void WaitEOF()
    {
      waitEOF.WaitOne();
    }

    // Private functions ================================================================================================
    // TimerProc runs in separate thread so we should take care about data sychronization
    // ConcurrentQueue is used to avoid sychronization problem
    private void TimerProc(object o1, EventArgs e1)
    {
      // [DEBUG]
      //System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
      //sWatch.Start();

      TRawDataPacket data;
      m_freePacketPool.TryDequeue(out data);

      try
      {
        int count = 0;
        lock (m_binReader)
        {
          for (int j = 0; j < m_blockSize; ++j)
          {
            for (int k = 0; k < N_CHANNELS_IN_FILE; ++k)
            {
              ushort tmpData = m_binReader.ReadUInt16();
              int channel = m_channelsToRead[k];
              if (channel >= 0)
              {
                count++;
                data[channel][j] = tmpData;
              }
            }
          }
        }
        m_readyQueue.Enqueue(data);
        m_onChannelData(m_cbHandle++, m_blockSize);
      }
      catch (EndOfStreamException e)
      {
        StopDacq();
        waitEOF.Set();
        // System.Windows.Forms.MessageBox.Show("EOF! " + e.Message);
      }

      // [DEBUG]
      //sWatch.Stop();
      //log[m_cbHandle - 1] = sWatch.ElapsedMilliseconds;
    }

    private void UpdateTimer(int blockSize, int sampleRate)
    {
      m_readoutTimer.Interval = 1000 * blockSize / sampleRate;
    }

    /* Sample header
    MC_DataTool binary conversion
    Version 2.6.8
    MC_REC file = "D:\MC_Rack_Data\data.mcd"
    Sample rate = 25000
    ADC zero = 32768
    El = 0.1136µV/AD
    Streams = El_21;El_31;El_41;El_51;El_61;El_71;El_12;El_22;El_32;El_42;El_52;El_62;El_72;El_82;El_13;El_23;El_33;El_43
    EOH
    */
    private bool ParseHeader(string fileName, ref int sampleRate, ref int zeroLevel, ref long startOfData, ref int[] elArray)
    {
      using (StreamReader strReader = new StreamReader(fileName))
      {
        char[] header = new char[11];
        strReader.Read(header, 0, 11);
        int position = 11;

        if (new string(header).CompareTo("MC_DataTool") == 0)
        {
          string s;
          s = strReader.ReadLine();           // Rest of the first line
          position += s.Length + CRLF;
          s = strReader.ReadLine();           // "Version" line
          position += s.Length + CRLF;
          s = strReader.ReadLine();           // "MC_REC file" line
          position += s.Length + CRLF;

          // Sample Rate
          s = strReader.ReadLine();
          position += s.Length + CRLF;
          int start = s.LastIndexOf('=') + 1;
          sampleRate = Int32.Parse(s.Substring(start));

          // ADC Zero Level
          s = strReader.ReadLine();
          position += s.Length + CRLF;
          start = s.LastIndexOf('=') + 1;
          zeroLevel = Int32.Parse(s.Substring(start));

          // Voltage (uV/bit) 
          s = strReader.ReadLine();
          position += s.Length + CRLF;

          // List of Channels
          s = strReader.ReadLine().Substring(START_CHANNEL_LIST);
          position += s.Length + START_CHANNEL_LIST + CRLF;
          string[] elNumbers = s.Split(new string[] { ";El_" }, MEA.MAX_CHANNELS, StringSplitOptions.RemoveEmptyEntries);
          elArray = Array.ConvertAll(elNumbers, el => MEA.EL_DECODE[Int32.Parse(el)]);

          // End Of Header
          if (strReader.ReadLine() == "EOH")
          {
            startOfData = position + 5;
          }
          else
          {
            throw new Exception("Wrong file format!");
          }
          return true;
        }
        else
        {
          return false;
        }
      }
    }

    private void FillPool(ref ConcurrentQueue<TRawDataPacket> m_pool, int[] channelsToRead, int blockSize)
    {
      TRawDataPacket dataPacket;
      // Clear queue
      while (m_pool.TryDequeue(out dataPacket)){};

      for (int i = 0; i < CYCLE_QUEUE_SIZE; ++i)
      {
        dataPacket = new TRawDataPacket(N_CHANNELS_TO_READ);
        for (int k = 0; k < N_CHANNELS_IN_FILE; ++k)
        {
          if (channelsToRead[k] >= 0) dataPacket[channelsToRead[k]] = new ushort[blockSize];
        }
        m_pool.Enqueue(dataPacket);
      }
    }
  }
}
