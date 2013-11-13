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

    //Way Nums:
    //1:Deault, Find all pegs by using ExpectedStims
    //2: Get all pegs by Abstract reserch


    public CStimDetectShift()
    {
      // TODO: Complete member initialization

    }
    public List<TStimIndex> GetStims(TRawData[] DataPocket, List<TStimGroup> ExpectedStims, int SearchType = 2)
    {
      List<TStimIndex> FindedPegs = new List<TStimIndex>();
      List<TStimIndex> ErrorList = new List<TStimIndex> { 0 };
      this.F = DataPocket;
      int ValidateCount = 0;
      switch (WayNum)
      {
        case 1:
          //TODO find by expected stims;
          break;
        case 2:
          //TODO find all by hard research;
          for (Int16 i = 0; i < DataPocket.Count(); i++)
          {
            if (BasicValidateSingleStimInT(i))
            {
              ValidateCount++;
              FindedPegs.Add(i);
            }
          }
          MissedStimsCount = ExpectedStims[0].count - ValidateCount;
          break;
      }
      //If all is realy bad;
      return ErrorList;
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
