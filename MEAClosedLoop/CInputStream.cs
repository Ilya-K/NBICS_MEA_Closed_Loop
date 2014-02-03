using System;
using System.Collections.Generic;
//using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using UsbNetDll;

namespace MEAClosedLoop
{
  using TRawDataPacket = Dictionary<int, ushort[]>;
  using TTime = System.UInt64;
 
  public class CMeaDeviceNet_ : CMeaDeviceNet, IRawDataProvider
  {
    public CMeaDeviceNet_(McsBusTypeEnumNet busType, OnChannelData onChannelData, OnError onError)
      : base(busType, onChannelData, onError) {}
  }

  // Класс входного потока.
  // Осуществляет чтение данных из входного устройства (USB MEA256 или файл)
  // и реализует очередь
  public class CInputStream
  {
    //const int NUM_BUFFERS = 3;
    const uint DEFAULT_SAMLPING_RATE = Param.DAQ_FREQ;
    private uint m_samplingRate;
    public IRawDataProvider m_dataSource;
//    private ConcurrentQueue<TRawDataPacket> m_rawQueue;
    private Queue<TRawDataPacket> m_rawQueue;
    private AutoResetEvent m_notEmpty;
    private List<int> m_channelList;
    public List<int> ChannelList { get { return new List<int>(m_channelList); } }
    private int m_blockSize;
    public int NChannels { get { return m_channelList.Count; } }
    public int BlockSize { get { return m_blockSize; } }
    private bool[] m_channelSet;

    private OnStreamKillDelegate m_onStreamKill = null;
    public OnStreamKillDelegate OnStreamKill { set { m_onStreamKill = value; } }
    private volatile bool m_kill = false;                   // Signal to stop acquisition and kill the thread
    private volatile bool m_stop = false;                   // Signal to stop acquisition
    private volatile bool m_stopped = true;                 // Flag = 1 when acquisition is stopped, = 0 otherwise

    delegate void OnChannelDataDelegate(Dictionary<int, ushort[]> data);
    
    private int m_lastBufLength;
    private object lockDeqTime = new Object();
    private TTime m_deqTimeStamp;                              // Contains sequental number of the first sample in the last dequeued buffer
    public TTime TimeStamp { get { lock (lockDeqTime) return m_deqTimeStamp; } }

    private object lockCurrentTime = new Object();
    private TTime m_currentTimeStamp;                       // Contains sequental number of the last read sample + 1

    private System.Diagnostics.Stopwatch m_windowsTime;

    //private delegate void OnChannelDataDelegate(TRawDataPacket data);
    
    public delegate void ConsumerDelegate(Dictionary<int, ushort[]> data);
    public List<ConsumerDelegate> ConsumerList = null;

    private Thread m_t;

    #region Constructors
    private CInputStream(List<int> channelList, int blockSize)
    {
      m_channelList = channelList;
      m_blockSize = blockSize;
      m_notEmpty = new AutoResetEvent(false);
      m_rawQueue = new Queue<TRawDataPacket>();
      m_samplingRate = DEFAULT_SAMLPING_RATE; // MC_Card does not support all settings, please see MC_Rack for valid settings
      m_lastBufLength = 0;
      m_deqTimeStamp = 0;
      m_currentTimeStamp = 0;
      m_windowsTime = new System.Diagnostics.Stopwatch();

      m_channelSet = new bool[MEA.MAX_CHANNELS];
      channelList.ForEach(channel => m_channelSet[channel] = true);

      ConsumerList = new List<ConsumerDelegate>();

      m_t = new Thread(new ThreadStart(RunStream));
      m_t.Start();
    }

    // Constructs InputStream for reading from file
    public CInputStream(string fileName, List<int> channelList, int blockSize)
      : this(channelList, blockSize)
    {
      // Length of channelList should be equal to the number of channels in the file, otherwise you'll receive wrong data
      m_dataSource = new CRawFileReader(fileName, OnChannelData, OnError);
      m_dataSource.SetSelectedChannelsQueue(m_channelSet, 10 * blockSize, blockSize, CMcsUsbDacqNet.SampleSize.Size16);
    }

    // Constructs InputStream for reading from USB-ME256 System
    public CInputStream(CMcsUsbListNet usbList, uint idx, List<int> channelList, int blockSize)
      : this(channelList, blockSize)
    {
      usbList.Initialize(DeviceEnumNet.MCS_MEA_DEVICE);
      uint dn = usbList.GetDeviceNumber();
      CMeaDeviceNet_ device = new CMeaDeviceNet_(usbList.GetBusType(idx), OnChannelData, OnError);
      device.Connect(usbList.GetUsbListEntry(idx));

      // [ToDo] Configure m_dataSource here
      device.SendStop(); // only to be sure

      int hwChannels;
      device.HWInfo().GetNumberOfHWADCChannels(out hwChannels);
      device.SetNumberOfChannels(hwChannels);

      device.SetSampleRate((int)m_samplingRate);

      int gain;
      device.GetGain(out gain);

      double[] voltageranges = device.HWInfo().GetSupportedVoltagesAsDoubles();
      /* for (int i = 0; i < voltageranges.Length; i++)
      {
        tbDeviceInfo.Text += "(" + i.ToString() + ") " + voltageranges[i] + "mV" + "\r\n";
      } */

      // Set the range acording to the index (only valid for MC_Card)
      device.SetVoltageRange(0);

      device.EnableDigitalIn(false);

      // Checksum not supported by MC_Card
      device.EnableChecksum(true);

      // Get the layout to know how the data look like that you receive
      int nChannels;
      //int ana, digi, che, tim
      //device.GetChannelLayout(out ana, out digi, out che, out tim, out block);
      // or
      device.GetChannelsInBlock(out nChannels);
      bool[] selChannels = new bool[nChannels];

      for (int i = 0; i < nChannels; i++)
      {
        selChannels[i] = false; // With false the channel i is deselected
      }
      int size = Math.Min(nChannels, m_channelSet.Length);
      for (int i = 0; i < size; i++)
      {
        selChannels[i] = m_channelSet[i];
      }

      device.SetSelectedChannelsQueue(selChannels, 10 * blockSize, blockSize, CMcsUsbDacqNet.SampleSize.Size16);

      m_dataSource = (IRawDataProvider)device;
    }
    #endregion

