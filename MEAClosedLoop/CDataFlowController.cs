using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEAClosedLoop
{
  public class CDataFlowController
  {
    public void AddConsumer<ObjType>(ObjType Obj)
    {//одписывание интерфейсов на раздачу потоков данных
      if ((from obj in BurstrRecievers where obj.Equals(Obj) select obj).Count() == 0) return;

      if (Obj is IRecieveBusrt)
      {
        BurstrRecievers.Add(Obj as IRecieveBusrt);
      }
      if (Obj is IRecieveFltData)
      {
        throw new NotImplementedException("in develop:");
      }
      if (Obj is IRecieveStim)
      {
        StimRecievers.Add(Obj as IRecieveStim);
      }
    }
    public void RemoveConsumer<ObjType>(ObjType Obj)
    {//одписывание интерфейсов на раздачу потоков данных
      if (Obj is IRecieveBusrt)
      {
        throw new NotImplementedException("in develop:");
      }
      if (Obj is IRecieveFltData)
      {
        throw new NotImplementedException("in develop:");
      }
      if (Obj is IRecieveStim)
      {
        throw new NotImplementedException("in develop:");
      }
    }
  }
}
