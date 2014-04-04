using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using MEAClosedLoop;
using System.Windows;
namespace MEAClosedLoop
{
  #region Definitions
  using TData = System.Double;
  using TTime = System.UInt64;
  using TStimIndex = System.Int16;
  using TAbsStimIndex = System.UInt64;
  using TRawData = UInt16;
  using TRawDataPacket = Dictionary<int, ushort[]>;
  using TFltDataPacket = Dictionary<int, System.Double[]>;
  using TFltData = System.Double;
  #endregion
  public class CDataRender : Microsoft.Xna.Framework.Game
  {
    #region стандартные значения
    private int DEFAULT_WINDOW_HEIGHT = 600;
    private int DEFAULT_WINDOW_WIDTH = 800;
    int HistoryLength = 40;
    private int MCHCompress = 10;
    private int SCHCompress = 2;
    private int MCHYRange = 10;
    private int SCHYRange = 2;
    #endregion
    #region внутренние данные
    TFltDataPacket DataPacket; //Данные для отрисовки фильтрованных данных
    Queue<TFltDataPacket> DataPacketHistory; // история данных
    object DataPacketLock = new object(); // блокировка данных
    TTime summary_time_stamp = 0;
    GraphicsDeviceManager graphics;
    // эффект BasicEffect для кривой
    BasicEffect basicEffect;
    // массив массивов вершин нашей кривой
    VertexPositionColor[][] vertices;
    object VertexPositionLock = new object();
    DrawMode SelectedDrawMode;
    int SingleChannelNum = MEA.AR_DECODE[20];
    bool IsDataUpdated;

    #endregion

    public CDataRender()
    {
      graphics = new GraphicsDeviceManager(this);

      graphics.PreferredBackBufferWidth = DEFAULT_WINDOW_WIDTH; // ширина приложения
      graphics.PreferredBackBufferHeight = DEFAULT_WINDOW_HEIGHT; // высота приложения
      graphics.IsFullScreen = false; // флаг полноэкранного приложения
      graphics.ApplyChanges(); // применяем параметры
      this.SelectedDrawMode = CDataRender.DrawMode.DrawMultiChannel;
      (System.Windows.Forms.Control.FromHandle(this.Window.Handle)).DoubleClick += ChangeDrawMode;
      (System.Windows.Forms.Control.FromHandle(this.Window.Handle)).KeyPress += CDataRender_KeyPress;
      
      IsDataUpdated = false;
     
      DataPacketHistory = new Queue<TFltDataPacket>(HistoryLength);
      System.Windows.Forms.MessageBox.Show("");
    }

