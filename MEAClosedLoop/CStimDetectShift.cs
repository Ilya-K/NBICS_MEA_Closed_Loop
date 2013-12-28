using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using MEAClosedLoop;
namespace MEAClosedLoop
{
  #region Definitions
  using TData = System.Double;
  using TTime = System.UInt64;
  using TStimIndex = System.Int16;
  using TAbsStimIndex = System.UInt64;
  using TRawData = UInt16;
  using TRawDataPacket = Dictionary<int, ushort[]>;
  #endregion
  public class CStimDetectShift
  {
    #region Стандартные значения класса
    public const int ErrorState = -3303;
    private const TStimIndex FILTER_DEPTH = 22;
    private const TStimIndex default_offset = 8;
    private const TStimIndex start_offset = 16;
    private const TRawData Defaul_Zero_Point = 32768;
    private List<TStimGroup> m_expectedStims;
    public int m_Artif_Channel = 22;
    #endregion

    #region Внутрение данные
    private cases CurrentCase;
    private cases NextCase;
    private int CallCount;
    private object LockStimList = new object();
    private TRawDataPacket PrevPacket;
    private ushort MaximumShiftRange = 250;
    private TTime CurrentTime;
    private bool IsInNextBuffZone;
    private bool IsInCurrentZone;
    private bool IsNullReturned;
    //TestGraph GrafForm = new TestGraph();
    #endregion
    #region Конструктор
    public CStimDetectShift()
    {
      CurrentCase = new cases();
      NextCase = new cases();
      m_expectedStims = new List<TStimGroup>();
      //Отстуствие артефактов в начале
      IsInCurrentZone = false;
      IsInNextBuffZone = false;
     

    }
    #endregion
    #region Добавление ожидаемых стимулов в список
    public void SetExpectedStims(TStimGroup StimGroupToAdd)
    {
      lock (LockStimList)
      {
        for (int i = 0; i < StimGroupToAdd.count; i++)
        {
          TStimGroup SingleStim = new TStimGroup();
          SingleStim.count = 1;
          SingleStim.stimTime = StimGroupToAdd.stimTime + (ulong)i * StimGroupToAdd.period;
          m_expectedStims.Add(SingleStim);
        }
      }
    }
    #endregion
    #region Проверка необходимости пакета, заканчивающегося временем TestingTime, анализ вхождения артефакта в этот пакет
    public bool IsDataRequired(TTime TestingTime)
    {
      
      bool Flag = false;
      //- если было в следующем пакете
      lock (LockStimList)
      {
        foreach (TStimGroup stim in m_expectedStims)
        {
          #region CASE_0 - артефакт вошел в текущую буфферную зону и при предыдущем запросе был возвращен null
          if (IsNullReturned)
          {
            Flag = true;
            IsInCurrentZone = false;
            IsInNextBuffZone = false;
            break;
          }
          #endregion
          #region CASE_1 - артефакт входит во внутрь пакета, исключая буфферные зоны
          if (stim.stimTime <= TestingTime + (ulong)FILTER_DEPTH)
          {
            IsInCurrentZone = true;
            Flag = true;
            //break;
          }
          else
          {
            IsInCurrentZone = false;
          }
          #endregion
          #region CASE_2 - артефакт находится в первой буфферной зоне следующего и замыкающей предыдущего пакета.
          if (stim.stimTime <= TestingTime + (ulong)FILTER_DEPTH && stim.stimTime >= TestingTime - (ulong)FILTER_DEPTH)
          {
            IsInNextBuffZone = true;
            Flag = false;
          }
          else
          {
            IsInNextBuffZone = false;
          }
          #endregion
        }

      }
      if (Flag) CurrentTime = TestingTime;
      return Flag;
    }
    #endregion
    #region Выдача найденных в пакете артефактов
    public List<TStimIndex> GetStims(TRawDataPacket DataPacket)
    {
      CallCount++;
      #region Определение ситуации с расположением артефактов в пакете
      //Случай, когда мы просим задержать пакет.
      if (IsNullReturned)
      {
        #region Подготовка склеенного массива
        TRawDataPacket _DataPacket = new TRawDataPacket(56);
        for (int channel_index = 0; channel_index < 56; channel_index++)
        {
          _DataPacket[channel_index] = new TRawData[PrevPacket[channel_index].Length + DataPacket[channel_index].Length];
          //Копируем старый в новый
          for (int i = 0; i < PrevPacket[channel_index].Length; i++)
          {
            _DataPacket[channel_index][i] = PrevPacket[channel_index][i];
          }
          //копируем новый в новый
          for (int i = 0; i < DataPacket[channel_index].Length; i++)
          {
            _DataPacket[channel_index + PrevPacket[channel_index].Length][i] = PrevPacket[channel_index][i];
          }
        }
        #endregion
        IsNullReturned = false;
        CurrentTime = CurrentTime - (ulong)_DataPacket[m_Artif_Channel].Length;
        return FindStims(_DataPacket[m_Artif_Channel]);
      }
      //Случай наличия артефакта в следующей плохой зоне.
      if (IsInNextBuffZone)
      {
        PrevPacket = DataPacket;
        IsInNextBuffZone = false;
        IsNullReturned = true;
        return null;
      }
      //Случай наличия артефакта в нормальной текущей зоне.
      if (IsInCurrentZone)
      {
        IsInCurrentZone = false;
        IsNullReturned = false;
        CurrentTime = CurrentTime - (ulong)DataPacket[m_Artif_Channel].Length;
        return FindStims(DataPacket[m_Artif_Channel]);
      }
      #endregion
      return new List<TStimIndex>();
    }
    #endregion
    #region Непосредственно поиск артефактов основываясь на ExpectedStims
    public List<TStimIndex> FindStims(TRawData[] DataPacket)
    {
      List<TStimIndex> FindedPegs = new List<TStimIndex>();

      for (short i = (short)FILTER_DEPTH; i < DataPacket.Length - FILTER_DEPTH; i++)
      {
        if (i == 1433)
        {
        }
        if (TrueValidateSingleStimInT(DataPacket, i))
        {
          FindedPegs.Add(i);
          lock (LockStimList)
          {
            foreach (TStimIndex stim in FindedPegs)
            {
              for (int exp_stim_num = 0; exp_stim_num < m_expectedStims.Count; exp_stim_num++)
              {
                if (m_expectedStims[exp_stim_num].stimTime > CurrentTime + (ulong)i - 700 
                  && m_expectedStims[exp_stim_num].stimTime < CurrentTime + (ulong)i + 700)
                {
                  m_expectedStims.Remove(m_expectedStims[exp_stim_num]);
                  break;
                }
              }
            }
          }
          break;
        }
      }

      return FindedPegs;
    }
    #endregion
    #region Упрощенная верификация артефакта в момент времени t
    private bool BasicValidateSingleStimInT(long t)
    {
      return false;
    }
    #endregion
    #region Верификация артефакта в момент времени t
    private bool TrueValidateSingleStimInT(TRawData[] DataPacket, long t)
    {
      try
      {
        Average pre_average = new Average();
        Average post_average = new Average();

        for (int i = 0; i < FILTER_DEPTH; i++)
        {
          pre_average.AddValueElem(DataPacket[t - i]);
        }
        for (int i = 5; i < FILTER_DEPTH; i++)
        {
          post_average.AddValueElem(DataPacket[t + i]);
        }
        pre_average.Calc();
        post_average.Calc();
        #region Условия Подтверждения артефакта
        if (pre_average.IsInArea(DataPacket[t])
          && !pre_average.IsInArea(DataPacket[t + 1])
          && !pre_average.IsInArea(post_average.Value - post_average.Sigma)
          && !pre_average.IsInArea(post_average.Value + post_average.Sigma)
          //Clipping Fix
          && pre_average.Value > 10
          && pre_average.Value < 65000
          //StableLinesFix
          && Math.Abs(pre_average.Value - post_average.Value) > 240
          //Blanking Fix
          && pre_average.Sigma < 35
          )
        #endregion
        {
          //MessageBox.Show("ArtifFounded");
          return true;
        }
        return false;
      }
      catch (Exception e)
      {
        return false;
      }

    }
    #endregion
  }
  #region Среднее и сигма
  class Average
  {
    private List<int> values;
    public double Value;
    public double TripleSigma;
    public double Sigma;
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
      Sigma = Math.Sqrt(QuarteSumm / values.Count);
      TripleSigma = 3.5 * Sigma;
    }
    public bool IsInArea(double Value)
    {
      this.Calc();
      if (Math.Abs(this.Value - Value) < TripleSigma) return true;
      return false;
    }
    public Average()
    {
      values = new List<int>();
      Value = 0;
      TripleSigma = 0;
    }
  }
  #endregion
  #region Описание различых вхождений артефакта в текущий пакет
  public class cases
  {
    public bool InFilterFreeZone;
    public bool InFilterZone;
    public cases()
    {
      this.InFilterFreeZone = false;
      this.InFilterZone = false;
    }
  }
  #endregion
}
