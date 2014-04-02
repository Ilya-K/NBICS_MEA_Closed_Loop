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
    #endregion
    #region внутренние данные
    TFltDataPacket DataPacket; //Данные для отрисовки фильтрованных данных
    List<TFltDataPacket> DataPacketHistory; // история данных
    object DataPacketLock = new object(); // блокировка данных
    TTime summary_time_stamp = 0;

    GraphicsDeviceManager graphics;
    // эффект BasicEffect для кривой
    BasicEffect basicEffect;
    // массив  вершин нашей кривой
    VertexPositionColor[] vertices;
    #endregion

    public CDataRender()
    {
      graphics = new GraphicsDeviceManager(this);

      graphics.PreferredBackBufferWidth = DEFAULT_WINDOW_WIDTH; // ширина приложения
      graphics.PreferredBackBufferHeight = DEFAULT_WINDOW_HEIGHT; // высота приложения
      graphics.IsFullScreen = false; // флаг полноэкранного приложения
      graphics.ApplyChanges(); // применяем параметры


      DataPacketHistory = new List<TFltDataPacket>();
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
        DataPacketHistory.Add(DataPacket);
        int AnyExistsKey = DataPacket.Keys.First();
        summary_time_stamp += (TTime)DataPacket[AnyExistsKey].Length;
      }

    }
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);
      TFltData[] data_to_display;
      lock (DataPacketLock)
      {
        int DebugChannelNum = 13;
        data_to_display = DataPacket[DebugChannelNum];
      }
      vertices = new VertexPositionColor[data_to_display.Length];
      //подготовка массива точек

      for (int i = 0; i < data_to_display.Length; i++)
      {
        vertices[i].Position.X = ((float)i * DEFAULT_WINDOW_WIDTH) / data_to_display.Length;
        vertices[i].Position.Y = DEFAULT_WINDOW_HEIGHT / 2 - (float)data_to_display[i];
        vertices[i].Position.Z = 0;
      }

      basicEffect.CurrentTechnique.Passes[0].Apply();
      graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices, 0, vertices.Count() - 2);

      base.Draw(gameTime);
    }
  }
}


