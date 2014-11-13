using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Threading;
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
  public class CDataRenderNew : Microsoft.Xna.Framework.Game
  {
    #region стандартные значения
    private int DEFAULT_WINDOW_HEIGHT = 600;
    private int DEFAULT_WINDOW_WIDTH = 800;
    int HistoryLength = 20;
    bool editFlagLeft = true;
    bool editFlagRight = true;
    bool editFlagUp = true;
    bool editFlagDown = true;

    private int DebugCount = 0;
    private int MCHCompress = 2;
    private int SCHCompress = 2;
    private int MCHYRange = 8;
    private int SCHYRange = 120;


    private int MaxVoltageMch = 3000;
    private int MaxVoltageSch = 3000;
    #endregion

    #region внутренние данные
    TFltDataPacket DataPacket; //последний пришедший пакет
    Queue<TFltDataPacket> DataPacketHistory; // история данных
    Queue<CPack> PacksHistory; // история пачек
    Queue<TAbsStimIndex> FoundStimData; // история найденных стимулов
    Queue<TAbsStimIndex> ExpStimData; // история ожидаемых стимулов

    object DataPacketLock = new object(); // блокировка данных
    object CurrentTimeCync = new object(); // блокировка времени для синхронизации отрисовки
    object UpdateGraphDataLock = new object();

    GameTime OldUpdateTimeMch;
    int timeElapsed = 0;
    int millisecondsBeetwinUpdate = 500;

    public TTime summary_time_stamp = 0;
    public CFiltering m_salpaFilter;
    GraphicsDeviceManager graphics;
    // эффект BasicEffect для кривой  
    BasicEffect basicEffect;
    // спрайт для текстуры
    private SpriteBatch TextSprite;
    private SpriteFont mainFont;
    private Vector2 TextPosition;
    // массив массивов массивов сверток нашей кривой для многоканального режима
    private VertexPositionColor[][][] vertices;
    // массив массивов вершин для режима одного канала со сверткой
    private VertexPositionColor[][] schCompVerices;
    // массив массивов вершин для режима одного канала без свертки
    private VertexPositionColor[] schUncompVerices;
    private object VertexPositionLock = new object();
    private DrawMode SelectedDrawMode;
    private int SingleChannelNum = MEA.NAME2IDX[21];
    private int ZeroChannelNum = MEA.NAME2IDX[53];
    private TTime HistoryTimeLength = 0;
    private volatile bool IsDataUpdated;
    private volatile int MaxPrepeareDataTime = 0;

    #endregion

    public CDataRenderNew(CFiltering salpaFilter)
    {
      graphics = new GraphicsDeviceManager(this);

      graphics.PreferredBackBufferWidth = DEFAULT_WINDOW_WIDTH; // ширина приложения
      graphics.PreferredBackBufferHeight = DEFAULT_WINDOW_HEIGHT; // высота приложения
      graphics.IsFullScreen = false; // флаг полноэкранного приложения
      graphics.ApplyChanges(); // применяем параметры
      this.SelectedDrawMode = CDataRenderNew.DrawMode.DrawMultiChannel;
      (System.Windows.Forms.Control.FromHandle(this.Window.Handle)).DoubleClick += ChangeDrawMode;


      IsDataUpdated = false;

      DataPacketHistory = new Queue<TFltDataPacket>(HistoryLength);
      FoundStimData = new Queue<TAbsStimIndex>();
      ExpStimData = new Queue<TAbsStimIndex>();
      PacksHistory = new Queue<CPack>();

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

      OldUpdateTimeMch = new GameTime();
      base.Initialize();
    }
    /*
    protected override void Dispose()
    {
      m_salpaFilter.RemoveDataConsumer(this.RecieveFltData);
      m_salpaFilter.RemoveStimulConsumer(this.RecieveStimData);
      base.Dispose();
    }
    */
    protected override void LoadContent()
    {
      // Создали новый SpriteBatch, который может быть использован для прорисовки текстур.
      TextSprite = new SpriteBatch(GraphicsDevice);
      Content.RootDirectory = @"Content\";
      mainFont = Content.Load<SpriteFont>("MainFont");

    }

    protected override void UnloadContent()
    {

    }

    protected override void Update(GameTime gameTime)
    {
      lock (DataPacketLock)
      {
        HistoryTimeLength = 0;
        for (int i = 0; i < DataPacketHistory.Count; i++)
        {
          HistoryTimeLength += (TTime)DataPacketHistory.ElementAt(i)[SingleChannelNum].Length;
        }
      }
      timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
      if (gameTime.ElapsedGameTime.Milliseconds > MaxPrepeareDataTime)
        MaxPrepeareDataTime = gameTime.ElapsedGameTime.Milliseconds;


      lock (DataPacketLock)
      {
        // удалим устаревшие найденные стимулы
        while (FoundStimData.Count > 0 && FoundStimData.Peek() + HistoryTimeLength < summary_time_stamp)
        {
          FoundStimData.Dequeue();
        }
      }


      #region Обработка клавиш
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
            MaxVoltageMch += (MaxVoltageMch > 50) ? 25 : 10;
            break;
          case DrawMode.DrawSingleChannel:
            MaxVoltageSch += (MaxVoltageSch > 50) ? 250 : 100;
            break;
        }
      }
      if (Keyboard.GetState().IsKeyDown(Keys.Down) && editFlagDown)
      {
        editFlagDown = false;
        switch (this.SelectedDrawMode)
        {
          case DrawMode.DrawMultiChannel:
            MaxVoltageMch -= (MaxVoltageMch > 100) ? 50 : (MaxVoltageMch > 20) ? 20 : 0;
            break;
          case DrawMode.DrawSingleChannel:
            MaxVoltageSch -= (MaxVoltageSch > 100) ? 500 : (MaxVoltageSch > 20) ? 20 : 0;
            break;
        }
      }
      if (Keyboard.GetState().IsKeyUp(Keys.Up)) editFlagUp = true;

      if (Keyboard.GetState().IsKeyUp(Keys.Down)) editFlagDown = true;
      #endregion
      #endregion

      #region Установка оптимальной частоты пересчета данных
      millisecondsBeetwinUpdate = 200;
      if (HistoryLength < 100)
        millisecondsBeetwinUpdate = 100;
      if (HistoryLength < 50)
        millisecondsBeetwinUpdate = 50;
      if (HistoryLength < 30)
        millisecondsBeetwinUpdate = 20;
      if (HistoryLength < 20)
        millisecondsBeetwinUpdate = 10;
      #endregion

      switch (SelectedDrawMode)
      {
        case DrawMode.DrawMultiChannel:
          // случай отрисовки всех каналов, подготовливаем массив точек
          if (/*timeElapsed > millisecondsBeetwinUpdate/2 && */!IsDataUpdated && DataPacket != null)
          // надо заменить на обработку GameTime [но пока сойдет и так] - для фиксации фпс
          {
            timeElapsed = 0;
            lock (DataPacketLock)
            {
              lock (CurrentTimeCync)
              {
                summary_time_stamp = m_salpaFilter.TimeStamp;
                if (DataPacket != null) summary_time_stamp += (TTime)DataPacket[DataPacket.Keys.FirstOrDefault()].Length;

              }
              lock (UpdateGraphDataLock)
              {
                // Здесь мы пересчитываем данные исходя из текущего состояния очереди, для чего её лочим 
                // создадим массив массивовх
                vertices = new VertexPositionColor[DataPacket.Keys.Count][][];
                //Получаем размеры окна;
                int WindowHeight = graphics.PreferredBackBufferHeight;
                //плотность точек
                double PointsPerPX = 1;
                //размеры
                int WindowWidth = graphics.PreferredBackBufferWidth;
                int FormWidth = (System.Windows.Forms.Control.FromHandle(this.Window.Handle)).Width - 16; // 16 поправка на  обрамление окна
                int FormHeight = (System.Windows.Forms.Control.FromHandle(this.Window.Handle)).Height - 39;
                // сетка 8 x 8 клеток
                // длина и ширина пропорциональны размеру главного окна
                int CellWidth = WindowWidth / 8;
                int CellHeight = WindowHeight / 8;
                int WindowCellWidth = FormWidth / 8;
                int WindowCellHeght = FormHeight / 8;
                int pxCount = FormWidth / 8;
                //количество точек в очереди
                int length = 0;
                #region Вычисление векторов смещения для каналов

                #endregion
                for (int i = 0; i < DataPacketHistory.Count; i++)
                {
                  length += DataPacketHistory.ElementAt(i)[DataPacket.Keys.First()].Length;
                }

                PointsPerPX = ((double)length) / CellWidth;
                // key - номер канала
                foreach (int key in DataPacket.Keys)
                {

                  vertices[key] = new VertexPositionColor[pxCount][];
                  double[] GlueArray = new double[length];

                  // текущая позиция в массиве точек 
                  int currentlength = 0;
                  int PackCount = DataPacketHistory.Count;
                  int CurrentPacketLength = 0;
                  long PrepeareDataTime = Environment.TickCount;
                  for (int PackNum = 0; PackNum < DataPacketHistory.Count; PackNum++)
                  {
                    CurrentPacketLength = DataPacketHistory.ElementAt(PackNum)[key].Length;
                    Array.Copy(DataPacketHistory.ElementAt(PackNum)[key], 0, GlueArray, currentlength, CurrentPacketLength);
                    currentlength += CurrentPacketLength;
                  }

                  PointsPerPX = length / pxCount; // точек в одном пикселе
                  for (int i = pxCount - 1; i > 0; i--)
                  {
                    float max = float.MinValue;
                    float min = float.MaxValue;
                    for (int j = (int)(i * PointsPerPX); (i + 1) * PointsPerPX + 1 < length && j < (i + 1) * PointsPerPX + 1; j++)
                    {
                      float t = (float)GlueArray[j];
                      if (t > max) max = t;
                      if (t < min) min = t;
                    }
                    VertexPositionColor[] line = new VertexPositionColor[2];
                    if (IsPackAtTime(length - (i) * (float)PointsPerPX + 2500))
                    {
                      line[0].Color = Color.Red;
                      line[1].Color = Color.Red;
                    }
                    else
                    {
                      line[0].Color = Color.Green;
                      line[1].Color = Color.Green;
                    }

                    line[0].Position.X = (i) * (float)CellWidth / WindowCellWidth;
                    line[0].Position.Y = max * WindowCellHeght / MaxVoltageMch;
                    line[1].Position.X = line[0].Position.X;
                    line[1].Position.Y = min * WindowCellHeght / MaxVoltageMch;
                    if (line[0].Position.Y > WindowCellHeght / 2) line[0].Position.Y = WindowCellHeght / 2;
                    if (line[0].Position.Y < -WindowCellHeght / 2) line[0].Position.Y = -WindowCellHeght / 2;
                    if (line[1].Position.Y < -WindowCellHeght / 2) line[1].Position.Y = -WindowCellHeght / 2;
                    if (line[1].Position.Y > WindowCellHeght / 2) line[1].Position.Y = WindowCellHeght / 2;

                    vertices[key][i] = line;

                  }
                  PrepeareDataTime = Environment.TickCount - PrepeareDataTime;
                  /*
                  if (ticks > 120)
                  {
                    pxCount /= 2; // оптимизация при задержках
                  }
                  */
                  #region Стрый, монструозный способ отрисовки
                  /*
                for (int PuckNum = 0; PuckNum < DataPacketHistory.Count; PuckNum++)
                {
                  for (int j = (int)PointsPerPX; j < DataPacketHistory.ElementAt(PuckNum)[key].Length; j += (int)PointsPerPX)
                  {
                    // находим минимум и максимум
                    float max = float.MinValue;
                    float min = float.MaxValue;
                    for (int z = 0; z < (int)PointsPerPX; z++)
                    {
                      float t = (float)DataPacketHistory.ElementAt(PuckNum)[key][j - z];
                      if (t > max) max = t;
                      if (t < min) min = t;
                    }
                    VertexPositionColor[] line = new VertexPositionColor[2];
                    line[0].Position = new Vector3(0, 0, 0);
                    line[1].Position = new Vector3(0, 0, 0);
                    line[0].Position.X = ((float)(PuckNum) * CellWidth) / length;
                    line[0].Position.Y = max * MCHYRange / 200 + CellHeight / 2;
                    line[1].Position.X = line[0].Position.X;
                    line[1].Position.Y = min * MCHYRange / 200 + CellHeight / 2;
                    if (line[0].Position.Y < 0)
                      line[0].Position.Y = 0;
                    if (line[0].Position.Y > CellHeight)
                      line[0].Position.Y = CellHeight;

                    if (line[1].Position.Y > CellHeight)
                      line[1].Position.Y = CellHeight;
                    if (line[1].Position.Y < 0)
                      line[1].Position.Y = 0;

                    line[0].Position += ChannelvectorsArray[key];
                    line[1].Position += ChannelvectorsArray[key];
                    line[0].Color = Color.DarkGreen;
                    line[1].Color = line[0].Color;

                    if (j > PointsPerPX)
                    {

                    }

                  }
                  currentlength += (int)(DataPacketHistory.ElementAt(PuckNum)[key].Length * 2 / PointsPerPX);
                }
                 */
                  #endregion


                }
              }
            }
          }
          break;
        case DrawMode.DrawSingleChannel:

          lock (CurrentTimeCync)
          {
            summary_time_stamp = m_salpaFilter.TimeStamp;
          }

          break;
      }

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


      }
      IsDataUpdated = false;
    }

    public void RecievePackData(CPack pack)
    {
      //pack.Length = 2000;//DEBUG что бы пачки имели длину (ненулевую).  
      lock (DataPacketLock)
      {
        PacksHistory.Enqueue(pack);
        while (PacksHistory.Count > 0 && PacksHistory.Peek().Start + (uint)PacksHistory.Peek().Length + HistoryTimeLength < summary_time_stamp)
        {
          PacksHistory.Dequeue();
        }
      }
    }

    public void RecieveStimData(List<TAbsStimIndex> stims)
    {
      lock (DataPacketLock)
      {
        //добавим в очередь пришедшие
        foreach (TAbsStimIndex stim in stims)
        {
          FoundStimData.Enqueue(stim);
        }
        // удалим устаревшие найденные стимулы
        while (FoundStimData.Count > 0 && FoundStimData.Peek() + HistoryTimeLength < summary_time_stamp)
        {
          FoundStimData.Dequeue();
        }
      }
    }

    public void SetExpStimData(TStimGroup stim) { }

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
          SCHYRange = MCHYRange;
          lock (DataPacketLock)
          {
            if (MEA.NAME2IDX[SingleChannelNum] >= 0)
            {
              SingleChannelNum = MEA.NAME2IDX[SingleChannelNum];
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
      lock (CurrentTimeCync)
      {
        double PointsPerPX;

        //Получаем размеры окна;
        int WindowHeight = graphics.PreferredBackBufferHeight;
        //graphics.PreferredBackBufferFormat = SurfaceFormat.Color;

        int WindowWidth = graphics.PreferredBackBufferWidth;
        int FormWidth = (System.Windows.Forms.Control.FromHandle(this.Window.Handle)).Width;
        int FormHeight = (System.Windows.Forms.Control.FromHandle(this.Window.Handle)).Height;

        TFltData[] data_to_display;
        switch (this.SelectedDrawMode)
        {
          case DrawMode.DrawMultiChannel:
            #region Отрисовка всех каналов

            GraphicsDevice.Clear(Color.CornflowerBlue);
            basicEffect.CurrentTechnique.Passes[0].Apply();

            //создание массива векторов для каналов
            Vector3[] ChannelvectorsArray = new Vector3[DataPacket.Keys.Max() + 1];

            // сетка 8 x 8 клеток
            // длина и ширина пропорциональны размеру главного окна
            int CellWidth = WindowWidth / 8;
            int CellHeight = WindowHeight / 8;

            #region Вычисление векторов смещения для каналов
            foreach (int i in DataPacket.Keys)
            {
              ChannelvectorsArray[i] = new Vector3((MEA.IDX2NAME[i] / 10 - 1) * CellWidth, (MEA.IDX2NAME[i] % 10 - 1) * CellHeight, 0);

            }
            #endregion


            foreach (int key in DataPacket.Keys)
            {
              //подготовка массива массивов точек
              IsDataUpdated = false;
              #region Старая, монструозная отрисовка
              /*
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

                  data_to_display = new TFltData[length];

                  int currentlength = 0;
                  #region Склейка истории
                  for (int i = 0; i < DataPacketHistory.Count; i++)
                  {
                    for (int j = 0; j < DataPacketHistory.ElementAt(i)[key].Length; j += 1)
                    {
                      data_to_display[currentlength + j] = DataPacketHistory.ElementAt(i)[key][j];
                    }
                    currentlength += DataPacketHistory.ElementAt(i)[key].Length;
                  }
                  #endregion
                  PointsPerPX = ((double)data_to_display.Length) * 8 / FormWidth;

                }
                for (int i = (int)PointsPerPX; i < length; i += (int)PointsPerPX)
                {
                  #region Подготовка и отрисовка вертикальных линий
                  float max = float.MinValue;
                  float min = float.MaxValue;
                  for (int z = 0; z < (int)PointsPerPX; z++)
                  {
                    float t = (float)data_to_display[i - z];
                    if (t > max) max = t;
                    if (t < min) min = t;
                  }

                  VertexPositionColor[] line = new VertexPositionColor[2];
                  line[0].Position = new Vector3(0, 0, 0);
                  line[1].Position = new Vector3(0, 0, 0);
                  line[0].Position.X = ((float)(i) * CellWidth) / length;
                  line[0].Position.Y = max * MCHYRange / 200 + CellHeight / 2;
                  line[1].Position.X = line[0].Position.X;
                  line[1].Position.Y = min * MCHYRange / 200 + CellHeight / 2;
                  if (line[0].Position.Y < 0)
                    line[0].Position.Y = 0;
                  if (line[0].Position.Y > CellHeight)
                    line[0].Position.Y = CellHeight;

                  if (line[1].Position.Y > CellHeight)
                    line[1].Position.Y = CellHeight;
                  if (line[1].Position.Y < 0)
                    line[1].Position.Y = 0;

                  line[0].Position += ChannelvectorsArray[RealChannelIndx];
                  line[1].Position += ChannelvectorsArray[RealChannelIndx];
                  line[0].Color = Color.DarkGreen;
                  line[1].Color = line[0].Color;
                  if (i == 5000)
                  {
                    //break point
                  }
                  graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, line, 0, 1);
                  for (int z = 0; z < SCHCompress && -z + i + SCHCompress < length; z++)
                  {
                    float t = (float)data_to_display[i + SCHCompress - z];
                    if (t > max) max = t;
                    if (t < min) min = t;
                  }
                  line[1].Position.X += ((float)CellWidth / (float)length);

                  line[1].Position.Y = max * MCHYRange / 200 + ChannelvectorsArray[RealChannelIndx].Y + CellHeight / 2;
                  line[0].Position.Y = min * MCHYRange / 200 + ChannelvectorsArray[RealChannelIndx].Y + CellHeight / 2;

                  //случай сплошного(длинного) нуля - горизонтальной прямой
                  if (Math.Abs(line[1].Position.Y - line[1].Position.Y) < 5)
                  {
                    line[1].Position.X += ((float)WindowWidth / (float)length);
                  }
                  else
                  {

                    if (line[1].Position.Y > CellHeight / 2 + ChannelvectorsArray[RealChannelIndx].Y)
                      line[1].Position.Y = CellHeight / 2 + ChannelvectorsArray[RealChannelIndx].Y;
                    if (line[1].Position.Y < ChannelvectorsArray[RealChannelIndx].Y)
                      line[1].Position.Y = ChannelvectorsArray[RealChannelIndx].Y;

                    if (line[0].Position.Y > CellHeight / 2 + ChannelvectorsArray[RealChannelIndx].Y)
                      line[0].Position.Y = CellHeight / 2 + ChannelvectorsArray[RealChannelIndx].Y;
                    if (line[0].Position.Y < ChannelvectorsArray[RealChannelIndx].Y)
                      line[0].Position.Y = ChannelvectorsArray[RealChannelIndx].Y;
                  }
                  graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, line, 0, 1);
                 
                  #endregion
                }
              }
                 
                #endregion
              */
              #endregion

              lock (UpdateGraphDataLock)
              {
                if (!(vertices != null && vertices.Count() == DataPacket.Keys.Count && vertices[key] != null && vertices[key].Length > 0))
                {
                  break;
                }
                for (int i = 0; i < vertices[key].Length; i++)
                {
                  if (vertices[key][i] == null) continue;
                  VertexPositionColor[] line = new VertexPositionColor[2];
                  line[0].Position = vertices[key][i][0].Position + ChannelvectorsArray[key];
                  line[0].Position.Y += CellHeight / 2;
                  line[1].Position = vertices[key][i][1].Position + ChannelvectorsArray[key];
                  line[1].Position.Y += CellHeight / 2;
                  line[0].Color = vertices[key][i][0].Color;
                  line[1].Color = vertices[key][i][1].Color;
                  graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, line, 0, 1);
                }
                #region Отрисовка стимулов
                lock (DataPacketLock)
                {
                  foreach (TAbsStimIndex stim in FoundStimData)
                  {
                    VertexPositionColor[] stimline = new VertexPositionColor[2];
                    stimline[0].Position.X = CellWidth * (1 - (float)(summary_time_stamp - stim - (TTime)DataPacket[DataPacket.Keys.First()].Length) / HistoryTimeLength);
                    stimline[0].Position.Y = CellHeight / 2 - 20;
                    stimline[0].Position.Z = 0;// (float)0.1;
                    stimline[0].Position += ChannelvectorsArray[key];
                    stimline[0].Color = Color.White;
                    stimline[1].Position = stimline[0].Position;
                    stimline[1].Position.Y += CellHeight / 2;
                    stimline[1].Color = Color.Black;

                    graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, stimline, 0, 1);
                   
                  }
                }
                #endregion
              }
            }
            IsDataUpdated = true;

            #region отрисовка сетки каналов
            VertexPositionColor[] HorizontalLinesPoints = new VertexPositionColor[2];
            VertexPositionColor[] VericalLinesPoints = new VertexPositionColor[2];

            for (int i = 1; i < 8; i++)
            {
              HorizontalLinesPoints[0].Position = new Vector3(0, i * CellHeight, 0);
              HorizontalLinesPoints[0].Color = Color.Black;
              HorizontalLinesPoints[1].Position = new Vector3(WindowWidth, i * CellHeight, 0);
              HorizontalLinesPoints[1].Color = Color.Black;

              VericalLinesPoints[0].Position = new Vector3(i * CellWidth, 0, 0);
              VericalLinesPoints[0].Color = Color.Black;
              VericalLinesPoints[1].Position = new Vector3(i * CellWidth, WindowHeight, 0);
              VericalLinesPoints[1].Color = Color.Black;

              graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, HorizontalLinesPoints, 0, 1);
              graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, VericalLinesPoints, 0, 1);
            }
            #endregion


            #region Отрисовка надписей
            // Текущее время
            string CurrentTimeMCH = "Current Time " + ((double)m_salpaFilter.TimeStamp / 25000).ToString() + " seconds";
            string QueueTimeLength = "Window Length " + ((double)HistoryTimeLength / 25000).ToString() + " seconds";
            string UpdateTime = "Update Time " + ((double)MaxPrepeareDataTime / 1000).ToString() + " seconds";
            //(System.Windows.Forms.Control.FromHandle(this.Window.Handle)).
            TextSprite.Begin();

            TextPosition = new Vector2(20, 40);
            //Выводим строку
            TextSprite.DrawString(mainFont, CurrentTimeMCH, new Vector2(0, 20), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);
            TextSprite.DrawString(mainFont, QueueTimeLength, new Vector2(0, 40), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);
            TextSprite.DrawString(mainFont, UpdateTime, new Vector2(0, 60), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);
            float kX, kY; // соотношение между шириной окна и буффера

            //39, 16  - разница в размерах между окном и буффером 

            kX = (float)(FormWidth - 16) / (WindowWidth + 0);
            kY = (float)(FormHeight - 39) / (WindowHeight + 0);
            foreach (int key in DataPacket.Keys)
            {
              Vector2 shift = new Vector2((ChannelvectorsArray[key].X - 1) * kX, ChannelvectorsArray[key].Y * kY);
              //Vector2 shift = new Vector2(ChannelvectorsArray[key].X, ChannelvectorsArray[key].Y);
              TextSprite.DrawString(mainFont, "Ch# " + MEA.IDX2NAME[key].ToString(), shift, Color.Red, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);
            }
            TextSprite.End();
            #endregion
            break;
            #endregion
          case DrawMode.DrawSingleChannel:
            #region Отрисовка одного канала
            GraphicsDevice.Clear(Color.White);
            basicEffect.CurrentTechnique.Passes[0].Apply();
            IsDataUpdated = false;

            lock (DataPacketLock)
            {
              #region Заполнение массива точек из истории
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
              #endregion
            }
            PointsPerPX = ((double)data_to_display.Length) / FormWidth;

            SCHCompress = (PointsPerPX < 2) ? 2 : (int)PointsPerPX;
            if (PointsPerPX <= 2)
            {
              #region сплошная отрисовка
              //отрисовка без наложения линий - готовим сплошной массив и его рисуем
              schUncompVerices = new VertexPositionColor[data_to_display.Length];
              for (int i = 0; i < data_to_display.Length; i++)
              {
                schUncompVerices[i].Position.X = ((float)i * WindowWidth) / data_to_display.Length;
                schUncompVerices[i].Position.Y = WindowHeight / 2 + (float)data_to_display[i] * WindowHeight  / MaxVoltageSch;
                if (schUncompVerices[i].Position.Y < 0) schUncompVerices[i].Position.Y = 0;
                if (schUncompVerices[i].Position.Y > WindowHeight) schUncompVerices[i].Position.Y = WindowHeight;
                schUncompVerices[i].Position.Z = 0;
                schUncompVerices[i].Color = (Math.Abs(data_to_display[i]) > 120) ? Color.Red : Color.Blue;
                if (IsPackAtTime(i))
                  schUncompVerices[i].Color = Color.Red;

              }
              graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, schUncompVerices, 0, schUncompVerices.Count() - 1);
              #endregion
            }
            else
            {
              #region отрисовка вертикальными линиями
              // отрисовка с наложением линий друг на друга - рисуем вертикальные линии от максимума до минимума
              // NOTE: наиболее частый случай
              int length = data_to_display.Length;
              for (int i = SCHCompress; i < length; i += SCHCompress)
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
                line[0].Position.Y = max * WindowHeight / MaxVoltageSch + WindowHeight / 2;
                line[0].Position.Z = 0;
                line[1].Position.X = line[0].Position.X;
                line[1].Position.Y = min * WindowHeight / MaxVoltageSch + WindowHeight / 2;
                line[1].Position.Z = 0;
                line[0].Color = Color.DarkGray;
                line[1].Color = Color.DarkGray;
                if (IsPackAtTime(length - i))
                {
                  line[0].Color = Color.Red;
                  line[1].Color = Color.Red;
                }
                if (line[0].Position.Y < 0) line[0].Position.Y = 0;
                if (line[1].Position.Y > WindowHeight) line[1].Position.Y = WindowHeight;
                //случай сплошного(длинного) нуля - горизонтальной прямой
                if (line[1].Position.Equals(line[0].Position))
                  line[1].Position.X += ((float)WindowWidth / (float)length);

                graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, line, 0, 1);
                //отрисовка соединяющей линии

                for (int z = 0; z < SCHCompress && -z + i + SCHCompress < length; z++)
                {
                  float t = (float)data_to_display[i + SCHCompress - z];
                  if (t > max) max = t;
                  if (t < min) min = t;
                }
                line[1].Position.X += ((float)WindowWidth / (float)length);
                line[1].Position.Y = (max + min) * SCHYRange / 200 + WindowHeight / 2;
                line[1].Position.Z = 0;
                if (line[1].Position.Y > WindowHeight) line[1].Position.Y = WindowHeight;
                //случай сплошного(длинного) нуля - горизонтальной прямой
                if (line[1].Position.Equals(line[0].Position))
                  line[1].Position.X += ((float)WindowWidth / (float)length);

                graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, line, 0, 1);
              }
              #endregion
            }
            #region Отрисовка стимулов
            lock (DataPacketLock)
            {
              foreach (TAbsStimIndex stim in FoundStimData)
              {
                VertexPositionColor[] stimline = new VertexPositionColor[2];
                stimline[0].Position.X = WindowWidth * (1 - (float)(summary_time_stamp - stim) / HistoryTimeLength);
                stimline[0].Position.Y = WindowHeight / 2 - 70;
                stimline[0].Position.Z = 0;
                stimline[0].Color = Color.Red;
                stimline[1].Position = stimline[0].Position;
                stimline[1].Position.Y = WindowHeight / 2;
                stimline[1].Color = Color.Blue;
                graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, stimline, 0, 1);
                stimline[0].Position.X += 1;
                stimline[1].Position.X += 1;
                graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, stimline, 0, 1);
              }
            }
            #endregion

            #region Отрисовка надписей
            // Текущее время
            CurrentTimeMCH = "Current Time " + ((double)m_salpaFilter.TimeStamp / 25000).ToString() + " seconds";
            QueueTimeLength = "Window Length " + ((double)HistoryTimeLength / 25000).ToString() + " seconds";
            UpdateTime = "Update Time " + ((double)MaxPrepeareDataTime / 1000).ToString() + " seconds";
            //(System.Windows.Forms.Control.FromHandle(this.Window.Handle)).
            TextSprite.Begin();

            TextPosition = new Vector2(20, 40);
            //Выводим строку
            TextSprite.DrawString(mainFont, CurrentTimeMCH, new Vector2(0, 12), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);
            TextSprite.DrawString(mainFont, QueueTimeLength, new Vector2(0, 24), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);
            TextSprite.DrawString(mainFont, "Ch# " + MEA.IDX2NAME[SingleChannelNum].ToString(), new Vector2(20, 36), Color.Red, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);

            TextSprite.End();

            //gr.DrawString(CurrentTime, (System.Windows.Forms.Control.FromHandle(this.Window.Handle)).Font, new windraw.SolidBrush(windraw.Color.Green), new windraw.Point(10, 10));

            #endregion
            break;

            #endregion
        }
      }
      base.Draw(gameTime);
    }

    private bool IsPackAtTime(float time)
    {
      lock (DataPacketLock)
      {
        foreach (CPack pack in PacksHistory)
        {
          if (pack.Start + (TTime)time < summary_time_stamp &&
            pack.Start + (TTime)pack.Length + (TTime)time > summary_time_stamp)
          {
            return true;
          }
        }
      }
      return false;
    }

    private enum DrawMode
    {
      DrawSingleChannel,
      DrawMultiChannel
    }
  }
}


