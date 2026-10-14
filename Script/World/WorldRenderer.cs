using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Game.World;

[Flags]
public enum DrawMode
{
    None    = 0,

    Terrain = 1 << 0,
    Water   = 1 << 1,
    Fire    = 1 << 2,
    Ice     = 1 << 3,

    Default = Terrain | Water,
    All     = Terrain | Water | Fire | Ice

}

public class WorldRenderer
{
    private DrawMode _currentDrawMode = DrawMode.Default;
    private KeyboardState _prekb;

    public void HandleInput()
    {
        KeyboardState kb = Keyboard.GetState();

        if (IsKeyTriggered(kb,Keys.F1,_prekb)) _currentDrawMode ^= DrawMode.Terrain; // 地形のみ
        if (IsKeyTriggered(kb,Keys.F2,_prekb)) _currentDrawMode ^= DrawMode.Water;   // 水のみ
        if (IsKeyTriggered(kb,Keys.F3,_prekb)) _currentDrawMode ^= DrawMode.Fire;    // 火のみ
        if (IsKeyTriggered(kb,Keys.F4,_prekb)) _currentDrawMode ^= DrawMode.Ice; // 氷のみ (※後述の記述ミス防止に注意)
        if (IsKeyTriggered(kb,Keys.F5,_prekb)) _currentDrawMode  = DrawMode.Default; // デフォルト (地形+水)
        if (IsKeyTriggered(kb,Keys.F6,_prekb)) _currentDrawMode  = DrawMode.All;     // 全部盛り

        _prekb = kb;
    }
    
    private bool IsKeyTriggered(KeyboardState current, Keys key, KeyboardState previous)
    {
        return current.IsKeyDown(key) && previous.IsKeyUp(key);
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel, World.WorldMap map, Camera2D camera, int screenWidth, int screenHeight, int tileSize)
    {
        // --- 1. 画面内カリング（描画範囲の絞り込み）の計算 ---
        // カメラの Position は画面中央のワールド座標を指しているため、そこから画面の端を逆算する
        Vector2 cameraPos = camera.Position;
        int halfW = screenWidth / 2;
        int halfH = screenHeight / 2;

        // 画面外のはみ出し分（余裕を持たせて±2タイル余分に描画する）
        int startX = Math.Max(0, (int)((cameraPos.X - halfW) / tileSize) - 2);
        int startY = Math.Max(0, (int)((cameraPos.Y - halfH) / tileSize) - 2);
        int endX = Math.Min(map.Width, (int)((cameraPos.X + halfW) / tileSize) + 3);
        int endY = Math.Min(map.Height, (int)((cameraPos.Y + halfH) / tileSize) + 3);

        // --- 2. Terrain（地形）の描画 ---
        if ((_currentDrawMode & DrawMode.Terrain) == DrawMode.Terrain)
        {
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    byte height = map.HeightMap[x, y];
                    Color tileColor = height switch
                    {
                        < 50  => new Color( 60,  60,  60),
                        < 90  => new Color(100, 100, 100),
                        < 110 => new Color(220, 210, 140),
                        < 150 => new Color( 60, 160,  60),
                        < 200 => new Color(130, 130, 130),
                        _     => new Color(240, 240, 240)
                    };
                    spriteBatch.Draw(pixel, new Vector2(x * tileSize, y * tileSize), null, tileColor, 0f, Vector2.Zero, tileSize, SpriteEffects.None, 0f);
                }
            }
        }

        // --- 3. Water（水）の描画 ---
        if ((_currentDrawMode & DrawMode.Water) == DrawMode.Water)
        {
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    float waterVal = map.WaterMap[x, y];
                    if (waterVal <= 0.001f) continue;
                    float alpha = MathF.Min(waterVal / 2.0f, 1.0f);
                    
                    Color waterColor = new Color(0, 120, 255) * alpha;
                    spriteBatch.Draw(pixel, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), waterColor);
                }
            }
        }

        // --- 4. Fire（火）の描画 ---
        if ((_currentDrawMode & DrawMode.Fire) == DrawMode.Fire)
        {
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    byte fireVal = map.FireLayer[x, y];
                    if (fireVal == 0) continue;
                    float alpha = (float)fireVal / 255f;
                    Color fireColor = new Color(255, 60, 0) * alpha;
                    spriteBatch.Draw(pixel, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), fireColor);
                }
            }
        }

        // --- 5. Ice（氷）の描画 ---
        if ((_currentDrawMode & DrawMode.Ice) == DrawMode.Ice)
        {
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    byte iceVal = map.IceLayer[x, y];
                    if (iceVal == 0) continue;
                    float alpha = (float)iceVal / 255f;
                    Color iceColor = new Color(0, 220, 255) * alpha;
                    spriteBatch.Draw(pixel, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), iceColor);
                }
            }
        }
    }
}

