using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEAClosedLoop
{

  using TRawData = UInt16;
  using TData = Double;
  using TStimIndex = System.Int16;

	public class CStimDetectShift
	{
		public const int ErrorState = -3303;
    private const TStimIndex FILTER_DEPTH = 3;
		private int shift;
		private int DoubleStimPeriod;
		private int sigma;
		private int AVD;// Average value of Derivative
		private const double MinDRatio = .5;// Min ration between AVG & Current Derivative  
		private Average Current;
		private Average Previews;
		private List<int> ExpectedStims;
		private int WayNum;
		private TRawData[] F;
		private List<TStimGroup> m_expectedStims;
		private int MissedStimsCount; // how many stims wasn't found at preview Data Pocket
    private TRawData[] m_prevDataPoints;

    //Way Nums:
    //1:Deault, Find all pegs by using ExpectedStims
    //2: Get all pegs by Abstract reserch

		public CStimDetectShift()
		{
			// TODO: Complete member initialization
      m_prevDataPoints = new TRawData[6];
		}
		public List<TStimIndex> GetStims(TRawData[] DataPacket, TStimGroup ExpectedStim)
		{
      List<TStimIndex> FoundPegs = new List<TStimIndex>();
			List<TStimIndex> ErrorList = new List<TStimIndex> { 0 };
      int ValidateCount = 0;

      // Process last FILTER_DEPTH points of the previous packet
      this.F = m_prevDataPoints;
      
      // [TODO] Миша, если тебе нужен этот switch, повтори его тут сам
      for (TStimIndex i = 0; i < FILTER_DEPTH; i++)
      {
        m_prevDataPoints[FILTER_DEPTH + i] = DataPacket[i];
        if (BasicValidateSingleStimInT(i))
        {
          ValidateCount++;
          FoundPegs.Add((TStimIndex)(i - FILTER_DEPTH));
        }
      }

      // Process current packet
      this.F = DataPacket;

      WayNum = 2;
      int DataPacketLength = DataPacket.Length - FILTER_DEPTH - 1;
			switch (WayNum)
			{
				case 1:
					//TODO find by expected stims;
					break;
				case 2:
					//TODO find all by hard research;
					//Opimization cycle
					//EndOpimization
          for (TStimIndex i = 0; i < DataPacketLength; i++)
					{
						if (BasicValidateSingleStimInT(i))
						{
							ValidateCount++;
							FoundPegs.Add(i);
						}
					}
					MissedStimsCount = ExpectedStim.count - ValidateCount;
					break;
			}

      for (int i = 0; i < FILTER_DEPTH; i++)
      {
        m_prevDataPoints[i] = DataPacket[DataPacketLength + i];
      }
      
			return FoundPegs;
			//If all is realy bad;
			//if(ValidateCount == 0) return null;
		}
		public List<TStimIndex> GetStims(TRawData[] DataPacket)
		{
			List<TStimIndex> FindedPegs = new List<TStimIndex>();
			List<TStimIndex> ErrorList = new List<TStimIndex> { 0 };
			this.F = DataPacket;
			WayNum = 2;
			int ValidateCount = 0;

			//TODO find all by hard research;
			//Opimization cycle
			int DataPacketLength = DataPacket.Length - 4;
			//EndOpimization

      for (Int16 i = 0; i < DataPacketLength; i++)
			{
				if (BasicValidateSingleStimInT(i))
				{
					ValidateCount++;
					FindedPegs.Add(i);
				}
			}
			//MissedStimsCount = ExpectedStim.count - ValidateCount;


			return FindedPegs;
			//If all is realy bad;
			//if(ValidateCount == 0) return null;
		}
		private bool BasicValidateSingleStimInT(long t)
		{
			switch (WayNum)
			{
				case 1:
					break;
				case 2:
          if (F[t + 1] - F[t] > 45 &&
							F[t + 2] - F[t + 1] > 45 &&
							F[t + 3] - F[t + 2] > 45)
					{
						return true;
					}
					else
					{
						return false;
					}
			}
			return false;
		}
	}
	class Average
	{
		private List<int> values;
		public double Value;
		public double TripleSigma;
		public void AddValueElem(int ValueToAdd)
		{
			values.Add(ValueToAdd);
		}
		public void Calc()
		{
			double summ = 0;
			for (int i = 0; i < values.Count; i++)
			{
				summ += values[i];
			}
			Value = summ / values.Count;
			double QuarteSumm = 0;
			for (int i = 0; i < values.Count; i++)
			{
				QuarteSumm += Math.Pow(Value - values[i], 2);
			}
			TripleSigma = 3.5 * Math.Sqrt(QuarteSumm / values.Count);
		}
		public Average()
		{
			values = new List<int>();
			Value = 0;
			TripleSigma = 0;
		}
	}
}
