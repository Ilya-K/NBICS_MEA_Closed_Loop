﻿//#define GRAPH
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using MEAClosedLoop;
using System.Threading;
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
    private const TStimIndex FILTER_DEPTH = 16; 
    private const TStimIndex default_offset = 8;
    private const TStimIndex start_offset = 16;
    private const TStimIndex GUARANTEED_EMPTY_SPACE = 240;
    private const TStimIndex POST_SIGMA_CALC_DEPTH = 12;
    private const TAbsStimIndex BLANK_ARTIF_PRE_MAX_LENGTH = 40;
    public TAbsStimIndex MaximumShiftRange = 250;
    private TStimIndex MinimumLengthBetweenPegs = 10; // 250 - for standart hiFreq Stim
    private const TRawData Defaul_Zero_Point = 32768;
    public bool FullResearch = true; //True for unoptimized research
    public int m_Artif_Channel = MEA.EL_DECODE[23];
    #endregion

    #region Внутрение данные|
    private cases CurrentCase;
    private cases NextCase;
    private int CallCount;
    private object LockStimList = new object();
    public object LockExternalData = new object();
    private TRawDataPacket PrevPacket;
    private TTime CurrentTime;
    private bool IsInNextBuffZone;
    private bool IsInCurrentZone;
    private bool IsNullReturned;
    public CGraphRender DataRender;
    List<TStimIndex> FindedPegs;
    public List<TStimGroup> m_expectedStims;
    public List<TStimGroup> inner_expectedStims_to_display;
    public TRawData[] inner_data_to_display;
    public List<TStimIndex> inner_found_indexes_to_display;
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
      Thread RawDataRender = new Thread(DrawCallFunc);
      RawDataRender.Start();
      Thread.Sleep(2400);

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
          #region CASE_0 - артефакт вошел в первую буфферную зону рассматриваемого пакета (при предыдущем запросе был возвращен null)
          if (IsNullReturned)
          {
            Flag = true;
            IsInCurrentZone = false;
            IsInNextBuffZone = false;
            break;
          }
          #endregion
          #region CASE_1 - артефакт входит во внутрь пакета, исключая буфферные зоны
          if (stim.stimTime <= TestingTime - (ulong)FILTER_DEPTH - MaximumShiftRange)
          {
            IsInCurrentZone = true;
            Flag = true;
          }
          else
          {
            IsInCurrentZone = false || IsInCurrentZone;
          }
          #endregion
          #region CASE_2 - артефакт находится в первой буфферной зоне следующего или второй рассматриваемого пакета.
          if (stim.stimTime <= TestingTime + (ulong)FILTER_DEPTH + MaximumShiftRange
            && stim.stimTime >= TestingTime - (ulong)FILTER_DEPTH - MaximumShiftRange)
          {
            IsInNextBuffZone = true;
            Flag = true;
          }
          else
          {
            IsInNextBuffZone = false || IsInNextBuffZone;
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
      lock (LockExternalData)
      {
        //inner_data_to_display = DataPacket[m_Artif_Channel];
      }
      //Thread.Sleep(100);
      #region Определение ситуации с расположением артефактов в пакете
      //Случай, когда мы просим задержать пакет.
      if (IsNullReturned)
      {
        #region Подготовка склеенного массива
        TRawDataPacket _DataPacket = new TRawDataPacket();
        for (int channel_index = 0; channel_index < 56; channel_index++)
        {
          _DataPacket.Add(channel_index, new TRawData[PrevPacket[channel_index].Length + DataPacket[channel_index].Length]);
          //Копируем старый в новый
          for (int i = 0; i < PrevPacket[channel_index].Length; i++)
          {
            _DataPacket[channel_index][i] = PrevPacket[channel_index][i];
          }
          //копируем новый в новый
          for (int i = 0; i < DataPacket[channel_index].Length; i++)
          {
            _DataPacket[channel_index][i + PrevPacket[channel_index].Length] = DataPacket[channel_index][i];
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
      lock (LockExternalData)
      {
        inner_expectedStims_to_display = new List<TStimGroup>();
        lock (LockStimList)
        {
          for (int i = 0; i < m_expectedStims.Count(); i++)
          {
            TStimGroup gr = new TStimGroup();
            gr.stimTime = m_expectedStims[i].stimTime - CurrentTime;
            inner_expectedStims_to_display.Add(gr);
          }
        }
      }
      FindedPegs = new List<TStimIndex>();
      List<TStimGroup> stims_to_remove = new List<TStimGroup>();
      if (FullResearch)
      {
        #region Поиск по всему пакету
        for (short i = (short)FILTER_DEPTH; i < DataPacket.Length - FILTER_DEPTH; i++)
        {

          if (TrueValidateSingleStimInT(DataPacket, i))
          {
            FindedPegs.Add(i);
            lock (LockStimList)
            {
              foreach (TStimIndex stim in FindedPegs)
              {
                for (int exp_stim_num = 0; exp_stim_num < m_expectedStims.Count; exp_stim_num++)
                {
                  if (m_expectedStims[exp_stim_num].stimTime > CurrentTime + (ulong)i - MaximumShiftRange && m_expectedStims[exp_stim_num].stimTime < CurrentTime + (ulong)i + MaximumShiftRange)
                  {
                    stims_to_remove.Add(m_expectedStims[exp_stim_num]);
                    break;
                  }
                }
              }
            }
            i += GUARANTEED_EMPTY_SPACE;
            //break;
          }
        }
        #endregion
      }
      else
      {
        #region Поиск около ожидаемых артефактов
        lock (LockStimList)
        {
          foreach (TStimGroup stim in m_expectedStims)
          {
            TAbsStimIndex rightRange = (stim.stimTime - CurrentTime + ((TAbsStimIndex)MaximumShiftRange) + (TAbsStimIndex)FILTER_DEPTH > (TAbsStimIndex)DataPacket.Length) ?
                    (TAbsStimIndex)DataPacket.Length : (stim.stimTime - CurrentTime + ((TAbsStimIndex)MaximumShiftRange));
            TAbsStimIndex leftRange = (stim.stimTime - CurrentTime - (TAbsStimIndex)MaximumShiftRange - (TAbsStimIndex)FILTER_DEPTH < 0 && stim.stimTime - CurrentTime - (TAbsStimIndex)MaximumShiftRange - (TAbsStimIndex)FILTER_DEPTH < 20000) ?
                     0 : (stim.stimTime - CurrentTime - (TAbsStimIndex)MaximumShiftRange);
            if (FindedPegs.Count() > 0 && leftRange <= (TAbsStimIndex)FindedPegs[FindedPegs.Count() - 1] + 10)
              leftRange += (TAbsStimIndex)GUARANTEED_EMPTY_SPACE;
            for (TAbsStimIndex i = leftRange; i < rightRange; i++)
            {
              if (TrueValidateSingleStimInT(DataPacket, (TStimIndex)i))
              {
                bool IsBlankinkArtif = false;
                TAbsStimIndex SubRightRange = (i + BLANK_ARTIF_PRE_MAX_LENGTH + (TAbsStimIndex)FILTER_DEPTH < (TAbsStimIndex)DataPacket.Length) ? i + BLANK_ARTIF_PRE_MAX_LENGTH : (TAbsStimIndex)DataPacket.Length - (TAbsStimIndex)FILTER_DEPTH - 1;
                for (TAbsStimIndex j = i + 28; j < SubRightRange; j++)
                {
                  if (TrueValidateSingleStimInT(DataPacket, (TStimIndex)j))
                  {
                    IsBlankinkArtif = true;
                    break;
                  }
                }
                if (IsBlankinkArtif)
                {
                  i += 28;
                  continue;
                }
                else
                {
                  bool IsItPrev = false;
                  for (int j = 0; j < FindedPegs.Count(); j++)
                  {
                    if (FindedPegs[j] + MinimumLengthBetweenPegs > (TStimIndex)i) IsItPrev = true;
                  }
                  if (IsItPrev)
                  {
                    i++;
                    continue;
                  }
                  FindedPegs.Add((TStimIndex)i);
                  stims_to_remove.Add(stim);
                  break;
                }
              }
            }
          }
        }
        #endregion
      }
      lock (LockExternalData)
      {
        inner_data_to_display = DataPacket;
        inner_found_indexes_to_display = FindedPegs;
      }
      //Thread.Sleep(150);
      int x = FindedPegs.Count();
      #region Удаление найденных координат артефактов стимуляций из списка ожидаемых.
      #region  Добавим устаревшие на удаление.
      lock (LockStimList)
      {
        foreach (TStimGroup _stim in m_expectedStims)
        {
          if (_stim.stimTime < CurrentTime) stims_to_remove.Add(_stim);
        }
      }
      #endregion
      lock (LockStimList)
      {
        foreach (TStimGroup _stim in stims_to_remove)
        {
          if (m_expectedStims.Contains(_stim)) m_expectedStims.Remove(_stim);
        }
      }
      #endregion

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
        for (int i = 5; i < POST_SIGMA_CALC_DEPTH; i++)
        {
          post_average.AddValueElem(DataPacket[t + i]);
        }
        pre_average.Calc();
        post_average.Calc();
        #region Условия Подтверждения артефакта
        if (pre_average.IsInArea(DataPacket[t])
          && !pre_average.IsInArea(DataPacket[t + 1])
          && !pre_average.IsInArea(post_average.Value - pre_average.TripleSigma)
          && !pre_average.IsInArea(post_average.Value + pre_average.TripleSigma)
          //Clipping Fix
          && pre_average.Value > 0
          && pre_average.Value < 65535
          //StableLinesFix
          && Math.Abs(pre_average.Value - post_average.Value) > 220
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
    #region отрисовка
    public void DrawCallFunc()
    {
#if GRAPH
      DataRender = new CGraphRender();
      DataRender.IsMouseVisible = true;
      DataRender.SetDataObj(this);
      DataRender.Run();
#endif
    }
    #endregion
  }
  #region Среднее и сигма
  class Average
  {
    private List<double> values;
    public double Value = 0;
    public double TripleSigma = 0;
    public double Sigma = 0;
    private bool first_point = true;

    public void AddValueElem(double ValueToAdd)
    {
      values.Add(ValueToAdd);
    }
    public void Calc()
    {
      if (values != null && values.Count > 0)
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
    }
    public bool IsInArea(double Value)
    {
      this.Calc();
      if (Math.Abs(this.Value - Value) < TripleSigma) return true;
      return false;
    }
    public Average()
    {
      values = new List<double>();
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
