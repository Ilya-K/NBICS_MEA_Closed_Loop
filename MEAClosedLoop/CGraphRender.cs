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
  #endregion
  public class CGraphRender : Microsoft.Xna.Framework.Game
  {
    GraphicsDeviceManager graphics;
    public TRawData[] DataPacket;
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
    public CGraphRender()
    {
      graphics = new GraphicsDeviceManager(this);

      graphics.PreferredBackBufferWidth = 2200; // ширина приложения
      graphics.PreferredBackBufferHeight = 1200; // высота приложения
      graphics.IsFullScreen = false; // флаг полноэкранного приложения
      ;
      graphics.ApplyChanges(); // применяем параметры


    }
    protected override void Initialize()
    {
      Content.RootDirectory = "Content";
      (System.Windows.Forms.Control.FromHandle(this.Window.Handle)).Location = new System.Drawing.Point(0, 0);
      basicEffect = new BasicEffect(graphics.GraphicsDevice);
      basicEffect.VertexColorEnabled = true;
      basicEffect.Projection = Matrix.CreateOrthographicOffCenter
         (0, graphics.GraphicsDevice.Viewport.Width,     // left, right
          graphics.GraphicsDevice.Viewport.Height, 0,    // bottom, top
          0, 1);                                         // near, far plane

      if (detector.inner_data_to_display != null)
      {
        float x_range = (float)graphics.PreferredBackBufferWidth / detector.inner_data_to_display.Count();
        x_range /= (arraylengh <= 2501) ? 2 : 1;
        vertices = new VertexPositionColor[detector.inner_data_to_display.Length];
        for (int i = 0; i < detector.inner_data_to_display.Length; i++)
        {
          vertices[i].Position = new Vector3(i * x_range, (detector.inner_data_to_display[i] - 32768) / 8 + 500, 0);
          vertices[i].Color = Color.Black;
        }
        arraylengh = detector.inner_data_to_display.Length - 1;
      }
      // TODO: Add your initialization logic here
      if (detector.inner_found_indexes_to_display != null)
      {
        float x_range = (float)graphics.PreferredBackBufferWidth / detector.inner_data_to_display.Count();
        x_range /= (arraylengh <= 2501) ? 2 : 1;
        stimcoords = new VertexPositionColor[detector.inner_found_indexes_to_display.Count()][];
        for (int i = 0; i < detector.inner_found_indexes_to_display.Count(); i++)
        {
          stimcoords[i] = new VertexPositionColor[2];

          stimcoords[i][0].Position = new Vector3(detector.inner_found_indexes_to_display[i] * x_range, 0, 0);
          stimcoords[i][0].Color = Color.Red;
          stimcoords[i][1].Position = new Vector3(detector.inner_found_indexes_to_display[i] * x_range, 900, 0);
          stimcoords[i][1].Color = Color.Red;
        }
      }
      if (detector.inner_expectedStims_to_display != null && detector.inner_data_to_display != null)
      {
        float x_range = (float)graphics.PreferredBackBufferWidth / detector.inner_data_to_display.Count();
        x_range /= (arraylengh <= 2501) ? 2 : 1;
        expstimcoords = new VertexPositionColor[detector.inner_expectedStims_to_display.Count()][];

        for (int i = 0; i < detector.inner_expectedStims_to_display.Count(); i++)
        {
          expstimcoords[i] = new VertexPositionColor[2];
          expstimcoords[i][0].Position = new Vector3(detector.inner_expectedStims_to_display[i].stimTime * x_range - 1, 100, 0);
          expstimcoords[i][0].Color = Color.Green;
          expstimcoords[i][1].Position = new Vector3(detector.inner_expectedStims_to_display[i].stimTime * x_range + 1, 800, 0);
          expstimcoords[i][1].Color = Color.Green;
        }
      }

      base.Initialize();
    }

    protected override void LoadContent()
    {
      // TODO: use this.Content to load your game content here
      // создать массив-контейнер для хранения трёх вершин

    }

    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
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
      this.Initialize();
      GraphicsDevice.Clear(Color.CornflowerBlue);
      if (detector.inner_data_to_display != null)
      {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        basicEffect.CurrentTechnique.Passes[0].Apply();
        try
        {

          graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices, 0, arraylengh);
          for (int i = 0; i < stimcoords.Count(); i++)
          {
            graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, stimcoords[i], 0, 1);
          }
          for (int i = 0; i < expstimcoords.Count(); i++)
          {
            graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, expstimcoords[i], 0, 1);
          }
        }
        catch (ArgumentNullException ex)
        {
          //System.Windows.Forms.MessageBox.Show(ex.Message);
        }
        base.Draw(gameTime);
      }
    }
  }
}


