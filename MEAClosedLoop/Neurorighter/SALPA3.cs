using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//implementation of Daniel Wagenaar's SALPA algorithm, based on his code from Meabench
//I have attempted to use variable names to reflect the vocabulary of the methods paper,
//but default to the meabench code when the paper is not specific, and to previous incarnations
//of the code in NeuroRighter for interface specifics.
//takes in one continuous raw stream (either raw or filtered data) and outputs two streams: 
//the 'corrected' stream with artifacts removed, as well as a stream detailing the estimated artifact 


namespace Neurorighter
{
    using TFltData = System.Double;
    using TStimIndex = System.Int16;
    using TFltDataPacket = Dictionary<int, System.Double[]>;
    using TRawDataPacket = Dictionary<int, ushort[]>;

    public struct StimTick
    {
        public int index;
        public int numStimReads;
        public StimTick(int index, int numStimReads)
        {
            this.index = index;
            this.numStimReads = numStimReads;
        }
    }
    class SALPA3
    {
        private TFltData railHigh;
        private TFltData railLow;
        private int PRE;
        private int POST;

        private int length_sams;// = 75;
        private int asym_sams;// = 10;
        private int blank_sams;// = 20;
        private int ahead_sams;// = 5;
        private int period_sams;// = 0;
        private int delay_sams;// = 0;
        private int forcepeg_sams;// = 0;
        private int current_time;
        
        private TFltData[] thresh;

        private Dictionary<int, LocalFit> fitters;
        //note that this only needs to filter the channels on this particular device
        public SALPA3(int length_sams, int asym_sams, int blank_sams, int ahead_sams, int forcepeg_sams, TFltData railLow, TFltData railHigh, int[] channels, TFltData[] thresh)
        {
            //MB defaults:
            this.length_sams = length_sams;   // 75;
            this.asym_sams = asym_sams;// 10;
            this.blank_sams = blank_sams;// 75;//HACK try 20
            this.ahead_sams = ahead_sams;// 5;
            //this.period_sams = period_sams;// 0;
            //this.delay_sams = 0;
            this.forcepeg_sams = forcepeg_sams;//10;

            length_sams = 35;
            asym_sams = 10;
            blank_sams = 35;
            ahead_sams = 5;
            forcepeg_sams = 10;

            this.thresh = thresh;
            this.railHigh = railHigh;
            this.railLow = railLow;
            this.current_time = 0;


            this.PRE = 2 * length_sams;
            this.POST = 2 * length_sams + 1 + ahead_sams;
            int numChannels = channels.Length;
            fitters = new Dictionary<int, LocalFit>(numChannels);
            for (int i = 0; i < numChannels; i++)
            {
                fitters[channels[i]] = new LocalFit(thresh[i], length_sams, blank_sams, ahead_sams, asym_sams, railHigh, railLow, forcepeg_sams);
            }
        }

        public void filter(Dictionary<int, TFltData[]> srcData, List<int> stimIndicesIn)
        {
            List<TStimIndex> stimIndices = new List<TStimIndex>();
            int length = srcData.First().Value.Length;
            //grab the stim indices needed for this particular buffer load
            lock (stimIndicesIn)
            { 
                //convert the stimindices input into something easier to search- indices are in relationship to the current buffload,
                //use all indices from the last two buffers (current buffer included).  Note that this is a deviation from previous NR
                //SALPA implimentations
                stimIndices = new List<TStimIndex>(stimIndicesIn.Count);
                for (int i = 0; i < stimIndicesIn.Count; ++i)
                {
                    if ((stimIndicesIn[i] + 300 > current_time) && (stimIndicesIn[i] < current_time + length))
                    {
                      stimIndices.Add((TStimIndex)(stimIndicesIn[i] - current_time));
                    }
                }
            }

            // [DONE] Add multithreading here
            /*
            TFltDataPacket filteredData = new TFltDataPacket(srcData.Count);
            // Fill keys to enable parallel foreach on the next step
            foreach (int channel in srcData.Keys)
            {
                filteredData[channel] = null;
            }
            */
            fitters.Keys.AsParallel().ForAll(channel =>
            {
              // filteredData[channel] = Array.ConvertAll(srcData[channel], x => (TFltData)x);
              srcData[channel] = fitters[channel].filter(srcData[channel], stimIndices);
            });
            current_time += length;
        }

        public int offset()
        {
            return POST;
        }
   
    }
   
}