    public void Start()
    {
      if (m_t.IsAlive && m_stopped)
      {
        m_stop = false;
        m_stopped = false;
        lock (m_rawQueue) m_rawQueue.Clear();
        m_dataSource.StartDacq();
      }
    }

    public void Stop()
    {
      m_stop = true;
    }

    #region Debug
    // [DEBUG]
    // Pause Acquisition ================================================================================================
    public void Pause()
    {
      if (m_dataSource is CRawFileReader)
      {
        ((CRawFileReader)m_dataSource).PauseDacq();
      }
      else
      {
        m_dataSource.StopDacq();
      }
    }
    // [DEBUG]
    // Resume Acquisition ===============================================================================================
    public void Resume()
    {
      if (m_dataSource is CRawFileReader)
      {
        ((CRawFileReader)m_dataSource).ResumeDacq();
      }
      else
      {
        m_dataSource.StartDacq();
      }
    }
    // [DEBUG]
    // Give one packet ==================================================================================================
    public void Next()
    {
      if (m_dataSource is CRawFileReader)
      {
        ((CRawFileReader)m_dataSource).Next();
      }
    }
    // [DEBUG]
    // Wait till End of File ============================================================================================
    public void WaitEOF()
    {
      if (m_dataSource is CRawFileReader)
      {
        ((CRawFileReader)m_dataSource).WaitEOF();
      }
    }

    #endregion

    public UInt32 GetIntervalFromNowInMS(TTime futureTime)
    {
      UInt32 delta;
      TTime currTime;
      lock (lockCurrentTime) {
        currTime = m_currentTimeStamp + (TTime)m_windowsTime.ElapsedMilliseconds * (m_samplingRate / 1000);
      }
      if (futureTime < currTime) return 0;
      delta = (UInt32)(futureTime - currTime);

      return (1000 * delta) / m_samplingRate;
    }

    public void Kill()
    {
      Stop();
      m_kill = true;
      m_notEmpty.Set();
      if (m_onStreamKill != null) m_onStreamKill();
      if (m_t != null)
      {
        m_t.Join(2000);
        if (m_t.IsAlive)        // Something has gone wrong. Try to terminate thread
        {
          m_t.Abort();
          m_t.Join(10000);
        }
        m_t = null;
      }
    }

    // Runs in separate stream
    // Reads out queue and calls handlers for each data packet
    private void RunStream()
    {
      TRawDataPacket dataPacket = null;
      do
      {
        m_notEmpty.WaitOne();
        if (!m_stop)
        {
          lock (m_rawQueue)
          {
            if (m_rawQueue.Count > 0)
            {
              dataPacket = m_rawQueue.Dequeue();

              lock (lockDeqTime) m_deqTimeStamp += (ulong)m_lastBufLength;
              m_lastBufLength = dataPacket.First().Value.Length;

              if (m_rawQueue.Count > 0) m_notEmpty.Set();
            }
          }
          if (dataPacket != null)
            lock (ConsumerList)
              if ((ConsumerList != null) && (ConsumerList.Count != 0))
                foreach (ConsumerDelegate consumer in ConsumerList) consumer(dataPacket);
        }
        else
        {
          if (!m_stopped)
          {
            m_dataSource.StopDacq();
            m_stopped = true;
          }
        }
      } while (!m_kill);
    }

    // Функция, которая передаётся в источник данных в кач-ве callback-а
    private void OnChannelData(int CbHandle, int numSamples)
    {
      // [TODO] Check time consistency
      m_windowsTime.Restart();
      
      int size_ret;
      Dictionary<int, ushort[]> data = m_dataSource.ChannelBlock_ReadFramesDictUI16(CbHandle, numSamples, out size_ret);

      lock (lockCurrentTime) m_currentTimeStamp += (ulong)data.First().Value.Length;

      if (!m_stop)
      {
        lock (m_rawQueue) m_rawQueue.Enqueue(data);
      }
      m_notEmpty.Set();
    }

    private void OnError(String msg, int info)
    {
      Kill();
      System.Windows.Forms.MessageBox.Show("Error: " + msg);
    }

    // Obsolete
    /*
    public TRawDataPacket WaitData()
    {
      TRawDataPacket dataPacket = null;
      m_timestamp += (ulong)m_lastBufLength;
      do
      {
        m_notEmpty.WaitOne();
        lock (m_rawQueue)
        {
          if (m_rawQueue.Count > 0)
          {
            dataPacket = m_rawQueue.Dequeue();
            if (m_rawQueue.Count > 0) m_notEmpty.Set();
            m_lastBufLength = dataPacket.First().Value.Length;
          }
          else if (m_kill) break;
        }
      } while (dataPacket == null);

      return dataPacket;
    }
    */
  }
}
