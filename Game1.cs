using System;
using Game.World;
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

    private WorldMap _worldMap;
    private WorldRenderer _worldRenderer;

    private WaterSimulation _waterSimulation = new();
    private const int TileSize = 16;

    private const int MapSizeX = 1000;
    private const int MapSizeY = 1000;

    private Point PlayerPos = new(MapSizeX/2, MapSizeY/2);

    private float _moveTimer = 0f;
    private const float MoveDelay = 0.01f; // 移動間隔（秒）

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
        _worldRenderer = new WorldRenderer();

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
                    // _waterSimulation.RunWater(_worldMap);
                    PlayerPos = nextPos;
                }
                _moveTimer = 0f;
            }
        }


        Vector2 playerPixelPos = new(PlayerPos.X * TileSize + TileSize / 2f, PlayerPos.Y * TileSize + TileSize / 2f);
        _camera.Update(playerPixelPos);

        _worldRenderer.HandleInput();


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        Matrix viewMatrix = _camera.GetViewMatrix(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        _spriteBatch.Begin(transformMatrix: viewMatrix);

        _worldRenderer.Draw(
            _spriteBatch, 
            _pixel, 
            _worldMap, 
            _camera, 
            _graphics.PreferredBackBufferWidth, 
            _graphics.PreferredBackBufferHeight, 
            TileSize
        );

        _spriteBatch.Draw(_playerTexture, new Vector2(PlayerPos.X * TileSize, PlayerPos.Y * TileSize), Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
