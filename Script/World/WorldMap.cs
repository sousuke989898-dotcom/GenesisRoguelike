using System;
using Microsoft.Xna.Framework;

namespace Game.World;

public class WorldMap
{
    public int Width { get; }
    public int Height { get; }

    public MapLayer<byte> TerrainMap { get; private set; }
    public MapLayer<byte> HeightMap { get; private set; } //100が基準
    public MapLayer<byte> TemperatureMap { get; private set; }
    public MapLayer<byte> WaterMap { get; private set; }
    public MapLayer<bool> WaterSourceMap { get; private set; }


    public WorldMap(int width, int height)
    {
        if (width < 5 || height < 5)
            throw new ArgumentException("Map size must be at least 5x5.");
        Width = width;
        Height = height;

        TerrainMap = new(width, height);
        HeightMap = new(width, height);
        TemperatureMap = new(width, height);
        WaterMap = new(width, height);
        WaterSourceMap = new(width, height);

        Random rand = new();
        GenerateHeightMap(rand);
        //GenerateWaterSourceMap(rand);

    }

    private void GenerateHeightMap(Random rand)
    {
        FastNoiseLite noise = new(rand.Next());

        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2); // 滑らかなノイズ
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);      // 波の重ね合わせ
        noise.SetFractalOctaves(3);                               // 重ね合わせる回数（多いほど細かい起伏が増える）
        noise.SetFrequency(0.02f);                                // 波の大きさ（数値を小さくすると広大な大陸、大きくすると細かい島々になる）

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float noiseValue = (noise.GetNoise(x, y) + 1f) / 2f; //ノイズ値を取得
                byte height = (byte)(noiseValue * 255);
                HeightMap[x, y] = height;

                TemperatureMap[x, y] = (byte)(255 - (height / 2));

                if (height < 100)
                {
                    WaterMap[x,y] = (byte)(100 - height);
                }
            }
        }
    }

    private void GenerateWaterSourceMap(Random rand)
    {
        FastNoiseLite sourceNoise = new(rand.Next());
        sourceNoise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        sourceNoise.SetFrequency(0.1f);

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                byte height = HeightMap[x, y];
                if (150 <= height && height < 200)
                {
                    float srcVal = sourceNoise.GetNoise(x, y);
                    WaterSourceMap[x,y] = srcVal < -0.99f;
                }
            }
        }
    }

    public void RunWater() //水を流す
    {
        Span<Point> directions =
        [
            new Point(0, -1), // 上
            new Point(0, 1),  // 下
            new Point(-1, 0), // 左
            new Point(1, 0)   // 右
        ];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                byte cHeight = HeightMap[x,y];
                if (cHeight < 100) continue; //海
                if (WaterSourceMap[x,y]) WaterMap[x,y] += 2;

                byte cWater  = WaterMap[x,y];
                if (cWater == 0) continue; //水がない


                int cWaterLevel = cHeight + cWater; //水位
                int minWaterLevel = cWaterLevel;

                int bestX = x;
                int bestY = y;

                foreach (Point dir in directions)
                {
                    int nx = x + dir.X;
                    int ny = y + dir.Y;
                    if (!HeightMap.IsInBounds(nx, ny)) continue;

                    byte nWater = WaterMap[nx,ny];
                    byte nHeifht = HeightMap[nx,ny];
                    if (nHeifht < 100)
                    {
                        bestX = nx;
                        bestY = ny;
                        break;
                    }
                    int nWaterLevel = nHeifht + nWater;

                    if (nWaterLevel < minWaterLevel)
                    {
                        minWaterLevel = nWaterLevel;
                        bestX = nx;
                        bestY = ny;
                    }
                }

                if (bestX != x || bestY != y)
                {
                    WaterMap[x, y]--;
                    
                    if (HeightMap[bestX, bestY] >= 100) //海は除外
                    {
                        WaterMap[bestX, bestY]++;
                    }
                }
            }
        }

    }

    public bool CanMove(Point nextPos)
    {
        if (!TerrainMap.IsInBounds(nextPos)) return false;
        var h = HeightMap[nextPos];
        return 100 <= h; //水は歩行不可
    }
}