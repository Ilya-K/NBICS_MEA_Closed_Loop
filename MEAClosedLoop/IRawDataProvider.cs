using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UsbNetDll;

namespace MEAClosedLoop
{
  public interface IRawDataProvider
  {
    uint SetSampleRate(int rate);
    
    // selectedChannels : set of channels to readout
    // queuesize        : seems to be max length of readout buffer per channel. Should be ~10 times more than "threshold"
    // threshold        : desired size of packet. Packet size will be >= threshold
    // samplesize       : 16
    void SetSelectedChannelsQueue(bool[] selectedChannels, int queuesize, int threshold, CMcsUsbDacqNet.SampleSize samplesize);
    
    void StartDacq();
    
    Dictionary<int, ushort[]> ChannelBlock_ReadFramesDictUI16(int cbHandle, int numSamples, out int retSize);
    
    void StopDacq();
  }
}