    void CDataRender_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
    {
      if (e.KeyChar.Equals('w'))
      {

      }
      if (e.KeyChar.Equals('s'))
      {
      } if (e.KeyChar.Equals('a'))
      {
        HistoryLength = (HistoryLength > 1) ? HistoryLength - 1 : HistoryLength;
      }
      if (e.KeyChar.Equals('d'))
      {
        HistoryLength = (HistoryLength > 1) ? HistoryLength + 1 : HistoryLength;
      }
    }
    protected override void Initialize()
    {
      (System.Windows.Forms.Control.FromHandle(this.Window.Handle)).Location = new System.Drawing.Point(0, 0);
      basicEffect = new BasicEffect(graphics.GraphicsDevice);
      basicEffect.VertexColorEnabled = true;
      basicEffect.Projection = Matrix.CreateOrthographicOffCenter
         (0, graphics.GraphicsDevice.Viewport.Width,     // left, right
          graphics.GraphicsDevice.Viewport.Height, 0,    // bottom, top
          0, 1);                                         // near, far plane

      base.Initialize();
    }
    protected override void LoadContent()
    {

    }
    protected override void UnloadContent()
    {
    }
    protected override void Update(GameTime gameTime)
    {
      // Allows the game to exit
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
        this.Exit();

      // TODO: Add your update logic here

      base.Update(gameTime);
    }
    public void RecivieFltData(TFltDataPacket data)
    {
      lock (DataPacketLock)
      {
        DataPacket = new TFltDataPacket(data);
        //DataPacketHistory.Add(DataPacket);
        DataPacketHistory.Enqueue(DataPacket);

        //если очередь полна
        for (; DataPacketHistory.Count >= HistoryLength; DataPacketHistory.Dequeue()) ;
        int AnyExistsKey = DataPacket.Keys.First();
        //Текущее время (время на конец последнего пакета)
        summary_time_stamp += (TTime)DataPacket[AnyExistsKey].Length;
      }
      IsDataUpdated = false;

    }
    public void ChangeDrawMode(object sender, EventArgs e)
    {
      MouseState mousestate = Mouse.GetState();
      int WindowHeight = (System.Windows.Forms.Control.FromHandle(this.Window.Handle)).Height;
      int WindowWidth = (System.Windows.Forms.Control.FromHandle(this.Window.Handle)).Width;
      switch (this.SelectedDrawMode)
      {
        case DrawMode.DrawMultiChannel:
          int X = 0, Y = 0;
          for (int i = 0; i < 8; i++)
          {
            if (mousestate.X > i * WindowWidth / 8 && mousestate.X < (i + 1) * WindowWidth / 8)
            {
              X = i;
              break;
            }
          }
          for (int i = 0; i < 8; i++)
          {
            if (mousestate.Y > i * WindowHeight / 8 && mousestate.Y < (i + 1) * WindowHeight / 8)
            {
              Y = i;
              break;
            }
          }
          SingleChannelNum = (X + 1) * 10 + (Y + 1);//MEA.AR_DECODE[X * 8 + Y];

          //System.Windows.Forms.MessageBox.Show("Double click at x = " + X.ToString() + "y = " + Y.ToString());
          lock (DataPacketLock)
          {
            if (MEA.EL_DECODE[SingleChannelNum] >= 0)
            {
              SingleChannelNum = MEA.EL_DECODE[SingleChannelNum];
              this.SelectedDrawMode = DrawMode.DrawSingleChannel;
            }
            IsDataUpdated = false;
          }
          break;
        case DrawMode.DrawSingleChannel:

          lock (DataPacketLock)
          {
            this.SelectedDrawMode = DrawMode.DrawMultiChannel;

            IsDataUpdated = false;
          }

          break;

      }
    }
    protected override void Draw(GameTime gameTime)
    {
      if (DataPacket == null) return;

      //Получаем размеры окна;
      int WindowHeight = graphics.PreferredBackBufferHeight;
      int WindowWidth = graphics.PreferredBackBufferWidth;


      TFltData[] data_to_display;
      switch (this.SelectedDrawMode)
      {
        case DrawMode.DrawMultiChannel:

          GraphicsDevice.Clear(Color.CornflowerBlue);
          //создание массива векторов для каналов
          List<Vector3> ChannelVectors = new List<Vector3>();
          Vector3[] ChannelvectorsArray = new Vector3[DataPacket.Keys.Count()];
          foreach (int key in DataPacket.Keys)
          {
            ChannelVectors.Add(new Vector3());
          }
          // сетка 8 x 8 клеток
          // длина и ширина пропорциональны размеру главного окна
          int CellWidth = WindowWidth / 8;
          int CellHeight = WindowHeight / 8;

          for (int i = 0; i < 8; i++)
          {
            for (int j = 0; j < 8 && i * 8 + j < ChannelvectorsArray.Length; j++)
            {
              ChannelvectorsArray[i * 8 + j] = new Vector3((MEA.AR_DECODE[i * 8 + j] / 10 - 1) * CellWidth, (MEA.AR_DECODE[i * 8 + j] % 10 - 1) * CellHeight, 0);
            }
          }
          int RealChannelIndx = 0;

          if (!IsDataUpdated) vertices = new VertexPositionColor[DataPacket.Keys.Count][];

          basicEffect.CurrentTechnique.Passes[0].Apply();

          foreach (int key in DataPacket.Keys)
          {
            //подготовка массива массивов точек

            if (!IsDataUpdated)// && RealChannelIndx == 13)
            {
              lock (DataPacketLock)
              {
                #region обновление массивов точек
                // склеим "исторические" данные
                int length = 0;
                for (int i = 0; i < DataPacketHistory.Count; i++)
                {
                  length += DataPacketHistory.ElementAt(i)[key].Length;
                }
                data_to_display = new TFltData[length / MCHCompress];
                int currentlength = 0;
                for (int i = 0; i < DataPacketHistory.Count; i++)
                {
                  for (int j = MCHCompress; j < DataPacketHistory.ElementAt(i)[key].Length / MCHCompress; j += MCHCompress)
                  {
                    double max = 0;
                    double average = 0;
                    for (int z = 0; z < MCHCompress; z++)
                    {
                      double t = DataPacketHistory.ElementAt(i)[RealChannelIndx][j - z];
                      if (Math.Abs(t) > Math.Abs(max)) max = t;
                      average += t / MCHCompress;
                    }
                    data_to_display[currentlength + j] = (max + average) / 2;
                  }
                  currentlength += DataPacketHistory.ElementAt(i)[key].Length / MCHCompress;
                }

                vertices[RealChannelIndx] = new VertexPositionColor[data_to_display.Length];

                for (int i = 0; i < data_to_display.Length; i++)
                {
                  vertices[RealChannelIndx][i].Position.X = ((float)i * CellWidth) / data_to_display.Length;
                  vertices[RealChannelIndx][i].Position.Y = CellHeight / 2 - (float)data_to_display[i] / MCHYRange;
                  if (vertices[RealChannelIndx][i].Position.Y < 0) vertices[RealChannelIndx][i].Position.Y = 0;
                  if (vertices[RealChannelIndx][i].Position.Y > CellHeight) vertices[RealChannelIndx][i].Position.Y = CellHeight;
                  vertices[RealChannelIndx][i].Position.X += ChannelvectorsArray[RealChannelIndx].X;
                  vertices[RealChannelIndx][i].Position.Y += ChannelvectorsArray[RealChannelIndx].Y;
                  vertices[RealChannelIndx][i].Position.Z = 0;

                  vertices[RealChannelIndx][i].Color = (Math.Abs(data_to_display[i]) > 120) ? Color.Red : Color.Black;
                }
              }
            }
                #endregion
            //if(RealChannelIndx == 13) 
            graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices[RealChannelIndx], 0, vertices[RealChannelIndx].Count() - 1);
            RealChannelIndx++;

          }
          IsDataUpdated = true;

          // отрисовка сетки каналов
          VertexPositionColor[] HorizontalLinesPoints = new VertexPositionColor[2];
          VertexPositionColor[] VericalLinesPoints = new VertexPositionColor[2];
          for (int i = 1; i < 8; i++)
          {
            HorizontalLinesPoints[0].Position = new Vector3(0, i * CellHeight, 0);
            HorizontalLinesPoints[0].Color = Color.Green;
            HorizontalLinesPoints[1].Position = new Vector3(WindowWidth, i * CellHeight, 0);
            HorizontalLinesPoints[1].Color = Color.Green;

            VericalLinesPoints[0].Position = new Vector3(i * CellWidth, 0, 0);
            VericalLinesPoints[0].Color = Color.Green;
            VericalLinesPoints[1].Position = new Vector3(i * CellWidth, WindowHeight, 0);
            VericalLinesPoints[1].Color = Color.Green;

            graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, HorizontalLinesPoints, 0, 1);
            graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, VericalLinesPoints, 0, 1);
          }
          break;
        case DrawMode.DrawSingleChannel:

