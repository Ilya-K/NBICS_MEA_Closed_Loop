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
		private TRawData[] F;//like a function
		private int shift;
		private int DoubleStimPeriod;
		private int sigma;
		private int AVD;// Average value of Derivative
		private const double MinDRatio = .5;// Min ration between AVG & Current Derivative  
		private Average Current;
		private Average Previews;
		private List<int> ExpectedStims;
		private int WayNum;
    private TRawData[] p;
    private List<TStimGroup> m_expectedStims;
		private int dF(long t)
		{
			return F[t + 1] - F[t];
		}
	
		//Way Nums:
		//1:Deault, Find all pegs by using ExpectedStims
		//2: Get all pegs by Abstract reserch
		public CStimDetectShift(TRawData[] Data, List<int> _ExpectedStims, int _sigma = 80, int _WayNum = 1)
		{
			F = Data;
			sigma = _sigma;
			WayNum = _WayNum;
			ExpectedStims = _ExpectedStims;
			if (ExpectedStims.Count == 1)
			{
				WayNum = 1;
			}
		}

    public CStimDetectShift(TRawData[] p, List<TStimGroup> m_expectedStims)
    {
      // TODO: Complete member initialization
      this.p = p;
      this.m_expectedStims = m_expectedStims;
    }
		public List<int> GetShift()
		{
			List<int> FindedPegs = new List<int>();
			List<int> ErrorList = new List<int> {0};
			int ValidateCount = 0;
			switch (WayNum)
			{
				case 1:

					for (shift = -sigma; shift <= sigma; shift++)
					{
						for (int j = 1; j <= ExpectedStims.Count; j++)
						{
							if (ValidateSingleStimInT(shift + ExpectedStims[j])) ValidateCount++;
						}
						if (ValidateCount <= (ExpectedStims.Count * 2)) return FindedPegs;
					}
					break;
				case 2:

					break;
			}


			//If all is realy bad;
			return ErrorList;
		}
		private bool ValidateSingleStimInT(long t)
		{
			return true;
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
			TripleSigma = 3.5 * Math.Sqrt(QuarteSumm/values.Count);
		}
		public Average()
		{
			values = new List<int>();
			Value = 0;
			TripleSigma = 0;
		}
	}
}
