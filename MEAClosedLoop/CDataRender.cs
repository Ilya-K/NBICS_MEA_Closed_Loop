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
  #endregion
  public class CDataRender : Microsoft.Xna.Framework.Game
  {
    #region стандартные значения
    private int DEFAULT_WINDOW_HEIGHT = 600;
    private int DEFAULT_WINDOW_WIDTH = 800;
    #endregion
    #region внутренние данные
    TFltDataPacket DataPacket; //Данные для отрисовки фильтрованных данных
    List<TFltDataPacket> DataPacketHistory; // история данных
    object DataPocketLock = new object(); // блокировка данных
    #endregion
    GraphicsDeviceManager graphics;
    //public TRawData[] DataPacket;
    private CStimDetectShift detector;
    // эффект BasicEffect для кривой
    BasicEffect basicEffect;
    // массив  вершин нашей кривой
    VertexPositionColor[] vertices;
    int arraylengh = 0;

    // массив массивов вершин для найденных стимулов
    VertexPositionColor[][] stimcoords;
    // массив массивов вершин для ожидаемых стимулов
    VertexPositionColor[][] expstimcoords;

    public void SetDataObj(CStimDetectShift obj)
    {
      detector = obj;
    }
    public CDataRender()
    {
      graphics = new GraphicsDeviceManager(this);

      graphics.PreferredBackBufferWidth = DEFAULT_WINDOW_WIDTH; // ширина приложения
      graphics.PreferredBackBufferHeight = DEFAULT_WINDOW_HEIGHT; // высота приложения
      graphics.IsFullScreen = false; // флаг полноэкранного приложения
      graphics.ApplyChanges(); // применяем параметры


    }
    protected override void Initialize()
    {
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

    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);
      base.Draw(gameTime);

    }
  }
}


