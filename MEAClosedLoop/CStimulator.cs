using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UsbNetDll;

namespace MEAClosedLoop
{
  //using StimuliList = List<UInt64>;
  using StimuliList = List<TStimGroup>;

  public class CStimulator
  {
    private const int DEFAULT_AMPLITUDE = 300;        // Amplitude of the default signal (mv)
    private CStg200xDownloadNet m_device;
    //private OnStg200xPollStatus m_pollHandler;
    private StimuliList m_stimuliList = new StimuliList();
    private bool m_configured = false;

    public CStimulator(CMcsUsbListNet usblist, uint idx, OnStg200xPollStatus pollHandler = null, string fileName = null)
    {
      //m_pollHandler = pollHandler;
      m_device = (pollHandler != null) ? new CStg200xDownloadNet(pollHandler) : new CStg200xDownloadNet();
      uint errStatus = m_device.Connect(usblist.GetUsbListEntry(idx));
      if (errStatus > 0) throw new Exception("Device connection error. Code returned: " + errStatus.ToString());

      if (fileName != null) DownloadStimulusShape(fileName, 1);
    }

    // It's dummy function now
    public TStimGroup GetStimulus()
    {
      // [TODO] Make real stimulus from config file
      TStimGroup stimulus;
      stimulus.stimTime = 0;
      stimulus.count = 1;
      stimulus.period = 0;
      return stimulus;
    }

    public void DownloadStimulusShape(string fileName, int channel)
    {
      CStimLoader config;
      //throw new NotImplementedException("Reading configuration from file is not implemented yet!\nCall Mikhail Tatarintsev +7-915-215-0924 for information ;) .");

      SetupMemory(1);             // Give all the memory to one segment
      // Only meaningfull for STG400x
      m_device.SetVoltageMode();
      SetupTrigger1(channel, 0, 0);
      int DACResolution = m_device.GetDACResolution();
      // Set highest bit to 1. Add "minus" to a positive nubmer to represent negative number
      ushort minus = (ushort)(1 << (DACResolution - 1));
      //load config
      if (fileName.Equals(""))
      {
        config = new CStimLoader();
      }
      else
      {
        config = new CStimLoader(fileName);
      }
      //prepeare STG
      m_device.ClearChannelData((uint)channel - 1);
      m_device.ClearSyncData((uint)channel - 1);
      //Send Data
      m_device.SendChannelData((uint)channel - 1, config.pdataPatterns[0], config.tdataPatterns[0]);
      m_device.SendSyncData((uint)channel - 1, config.psyncPatterns[0], config.tsyncPatterns[0]);

      m_configured = true;
    }

    public void DownloadDefaultShape(int channel, int sync, uint count, uint period)
    {
      // We have only 8 channels
      if ((channel < 1) || (channel > 8)) throw new ArgumentOutOfRangeException("(channel < 1) || (channel > 8)");
      if ((sync < 1) || (sync > 8)) throw new ArgumentOutOfRangeException("(sync < 1) || (sync > 8)");

      // We don't support period less than 1.2 ms
      if (period < 1200) throw new ArgumentOutOfRangeException("period < 1200");

      SetupMemory(1);             // Give all the memory to one segment

      SetupTrigger1(channel, sync, count);

      // Only meaningfull for STG400x
      m_device.SetVoltageMode();

      int DACResolution = m_device.GetDACResolution();

      // Set highest bit to 1. Add "minus" to a positive nubmer to represent negative number
      ushort minus = (ushort)(1 << (DACResolution - 1));

      int vRange, iRange;
      m_device.GetAnalogRanges(0, out vRange, out iRange);

      // Data for Channel ...
      {
        m_device.ClearChannelData((uint)(channel - 1));

        ushort u300mv = (ushort)((DEFAULT_AMPLITUDE * ((1 << (DACResolution - 1)) - 1)) / vRange);
        ushort u70mv = (ushort)((70 * ((1 << (DACResolution - 1)) - 1)) / vRange);

        ushort[] pData = new ushort[4];
        UInt64[] tData = new UInt64[4];

        pData[0] = u300mv;
        tData[0] = 400;       // us
        pData[1] = (ushort)(minus + u300mv);
        tData[1] = 300;       // us
        pData[2] = u70mv;
        tData[2] = 100;       // us
        pData[3] = 0;
        tData[3] = 200;       // us

        m_device.SendChannelData((uint)(channel - 1), pData, tData);
      }

      // Data for Sync ...
      {
        m_device.ClearSyncData((uint)(sync - 1));

        ushort[] pData = new ushort[2];
        UInt64[] tData = new UInt64[2];

        pData[0] = 1;
        tData[0] = 1200;                // us
        pData[1] = 0;
        tData[1] = period - 1200;       // us

        m_device.SendSyncData((uint)(sync - 1), pData, tData);
      }

      m_configured = true;
    }

