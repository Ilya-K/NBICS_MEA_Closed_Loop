﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEAClosedLoop
{
    using TTime = UInt64;
    using TPack = List<bool>;

    public class CStat
    {
      public struct windowBorder
      {
        public uint winStart;
        public uint winEnd;
      }

        const double Psp = 0.1;
        TTime m_totalPackPeriod;
        uint m_packCount;
        TTime m_prevPackStart;
        
      const int PACK_DETECTED_PERCENT_CRITERION = 50;
      const TTime DEFAULT_DEAD_ZONE_SIZE = 10;
      const TTime DEFAULT_WIN_LENGTH = 20;

        private windowBorder FindWindow(TPack pack, TTime deadZoneSize, TTime winLength)
        {
            uint emptyCombo = 0, emptyComboStart=0;
            windowBorder output;
            for (uint Iterator = (uint)deadZoneSize; Iterator < Convert.ToUInt64(pack.Count()); Iterator++)
            {
                if (pack[(int)Iterator])
                { //combo break
                  if (emptyCombo >= winLength)
                  {
                    output.winStart = emptyComboStart;
                    output.winEnd = Iterator;
                    return output;
                  }
                  emptyCombo = 0;
                }
                else
                {
                    if (emptyCombo == 0)
                        emptyComboStart = Iterator;

                    emptyCombo++;
                }
            }

            if (emptyCombo >= winLength)
            {
              output.winStart = emptyComboStart;
              output.winEnd = Convert.ToUInt32(pack.Count());
              return output;
            }

            output.winStart = Convert.ToUInt32(pack.Count());
            output.winEnd = Convert.ToUInt32(pack.Count());
            return output; //return last pack moment if no windows found
        }
        
        public CStat()
        {
            //constructor
            m_packCount = 0;
            m_totalPackPeriod = 0;
        }

        public void AddPack(TPack pack, TTime packStart)
        {
            if (m_packCount >= 1)
            {
                m_totalPackPeriod += packStart - m_prevPackStart;
            }


            m_prevPackStart = packStart;
            m_packCount++;
        }

        public TTime AvgPackPeriod()
        {
            return m_totalPackPeriod / m_packCount;
        }

        public uint WindowStat(List<TPack> all_packs)
        {
          List<windowBorder> foundWindows = new List<windowBorder>();
          //List<uint> coverMap = new List<uint>List
          double foundWindowPercent;
          windowBorder currentWindow;
          foreach(TPack PackIterator in all_packs){
            currentWindow = FindWindow(PackIterator, DEFAULT_DEAD_ZONE_SIZE, DEFAULT_WIN_LENGTH);
            if(currentWindow.winStart != (uint)(PackIterator.Count())){
              foundWindows.Add(currentWindow);
            }
          }

          foundWindowPercent = foundWindows.Count() * 100 / all_packs.Count();
          if(foundWindowPercent < PACK_DETECTED_PERCENT_CRITERION)
            return uint.MaxValue;



          return uint.MaxValue; //if none found
        }
        


    }
}
