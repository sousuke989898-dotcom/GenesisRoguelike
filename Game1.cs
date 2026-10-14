using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _pixel;

    private Texture2D _playerTexture;

    private World.WorldMap _worldMap;
    private const int TileSize = 16;

    private const int MapSizeX = 160;
    private const int MapSizeY = 160;

    private Point PlayerPos = new(MapSizeX/2, MapSizeY/2);

    private float _moveTimer = 0f;
    private const float MoveDelay = 0.1f; // 移動間隔（秒）

    private Camera2D _camera = new(128f, 128f, 0.3f);


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _worldMap = new(MapSizeX,MapSizeX);

        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();

        Vector2 initialPlayerPixelPos = new(
        PlayerPos.X * TileSize + TileSize / 2f, 
        PlayerPos.Y * TileSize + TileSize / 2f
        );
        _camera.SnapTo(initialPlayerPixelPos);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData([Color.White]);

        _playerTexture = Content.Load<Texture2D>("Player");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _moveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        KeyboardState keyboardState = Keyboard.GetState();

        if (_moveTimer >= MoveDelay)
        {
            Point moveDir = Point.Zero;

            if (keyboardState.IsKeyDown(Keys.W)) moveDir = new Point(0, -1);
            else if (keyboardState.IsKeyDown(Keys.A)) moveDir = new Point(-1, 0);
            else if (keyboardState.IsKeyDown(Keys.S)) moveDir = new Point(0, 1);
            else if (keyboardState.IsKeyDown(Keys.D)) moveDir = new Point(1, 0);

            if (keyboardState.IsKeyDown(Keys.Space))//試運転用
            {
                _moveTimer = -1f;
            }

            if (moveDir != Point.Zero)
            {
                Point nextPos = PlayerPos + moveDir;
                if (_worldMap.CanMove(nextPos))
                {
                    // _worldMap.RunWater();
                    PlayerPos = nextPos;
                }
                _moveTimer = 0f;
            }
        }


        Vector2 playerPixelPos = new(PlayerPos.X * TileSize + TileSize / 2f, PlayerPos.Y * TileSize + TileSize / 2f);
        _camera.Update(playerPixelPos);


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        Matrix viewMatrix = _camera.GetViewMatrix(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        _spriteBatch.Begin(transformMatrix: viewMatrix);

        for (int y = 0; y < _worldMap.Height; y++)
        {
            for (int x = 0; x < _worldMap.Width; x++)
            {
                byte height = _worldMap.HeightMap[x, y];

                Color tileColor = height switch
                {

                    < 50  => new Color( 60,  60,  60), // 深海(黒)
                    < 90  => new Color(100, 100, 100), // 海(灰色)
                    < 110 => new Color(220, 210, 140), // 砂浜・浅瀬（黄土色）
                    < 150 => new Color( 60, 160,  60), // 平地・草原（緑）
                    < 200 => new Color(130, 130, 130), // 山（灰色）
                    _     => new Color(240, 240, 240)  // 雪山（白）
                };

                Vector2 position = new(x * TileSize, y * TileSize);
                _spriteBatch.Draw(_pixel, position, null, tileColor, 0f, Vector2.Zero, TileSize, SpriteEffects.None, 0f);
            }
        }

        for (int y = 0; y < _worldMap.Height; y++)
        {
            for (int x = 0; x < _worldMap.Width; x++)
            {
                if (_worldMap.WaterMap[x, y] != 0)
                {
                    float waterVal = _worldMap.WaterMap[x, y];
                    if (waterVal <= 0.001f) continue;
                    float alpha = MathF.Min(waterVal / 2.0f, 1.0f);
                    Color waterColor = new Color(0, 120, 255) * alpha;
                    Rectangle destRect = new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize);
                    _spriteBatch.Draw(_pixel, destRect, waterColor);


                    if (_worldMap.WaterSourceMap[x, y])
                    {
                        _spriteBatch.Draw(_playerTexture, new Vector2(x * TileSize,y * TileSize), Color.Red);

                    }
                }
            }
        }

        _spriteBatch.Draw(_playerTexture, new Vector2(PlayerPos.X * TileSize, PlayerPos.Y * TileSize), Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
