using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _pixel;

    private WorldMap _worldMap;
    private const int TileSize = 16;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _worldMap = new WorldMap(80,45);

        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData([Color.White]);

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        for (int y = 0; y < _worldMap.Height; y++)
        {
            for (int x = 0; x < _worldMap.Width; x++)
            {
                byte height = _worldMap.HeightMap[x, y];

                Color tileColor = height switch
                {
                    < 50  => new Color( 30,  60, 100), // 深海 (青)
                    < 100 => new Color( 30, 100, 200), // 海（青）
                    < 110 => new Color(220, 210, 140), // 砂浜・浅瀬（黄土色）
                    < 150 => new Color( 60, 160,  60), // 平地・草原（緑）
                    < 200 => new Color(130, 130, 130), // 山（灰色）
                    _     => new Color(240, 240, 240)  // 雪山（白）
                };

                Vector2 position = new Vector2(x * TileSize, y * TileSize);
                _spriteBatch.Draw(_pixel, position, null, tileColor, 0f, Vector2.Zero, TileSize, SpriteEffects.None, 0f);
            }
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
