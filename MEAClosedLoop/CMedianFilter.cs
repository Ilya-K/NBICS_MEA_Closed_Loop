using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEAClosedLoop
{
  class CMedianFilter
  {
    public CMedianFilter()
    {

    }

    static private double Median5(double a, double b, double c, double d, double e)
    {
      return b < a ? d < c ? b < d ? a < e ? a < d ? e < d ? e : d
                                                   : c < a ? c : a
                                           : e < d ? a < d ? a : d
                                                   : c < e ? c : e
                                   : c < e ? b < c ? a < c ? a : c
                                                   : e < b ? e : b
                                           : b < e ? a < e ? a : e
                                                   : c < b ? c : b
                           : b < c ? a < e ? a < c ? e < c ? e : c
                                                   : d < a ? d : a
                                           : e < c ? a < c ? a : c
                                                   : d < e ? d : e
                                   : d < e ? b < d ? a < d ? a : d
                                                   : e < b ? e : b
                                           : b < e ? a < e ? a : e
                                                   : d < b ? d : b
                   : d < c ? a < d ? b < e ? b < d ? e < d ? e : d
                                                   : c < b ? c : b
                                           : e < d ? b < d ? b : d
                                                   : c < e ? c : e
                                   : c < e ? a < c ? b < c ? b : c
                                                   : e < a ? e : a
                                           : a < e ? b < e ? b : e
                                                   : c < a ? c : a
                           : a < c ? b < e ? b < c ? e < c ? e : c
                                                   : d < b ? d : b
                                           : e < c ? b < c ? b : c
                                                   : d < e ? d : e
                                   : d < e ? a < d ? b < d ? b : d
                                                   : e < a ? e : a
                                           : a < e ? b < e ? b : e
                                                   : d < a ? d : a;
    }
  }
}