          GraphicsDevice.Clear(Color.White);
          basicEffect.CurrentTechnique.Passes[0].Apply();

          if (!IsDataUpdated)
          {
            vertices = new VertexPositionColor[DataPacket.Keys.Count][];
            lock (DataPacketLock)
            {
              foreach (int key in DataPacket.Keys)
              {
                if (key.Equals(SingleChannelNum))
                {
                  int length = 0;
                  for (int i = 0; i < DataPacketHistory.Count; i++)
                  {
                    length += DataPacketHistory.ElementAt(i)[key].Length;
                  }
                  data_to_display = new TFltData[length / SCHCompress];

                  int currentlength = 0;
                  for (int i = 0; i < DataPacketHistory.Count; i++)
                  {
                    for (int j = 0; j < DataPacketHistory.ElementAt(i)[key].Length / SCHCompress; j += SCHCompress)
                    {
                      data_to_display[currentlength + j] = DataPacketHistory.ElementAt(i)[SingleChannelNum][j];
                    }
                    currentlength += DataPacketHistory.ElementAt(i)[key].Length / SCHCompress;
                  }

                  vertices[SingleChannelNum] = new VertexPositionColor[data_to_display.Length];
                  for (int i = 0; i < data_to_display.Length; i++)
                  {
                    vertices[SingleChannelNum][i].Position.X = ((float)i * WindowWidth) / data_to_display.Length;
                    vertices[SingleChannelNum][i].Position.Y = WindowHeight / 2 - (float)data_to_display[i] / SCHYRange;
                    if (vertices[SingleChannelNum][i].Position.Y < 0) vertices[SingleChannelNum][i].Position.Y = 0;
                    if (vertices[SingleChannelNum][i].Position.Y > WindowHeight) vertices[SingleChannelNum][i].Position.Y = WindowHeight;
                    vertices[SingleChannelNum][i].Position.Z = 0;
                    vertices[SingleChannelNum][i].Color = (Math.Abs(data_to_display[i]) > 120) ? Color.Red : Color.Blue;
                  }
                }
              }
            }
          }
          graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices[SingleChannelNum], 0, vertices[SingleChannelNum].Count() - 1);
          IsDataUpdated = true;
          break;
      }
      base.Draw(gameTime);
    }
    private enum DrawMode
    {
      DrawSingleChannel,
      DrawMultiChannel
    }
  }
}


