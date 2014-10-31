using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MEAClosedLoop.UI_Forms
{
  public partial class FBurstFormDisplay : Form
  {
    private SEvokedPack[] BurstQueue;
    private int CurrentCh;
    public FBurstFormDisplay()
    {
      InitializeComponent();
    }
    public FBurstFormDisplay(Queue<SEvokedPack> _BurstQueue, int Ch)
    {
      _BurstQueue.CopyTo(BurstQueue, 0);
      CurrentCh = Ch;
    }

    private void FBurstFormDisplay_Load(object sender, EventArgs e)
    {
      PrepeareDrawData(CurrentCh);
      PicBox.Refresh();
    }

    private void PrepeareDrawData(int Ch)
    {
      if (BurstQueue != null) return;

      //int length = (from b in BurstQueue select b.Burst.Data[Ch].Length).Max();
      PointF[][] dataToPlot = new PointF[BurstQueue.Count()][];

      for (int i = 0; i < BurstQueue.Count(); i++)
      {
        int length = BurstQueue.ElementAt(i).Burst.Data[Ch].Length;
        dataToPlot[i] = new PointF[length];
        SEvokedPack evPack = BurstQueue.ElementAt(i);
        for (int j = 0; j < length ; j++)
        {
          dataToPlot[i][j] = new PointF((float)(j * .2), (float)(evPack.Burst.Data[Ch][j] + PicBox.Height/2.0));
        }
      }
    }
  }
}
