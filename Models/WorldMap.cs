using System;
using Microsoft.Xna.Framework;

namespace Game;

public class WorldMap
{
    public int Width { get; }
    public int Height { get; }

    public MapLayer<byte> TerrainMap { get; private set; }
    public MapLayer<byte> HeightMap {get; private set; } //100が基準

    public WorldMap(int width, int height)
    {
        if (width < 5 || height < 5)
            throw new ArgumentException("Map size must be at least 5x5.");
        Width = width;
        Height = height;
        TerrainMap = new(width, height);
        HeightMap = new(width, height);



        TerrainMap.Fill(1);
        for (int y = 1; y < Height-1; y++) TerrainMap.AsSpan(y * Width + 1, Width -2).Clear(); //外側を残し床(0)で埋める


        Random rand = new();

        GenerateHeightMap(rand);
    }

    private void GenerateHeightMap(Random rand)
    {
        FastNoiseLite noise = new();

        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2); // 滑らかなノイズ
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);      // 波の重ね合わせ
        noise.SetFractalOctaves(3);                               // 重ね合わせる回数（多いほど細かい起伏が増える）
        noise.SetFrequency(0.02f);                                // 波の大きさ（数値を小さくすると広大な大陸、大きくすると細かい島々になる）

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                float noiseValue = noise.GetNoise(x, y); //ノイズ値を取得
                float normalized = (noiseValue + 1f) / 2f; //0.0 ~ 1.0に正規化
                HeightMap[x, y] = (byte)(normalized * 255); // 0 ~ 255のbyteに変換
            }
        }
    }

    public bool CanMove(Point nextPoint)
    {
        if (!TerrainMap.IsInBounds(nextPoint)) return false;
        var h = HeightMap[nextPoint];
        return 100 <= h; //水は歩行不可
    }
}