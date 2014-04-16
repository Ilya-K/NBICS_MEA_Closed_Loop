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
using MEAClosedLoop.Common;
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
    int HistoryLength = 2;
    bool editFlagLeft = true;
    bool editFlagRight = true;
    bool editFlagUp = true;
    bool editFlagDown = true;
    private int MCHCompress = 2;
    private int SCHCompress = 2;
    private int MCHYRange = 8;
    private int SCHYRange = 120;
    #endregion
    #region внутренние данные
    TFltDataPacket DataPacket; //последний пришедший пакет
    Queue<TFltDataPacket> DataPacketHistory; // история данных
    Queue<TAbsStimIndex> FoundStimData; // история найденных стимулов
    Queue<TAbsStimIndex> ExpStimData; // история ожидаемых стимулов

    object DataPacketLock = new object(); // блокировка данных
    public TTime summary_time_stamp = 0;
    CFiltering m_salpaFilter;
    GraphicsDeviceManager graphics;
    // эффект BasicEffect для кривой
    BasicEffect basicEffect;
    // массив массивов вершин нашей кривой
    VertexPositionColor[][] vertices;
    // массив массивов вершин для режима одного канала со сверткой
    //VertexPositionColor[][] schCompVerices;
    // массив массивов вершин для режима одного канала без свертки
    VertexPositionColor[] schUncompVerices;
    object VertexPositionLock = new object();
    DrawMode SelectedDrawMode;
    int SingleChannelNum = MEA.AR_DECODE[20];
    TTime HistoryTimeLength = 0;
    bool IsDataUpdated;
    
    #endregion
    public CDataRender(CFiltering salpaFilter)
    {
      graphics = new GraphicsDeviceManager(this);

      graphics.PreferredBackBufferWidth = DEFAULT_WINDOW_WIDTH; // ширина приложения
      graphics.PreferredBackBufferHeight = DEFAULT_WINDOW_HEIGHT; // высота приложения
      graphics.IsFullScreen = false; // флаг полноэкранного приложения
      graphics.ApplyChanges(); // применяем параметры
      this.SelectedDrawMode = CDataRender.DrawMode.DrawMultiChannel;
      (System.Windows.Forms.Control.FromHandle(this.Window.Handle)).DoubleClick += ChangeDrawMode;

      IsDataUpdated = false;

      DataPacketHistory = new Queue<TFltDataPacket>(HistoryLength);

      this.m_salpaFilter = salpaFilter;
      this.Window.AllowUserResizing = true;
      this.IsMouseVisible = true;

      m_salpaFilter.AddDataConsumer(RecieveFltData);
      m_salpaFilter.AddStimulConsumer(RecieveStimData);
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
      #region Изменение длины очереди
      if (Keyboard.GetState().IsKeyDown(Keys.Left))
      {
        if (editFlagLeft)
        {
          editFlagLeft = false;
          HistoryLength = (HistoryLength > 2) ? HistoryLength - 2 : HistoryLength;
        }
      }
      if (Keyboard.GetState().IsKeyUp(Keys.Left)) editFlagLeft = true;

      if (Keyboard.GetState().IsKeyDown(Keys.Right))
      {
        if (editFlagRight)
        {
          editFlagRight = false;
          HistoryLength += 2;
        }
      }
      if (Keyboard.GetState().IsKeyUp(Keys.Right)) editFlagRight = true;
      #endregion
      #region Изменение пропорции по высоте
      if (Keyboard.GetState().IsKeyDown(Keys.Up) && editFlagUp)
      {
        editFlagUp = false;
        switch (this.SelectedDrawMode)
        {
          case DrawMode.DrawMultiChannel:
            MCHYRange = (MCHYRange > 2) ? MCHYRange - 2 : MCHYRange;
            break;
          case DrawMode.DrawSingleChannel:
            SCHYRange = (SCHYRange > 2) ? MCHYRange - 2 : MCHYRange;
            break;
        }
      }
      if (Keyboard.GetState().IsKeyDown(Keys.Down) && editFlagDown)
      {
        editFlagDown = false;
        switch (this.SelectedDrawMode)
        {
          case DrawMode.DrawMultiChannel:
            MCHYRange += 2;
            break;
          case DrawMode.DrawSingleChannel:
            SCHYRange += 2;
            break;
        }
      }
      if (Keyboard.GetState().IsKeyUp(Keys.Up)) editFlagUp = true;

      if (Keyboard.GetState().IsKeyUp(Keys.Down)) editFlagDown = true;
      #endregion
      base.Update(gameTime);
    }
    public void RecieveFltData(TFltDataPacket data)
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
        summary_time_stamp = m_salpaFilter.TimeStamp + (TTime)DataPacket[AnyExistsKey].Length;
        HistoryTimeLength = 0;
        for (int i = 0; i < DataPacketHistory.Count; i++)
        {
          HistoryTimeLength += (TTime)DataPacketHistory.ElementAt(i)[SingleChannelNum].Length;
        }
      }
      IsDataUpdated = false;

    }
    public void SetExpStimData(TStimGroup stim) { }
    public void RecieveStimData(List<TAbsStimIndex> stims) 
    {

      lock (DataPacketLock)
      {
        foreach (TAbsStimIndex stim in stims)
        {
          FoundStimData.Enqueue(stim);
        }
        //while (FoundStimData.Peek() <   )
        {
        }

      }
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
          SingleChannelNum = (X + 1) * 10 + (Y + 1);

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
      if (DataPacketHistory.Count == 0) return;
      //Получаем размеры окна;
      int WindowHeight = graphics.PreferredBackBufferHeight;
      //graphics.PreferredBackBufferFormat = SurfaceFormat.Color;

      int WindowWidth = graphics.PreferredBackBufferWidth;
      int FormWidth = (System.Windows.Forms.Control.FromHandle(this.Window.Handle)).Width;

      TFltData[] data_to_display;
      switch (this.SelectedDrawMode)
      {
        case DrawMode.DrawMultiChannel:
          #region Отрисовка всех каналов
          GraphicsDevice.Clear(Color.CornflowerBlue);
          basicEffect.CurrentTechnique.Passes[0].Apply();

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



          foreach (int key in DataPacket.Keys)
          {
            //подготовка массива массивов точек
            IsDataUpdated = false;
            if (!IsDataUpdated)
            {
              #region обновление массивов точек
              // склеим "исторические" данные
              int length = 0;
              lock (DataPacketLock)
              {
                for (int i = 0; i < DataPacketHistory.Count; i++)
                {
                  length += DataPacketHistory.ElementAt(i)[key].Length;
                }
                if (length == 0) return;
                //MCHCompress = HistoryLength / 5;

                //MCHCompress = length / (CellWidth);
                //if (MCHCompress > 10) MCHCompress = 10;
                data_to_display = new TFltData[length];
                int currentlength = 0;
                for (int i = 0; i < DataPacketHistory.Count; i++)
                {
                  for (int j = 0; j < DataPacketHistory.ElementAt(i)[key].Length; j += 1)
                  {
                    data_to_display[currentlength + j] = DataPacketHistory.ElementAt(i)[key][j];
                  }
                  currentlength += DataPacketHistory.ElementAt(i)[key].Length;
                }
              }
              for (int i = 0; i < length; i += MCHCompress)
              {
                float max = float.MinValue;
                float min = float.MaxValue;
                for (int z = 0; z < MCHCompress; z++)
                {
                  float t = (float)data_to_display[i + z];
                  if (t > max) max = t;
                  if (t < min) min = t;
                }
                VertexPositionColor[] line = new VertexPositionColor[2];
                line[0].Position.X = ((float)(i) * CellWidth) / length;
                line[0].Position.Y = max * MCHYRange / 100 + CellHeight / 2;
                line[0].Position.Z = 0;
                line[1].Position.X = line[0].Position.X;
                line[1].Position.Y = min * MCHYRange / 100 + CellHeight / 2;
                line[1].Position.Z = 0;
                if (line[0].Position.Y < 0) line[0].Position.Y = 0;
                if (line[1].Position.Y > CellHeight) line[1].Position.Y = CellHeight;
                line[0].Position += ChannelvectorsArray[RealChannelIndx];
                line[1].Position += ChannelvectorsArray[RealChannelIndx];
                if (i == 5000)
                {
                  //break point
                }
                graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, line, 0, 1);

              }
              // old
              //vertices[RealChannelIndx] = new VertexPositionColor[data_to_display.Length];

              //for (int i = 0; i < data_to_display.Length; i++)
              //{
              //vertices[RealChannelIndx][i].Position.X = ((float)i * CellWidth) / data_to_display.Length;
              //vertices[RealChannelIndx][i].Position.Y = CellHeight / 2 - (float)data_to_display[i] / MCHYRange;
              //if (vertices[RealChannelIndx][i].Position.Y < 0) vertices[RealChannelIndx][i].Position.Y = 0;
              //if (vertices[RealChannelIndx][i].Position.Y > CellHeight) vertices[RealChannelIndx][i].Position.Y = CellHeight;
              //vertices[RealChannelIndx][i].Position.X += ChannelvectorsArray[RealChannelIndx].X;
              //vertices[RealChannelIndx][i].Position.Y += ChannelvectorsArray[RealChannelIndx].Y;
              //vertices[RealChannelIndx][i].Position.Z = 0;

              //vertices[RealChannelIndx][i].Color = (Math.Abs(data_to_display[i]) > 120) ? Color.Red : Color.Black;
              //}
            }
              #endregion
            //if(RealChannelIndx == 13) 
            //graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices[RealChannelIndx], 0, vertices[RealChannelIndx].Count() - 1);
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
          #endregion
        case DrawMode.DrawSingleChannel:
          #region Отрисовка одного канала
          GraphicsDevice.Clear(Color.White);
          basicEffect.CurrentTechnique.Passes[0].Apply();
          IsDataUpdated = false;
          if (!IsDataUpdated)
          {
            vertices = new VertexPositionColor[DataPacket.Keys.Count][];
            lock (DataPacketLock)
            {
              int length = 0;
              int currentlength = 0;
              for (int i = 0; i < DataPacketHistory.Count; i++)
              {
                length += DataPacketHistory.ElementAt(i)[SingleChannelNum].Length;
              }
              data_to_display = new TFltData[length];
              for (int PacketNum = 0; PacketNum < DataPacketHistory.Count; PacketNum++)
              {
                for (int j = 0; j < DataPacketHistory.ElementAt(PacketNum)[SingleChannelNum].Length; j++)
                {
                  data_to_display[currentlength + j] = DataPacketHistory.ElementAt(PacketNum)[SingleChannelNum][j];
                }
                currentlength += DataPacketHistory.ElementAt(PacketNum)[SingleChannelNum].Length;
              }
            }
            double PointsPerPX = ((double)data_to_display.Length) / FormWidth;
            SCHCompress = (PointsPerPX < 1) ? 1 : (int)PointsPerPX;
            if (PointsPerPX <= 1)
            {
              //отрисовка без наложения линий - готовим сплошной массив и его рисуем
              schUncompVerices = new VertexPositionColor[data_to_display.Length];
              for (int i = 0; i < data_to_display.Length; i++)
              {
                schUncompVerices[i].Position.X = ((float)i * WindowWidth) / data_to_display.Length;
                schUncompVerices[i].Position.Y = WindowHeight / 2 - (float)data_to_display[i] * SCHYRange / 100;
                if (schUncompVerices[i].Position.Y < 0) schUncompVerices[i].Position.Y = 0;
                if (schUncompVerices[i].Position.Y > WindowHeight) schUncompVerices[i].Position.Y = WindowHeight;
                schUncompVerices[i].Position.Z = 0;
                schUncompVerices[i].Color = (Math.Abs(data_to_display[i]) > 100) ? Color.Red : Color.Blue;
              }
              graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, schUncompVerices, 0, schUncompVerices.Count() - 1);
            }
            else
            {
              // отрисовка с наложением линий друг на друга - рисуем вертикальные линии от максимума до минимума
              // NOTE: наиболее частый случай
              int length = data_to_display.Length;
              for (int i = SCHCompress; i < length - 1; i += SCHCompress)
              {
                float max = float.MinValue;
                float min = float.MaxValue;
                for (int z = 0; z < SCHCompress; z++)
                {
                  float t = (float)data_to_display[i - z];
                  if (t > max) max = t;
                  if (t < min) min = t;
                }
                VertexPositionColor[] line = new VertexPositionColor[2];
                line[0].Position.X = ((float)(i) * WindowWidth) / length;
                line[0].Position.Y = max * SCHYRange / 100 + WindowHeight / 2;
                line[0].Position.Z = 0;
                line[1].Position.X = line[0].Position.X;
                line[1].Position.Y = min * SCHYRange / 100 + WindowHeight / 2;
                line[1].Position.Z = 0;
                if (line[0].Position.Y < 0) line[0].Position.Y = 0;
                if (line[1].Position.Y > WindowHeight) line[1].Position.Y = WindowHeight;
                //случай сплошного(длинного) нуля - горизонтальной прямой
                if (line[1].Position.Equals(line[0].Position))
                  line[1].Position.X += ((float)WindowWidth / (float)length);

                graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, line, 0, 1);
                //отрисовка соединяющей линии

                for (int z = 0; z < SCHCompress; z++)
                {
                  float t = (float)data_to_display[i + 1 - z];
                  if (t > max) max = t;
                  if (t < min) min = t;
                }
                line[1].Position.X +=((float)WindowWidth / (float)length);
                line[1].Position.Y = max * SCHYRange / 100 + WindowHeight / 2;
                line[1].Position.Z = 0;
                if (line[1].Position.Y > WindowHeight) line[1].Position.Y = WindowHeight;
                //случай сплошного(длинного) нуля - горизонтальной прямой
                if (line[1].Position.Equals(line[0].Position))
                  line[1].Position.X += ((float)WindowWidth / (float)length);

                graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, line, 0, 1);

              }
            }
            //###### old
            /*
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
                  for (int j = 0; j < DataPacketHistory.ElementAt(i)[key].Length; j += SCHCompress)
                  {
                    data_to_display[currentlength / SCHCompress + j / SCHCompress] = DataPacketHistory.ElementAt(i)[SingleChannelNum][j];
                  }
                  currentlength += DataPacketHistory.ElementAt(i)[key].Length;
                }

                vertices[SingleChannelNum] = new VertexPositionColor[data_to_display.Length];
                for (int i = 0; i < data_to_display.Length; i++)
                {
                  vertices[SingleChannelNum][i].Position.X = ((float)i * WindowWidth) / data_to_display.Length;
                  vertices[SingleChannelNum][i].Position.Y = WindowHeight / 2 - 10 * (float)data_to_display[i] * SCHYRange / 100;
                  if (vertices[SingleChannelNum][i].Position.Y < 0) vertices[SingleChannelNum][i].Position.Y = 0;
                  if (vertices[SingleChannelNum][i].Position.Y > WindowHeight) vertices[SingleChannelNum][i].Position.Y = WindowHeight;
                  vertices[SingleChannelNum][i].Position.Z = 0;
                  vertices[SingleChannelNum][i].Color = (Math.Abs(data_to_display[i]) > 100) ? Color.Red : Color.Blue;
                }
              }
            }
             */

          }
          IsDataUpdated = true;
          break;
          #endregion
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


