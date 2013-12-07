using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEAClosedLoop
{
    using TTime = UInt64;

    public class CStat
    {
        const double Psp = 0.1;
        TTime m_totalPackPeriod;
        uint m_packCount;
        TTime m_prevPackStart;

        private uint FindWindow(List<bool> pack, TTime deadZoneSize, TTime winLength)
        {
            uint emptyCombo = 0, emptyComboStart=0;
            for (uint Iterator = (uint)deadZoneSize; Iterator < Convert.ToUInt64(pack.Count()); Iterator++)
            {
                if (pack[(int)Iterator])
                { //combo break
                    emptyCombo = 0;
                }
                else
                {
                    if (emptyCombo == 0)
                        emptyComboStart = Iterator;

                    emptyCombo++;

                    if (emptyCombo >= winLength)
                        return emptyComboStart;
                }
            }
            
            return (uint)(pack.Count()); //return last pack moment if no windows found
        }
        
        public CStat()
        {
            //constructor
            m_packCount = 0;
            m_totalPackPeriod = 0;
        }

        public void AddPack(List<bool> pack, TTime packStart)
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
        


    }
}
