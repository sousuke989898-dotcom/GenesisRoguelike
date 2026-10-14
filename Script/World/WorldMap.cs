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
    //public MapLayer<bool> WaterSourceMap { get; private set; } //水源

    public MapLayer<byte> FireLayer { get; }
    public MapLayer<byte> IceLayer { get; }

    public ElementSimulation ElementSimulation {get; }


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
        //WaterSourceMap = new(width, height);

        FireLayer = new(width, height);
        IceLayer = new(width, height);

        ElementSimulation = new();

        Random rand = new();
        GenerateBaseMap(rand);
        ElementSimulation.GenerateElementMap(this,rand);
        //GenerateWaterSourceMap(rand);

    }

    private void GenerateBaseMap(Random rand)
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
                byte height = (byte)((noise.GetNoise(x, y) + 1f) / 2f * 255);
                HeightMap[x, y] = height;
                TemperatureMap[x, y] = (byte)(255 - (height / 2));

                if (height < 100)
                {
                    WaterMap[x,y] = (byte)(100 - height); //海
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