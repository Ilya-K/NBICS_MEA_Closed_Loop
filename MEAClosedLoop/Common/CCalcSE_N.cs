using System;
using System.Text;

namespace MEAClosedLoop
{
  using TRawData = UInt16;
  using TData = Double;

  // [Not yet] This class provides a method to calculate moving SE over N samples

  // This class provides a method to calculate SE over adjacent blocks of N samples
  // Считает стандартное отклонение (SE = Standard Error) по соседним участкам шириной N.
  // Т.е. если ему на вход приходит 2500 отсчётов и N=15, например, то он выдаст массив 
  // из 166 посчитанных SE и ещё 10 точек останется и они прилепятся в начало следующего блока.
  // Потом по ним я ищу момент, когда SE упало ниже плинтуса :)
  public class CCalcSE_N 
  {
    private readonly int N_SAMPLES;
    private UInt32 partialSum;
    private UInt64 partialSum2;
    private int partialN;
    public int PartialN { get { return partialN; } }
    public int Width { get { return N_SAMPLES; } }

    /// <summary>
    /// Create SE calculator
    /// </summary>
    /// <param name="n">Number of samples to calculate SE over</param>
    public CCalcSE_N(int n)
    {
      N_SAMPLES = n;
      partialSum = 0;
      partialSum2 = 0;
      partialN = 0;
    }

    /// <summary>
    /// Calculate Standard Error (SE) of adjacent blocks of width N_SAMPLES
    /// Reminder will be taken into account on the subsequent call of this function
    /// </summary>
    /// <param name="data">Array of TRawData type (ushort in our case) </param>
    /// <returns>Array of TData type (double in our case) cointaining SE of adjacent blocks</returns>
    /// [TODO] Ошибка при переменной длине пакетов
    public TData[] SE(TRawData[] data)
    {
      int length = data.Length;
      int nBlocks = (length + partialN) / N_SAMPLES;
      int reminder = (length + partialN) % N_SAMPLES;
      TData[] result = new TData[nBlocks];

      int counter = 0;
      int shift = 0;
      while (true)
      {
        if (counter == nBlocks)                             // Process final block if exists (reminder > 0)
        {
          partialN = reminder;
          for (int i = shift; i < shift + partialN; ++i)
          {
            partialSum += data[i];
            partialSum2 += ((uint)data[i] * (uint)data[i]);
          }
          break;                                            // and exit
        }

        int nSamples = N_SAMPLES;
        if (counter == 0) nSamples -= partialN;             // Process incomplete buffer first

        for (int i = shift; i < shift + nSamples; ++i)      // Process full length buffers in the middle
        {
          partialSum += data[i];
          partialSum2 += ((uint)data[i] * (uint)data[i]);
        }
        shift += nSamples; 

        // SE = sqrt(mean(X^2) - (mean(X))^2)
        result[counter++] = Math.Sqrt(((TData)partialSum2 - (TData)((UInt64)partialSum * partialSum) / N_SAMPLES) / N_SAMPLES);
        partialSum = 0;
        partialSum2 = 0;
      }
      return result;
    }
  }
}
