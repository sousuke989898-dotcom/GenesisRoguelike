using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Game.World;

[Flags]
public enum DrawMode
{
    None          = 0,

    Terrain       = 1 << 0,
    Water         = 1 << 1,
    ThermalEnergy = 1 << 2,
    Temperature   = 1 << 3,
    
    // Fire        = 1 << 3,
    // Ice         = 1 << 4,

    Default = Terrain | Water,
    All     = Terrain | Water | ThermalEnergy | Temperature

}

public class WorldRenderer
{
    private DrawMode _currentDrawMode = DrawMode.Default;
    private KeyboardState _prekb;

    public void HandleInput()
    {
        KeyboardState kb = Keyboard.GetState();

        if (IsKeyTriggered(kb,Keys.F1,_prekb)) _currentDrawMode ^= DrawMode.Terrain;
        if (IsKeyTriggered(kb,Keys.F2,_prekb)) _currentDrawMode ^= DrawMode.Water;
        if (IsKeyTriggered(kb,Keys.F3,_prekb)) _currentDrawMode ^= DrawMode.ThermalEnergy;
        if (IsKeyTriggered(kb,Keys.F4,_prekb)) _currentDrawMode ^= DrawMode.Temperature;
        // if (IsKeyTriggered(kb,Keys.F3,_prekb)) _currentDrawMode ^= DrawMode.Fire;
        // if (IsKeyTriggered(kb,Keys.F4,_prekb)) _currentDrawMode ^= DrawMode.Ice;

        if (IsKeyTriggered(kb,Keys.F5,_prekb)) _currentDrawMode  = DrawMode.Default; // デフォルト (地形+水)
        if (IsKeyTriggered(kb,Keys.F6,_prekb)) _currentDrawMode  = DrawMode.All;     // 全部盛り

        _prekb = kb;
    }
    
    private bool IsKeyTriggered(KeyboardState current, Keys key, KeyboardState previous)
    {
        return current.IsKeyDown(key) && previous.IsKeyUp(key);
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel, WorldMap map, Camera2D camera, int screenWidth, int screenHeight, int tileSize)
    {
        Vector2 cameraPos = camera.Position;
        int halfW = screenWidth / 2;
        int halfH = screenHeight / 2;

        // 画面外のはみ出し分（余裕を持たせて±2タイル余分に描画する）
        int startX = Math.Max(0, (int)((cameraPos.X - halfW) / tileSize) - 2);
        int startY = Math.Max(0, (int)((cameraPos.Y - halfH) / tileSize) - 2);
        int endX = Math.Min(map.Width, (int)((cameraPos.X + halfW) / tileSize) + 3);
        int endY = Math.Min(map.Height, (int)((cameraPos.Y + halfH) / tileSize) + 3);

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

        if ((_currentDrawMode & DrawMode.Water) == DrawMode.Water)
        {
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    byte waterVal = map.WaterMap[x, y];
                    if (waterVal == 0) continue;
                    float alpha = MathF.Min(waterVal / 20.0f, 0.8f);
                    Color waterColor = new Color(0, 120, 255) * alpha;
                    spriteBatch.Draw(pixel, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), waterColor);
                }
            }
        }

        if ((_currentDrawMode & DrawMode.ThermalEnergy) == DrawMode.ThermalEnergy)
        {
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    int energy = map.ThermalEnergyMap[x, y];
                    
                    // 熱エネルギーの数値（例: 20000～80000あたりを想定）を 0.0～1.0 にスケーリング
                    float normalized = MathHelper.Clamp((energy - 5000f) / 15000f, 0f, 1f);
                    
                    // 熱いほど赤、低いほど黒〜青にするヒートマップ風カラー
                    Color energyColor = new Color(normalized, 0f, 1f - normalized) * 0.5f;

                    spriteBatch.Draw(pixel, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), energyColor);
                }
            }
        }

        if ((_currentDrawMode & DrawMode.Temperature) == DrawMode.Temperature)
        {
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    float temp = map.GetTemperature(x, y);

                    // 気温のスケール（例: 3000～10000あたりを想定）に合わせて 0.0～1.0 に正規化
                    float normalized = MathHelper.Clamp((temp - 500f) / 1500f, 0f, 1f);
                    Color tempColor = GetTemperatureColor(normalized) * 0.5f;

                    spriteBatch.Draw(pixel, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), tempColor);
                }
            }
        }


        // // --- Fireの描画 ---
        // if ((_currentDrawMode & DrawMode.Fire) == DrawMode.Fire)
        // {
        //     for (int y = startY; y < endY; y++)
        //     {
        //         for (int x = startX; x < endX; x++)
        //         {
        //             byte fireVal = map.FireLayer[x, y];
        //             if (fireVal == 0) continue;
        //             float alpha = (float)fireVal / 255f;
        //             Color fireColor = new Color(255, 60, 0) * alpha;
        //             spriteBatch.Draw(pixel, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), fireColor);
        //         }
        //     }
        // }

        // // --- Iceの描画 ---
        // if ((_currentDrawMode & DrawMode.Ice) == DrawMode.Ice)
        // {
        //     for (int y = startY; y < endY; y++)
        //     {
        //         for (int x = startX; x < endX; x++)
        //         {
        //             byte iceVal = map.IceLayer[x, y];
        //             if (iceVal == 0) continue;
        //             float alpha = (float)iceVal / 255f;
        //             Color iceColor = new Color(0, 220, 255) * alpha;
        //             spriteBatch.Draw(pixel, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), iceColor);
        //         }
        //     }
        // }
    }

    private Color GetTemperatureColor(float temp)
    {
        // 3段階のグラデーション（青 -> 白 -> 赤）
        if (temp < 0f)
        {
            byte r = (byte)(255 * (1.0f + temp));
            byte g = (byte)(255 * (1.0f + temp));
            byte b = 255;
            return new Color(r, g, b);
        }
        else
        {
            byte r = 255;
            byte g = (byte)(255 * (1.0f - temp));
            byte b = (byte)(255 * (1.0f - temp));
            return new Color(r, g, b);
        }
    }
}

