using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEAClosedLoop
{
    using TRawData = UInt16;
    using TData = Double;
    using TStimIndex = System.Int16;
    class CStimDetectShift
    {
        public const int ErrorState = -3303;
        private TRawData[] F;
        private int shift;
        private int StartIndex;
        private int StimCount;
        private int DoubleStimPeriod;
        private int sigma;
        private int AVD;// Average value of Derivative
        private const double MinDRatio = .5;// Min ration between AVG & Current Derivative  
        private int dF(long t)
        {
            return F[t + 1] - F[t];
        }
        public CStimDetectShift(TRawData[] Data, int _StimCount, int _StartIndex, int _DoubleStimPeriod, int _sigma = 80)
        {
            F = Data;
            StimCount = _StimCount;
            StartIndex = _StartIndex;
            DoubleStimPeriod = _DoubleStimPeriod;
            sigma = _sigma;
        }
        public int GetShift()
        {

            for (shift = - sigma; shift <= sigma; shift++)
            {
                int ValidateCount = 0;
                AVD = dF(StartIndex + shift);
                for (int j = 1; j <= StimCount; j++ )
                {
                    if (ValidateStimInT(shift + StartIndex + j * DoubleStimPeriod)) ValidateCount++;
                }
                if(ValidateCount == StimCount) return shift;
            }
            return ErrorState; 
        }
        private bool ValidateStimInT(long t)
        {
            if(dF(t)*AVD < 0) return false;
            if (AVD / dF(t) < MinDRatio) return false;
            
            return true;
        }
    }
}