    public void DownloadLadderShape(int channel, int sync, uint count, uint period)
    {
      // We have only 8 channels
      if ((channel < 1) || (channel > 8)) throw new ArgumentOutOfRangeException("(channel < 1) || (channel > 8)");
      if ((sync < 1) || (sync > 8)) throw new ArgumentOutOfRangeException("(sync < 1) || (sync > 8)");

      // We don't support period less than 1.2 ms
      if (period < 1200) throw new ArgumentOutOfRangeException("period < 1200");

      SetupMemory(1);             // Give all the memory to one segment

      SetupTrigger1(channel, sync, count);

      // Only meaningfull for STG400x
      m_device.SetVoltageMode();

      int DACResolution = m_device.GetDACResolution();

      // Set highest bit to 1. Add "minus" to a positive nubmer to represent negative number
      ushort minus = (ushort)(1 << (DACResolution - 1));

      int vRange, iRange;
      m_device.GetAnalogRanges(0, out vRange, out iRange);

      // Data for Channel ...
      {
        m_device.ClearChannelData((uint)(channel - 1));

        ushort u100mv = (ushort)((DEFAULT_AMPLITUDE / 3 * ((1 << (DACResolution - 1)) - 1)) / vRange);
        ushort u70mv = (ushort)((70 * ((1 << (DACResolution - 1)) - 1)) / vRange);

        ushort[] pData = new ushort[8];
        UInt64[] tData = new UInt64[8];

        for (int i = 0; i < 8; ++i)
        {
          pData[i] = (ushort)(u100mv * (i + 1));
          tData[i] = 1000;       // us
        }

        m_device.SendChannelData((uint)(channel - 1), pData, tData);
      }

      // Data for Sync ...
      {
        m_device.ClearSyncData((uint)(sync - 1));

        ushort[] pData = new ushort[2];
        UInt64[] tData = new UInt64[2];

        pData[0] = 1;
        tData[0] = 1200;                // us
        pData[1] = 0;
        tData[1] = period - 1200;       // us

        m_device.SendSyncData((uint)(sync - 1), pData, tData);
      }

      m_configured = true;
    }

    public void UpdateTime(ulong time)
    {

    }

    /// <summary>
    ///  Adds list of new stimuli to the end of list of stimuli regardless of their timestamp
    ///  Maybe it would be useful to implement kind of merge algorithm to keep stimuli list sorted
    ///  A Priority Queue might be used: http://www.codeproject.com/Articles/13295/A-Priority-Queue-in-C
    /// </summary>
    /// <param name="newStimuli">List of new stimuli</param>
    public void AddStimuli(StimuliList newStimuli)
    {
      lock (m_stimuliList) m_stimuliList.AddRange(newStimuli);
    }

    public void Start()
    {
      if (!m_configured) throw new InvalidOperationException("Waveform hasn't been loaded yet!");
      m_device.SendStart(1);  // Trigger 1
    }

    public void Stop()
    {
      m_device.SendStop(1);   // Trigger 1
    }

    #region These 2 functions are to be used before and after calibration
    // Call before calibration
    public void BackupCurrentShape()
    {

    }

    // Call after calibration
    public void RestoreShape()
    {

    }
    #endregion


    private void SetupTrigger1(int channel, int sync, uint count)
    {
      // Setup Trigger
      uint TriggerInputs = m_device.GetNumberOfTriggerInputs();
      uint[] channelmap = new uint[TriggerInputs];
      uint[] syncoutmap = new uint[TriggerInputs];
      uint[] repeat = new uint[TriggerInputs];
      for (int i = 0; i < TriggerInputs; i++)
      {
        channelmap[i] = 0;
        syncoutmap[i] = 0;
        repeat[i] = 0;
      }

      // Trigger 0
      channelmap[0] = (uint)(1 << (channel - 1));     // Channel
      syncoutmap[0] = (uint)(1 << (sync - 1));        // Syncout
      repeat[0] = count;                              // 0 = forever

      m_device.SetupTrigger(channelmap, syncoutmap, repeat);
    }

    private void SetupMemory(uint nSegs)
    {
      if ((nSegs < 1) || (nSegs > 100)) throw new ArgumentOutOfRangeException("nSegs");

      uint memory = m_device.GetTotalMemory();                // obtain total memory available

      uint[] segmentMemory = new uint[nSegs];                 // nSegs of segments
      for (int i = 0; i < nSegs; i++)                         // for each segment
      {
        segmentMemory[i] = memory / nSegs;                    // each segment has 1/nSegs share of the memory
      }
      m_device.SendSegmentDefine(segmentMemory);              // setup the STG

      uint nchannels = m_device.GetNumberOfAnalogChannels();
      uint nsync = m_device.GetNumberOfSyncoutChannels();
      uint[] channel_cap = new uint[nchannels];
      uint[] syncout_cap = new uint[nsync];
      for (int i = 0; i < nSegs; i++)                         // for each segment
      {
        m_device.SendSegmentSelect((uint)i, 0);               // switch to segment
        uint segment_mem = m_device.GetMemory();              // get memory available in this segment

        for (int j = 0; j < nchannels; j++)
        {
          channel_cap[j] = segment_mem / (nchannels + nsync); // devide memory amount to all channels
        }
        for (int j = 0; j < nsync; j++)
        {
          syncout_cap[j] = segment_mem / (nchannels + nsync); // and all sync outs.
        }

        m_device.SetCapacity(channel_cap, syncout_cap);       // define memory for current segment
      }

      m_device.SendSegmentSelect(0, 0);
    }

  }
}
