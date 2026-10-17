using System;
using Microsoft.Xna.Framework;

namespace Game.World;

public class WorldMap
{
    public int Width { get; }
    public int Height { get; }

    const byte SeaLevel = 100;
    const int BaseThermalEnergy = 10000;


    public MapLayer<byte> TerrainMap { get; private set; }
    public MapLayer<byte> HeightMap { get; private set; } //100が基準
    public MapLayer<byte> WaterMap { get; private set; }
    //public MapLayer<bool> WaterSourceMap { get; private set; } //水源

    public MapLayer<int> ThermalEnergyMap {get; private set;} // 熱エネルギー（常に正の整数）

    // --属性レイヤー -- 今後ThermalEnergyと同じ設計にするか考える
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
        WaterMap = new(width, height);
        //WaterSourceMap = new(width, height);

        ThermalEnergyMap = new(width, height);

        FireLayer = new(width, height);
        IceLayer = new(width, height);

        // ElementSimulation = new();

        Random rand = new();
        GenerateBaseMap(rand);
        //ElementSimulation.GenerateElementMap(this,rand);
        //GenerateWaterSourceMap(rand);

    }

    private void GenerateBaseMap(Random rand)
    {
        FastNoiseLite hNoise = new(rand.Next()); //標高ノイズ
        FastNoiseLite tNoise = new(rand.Next()); //気温ノイズ

        hNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2); // 滑らかなノイズ
        hNoise.SetFractalType(FastNoiseLite.FractalType.FBm);      // 波の重ね合わせ
        hNoise.SetFractalOctaves(3);                               // 重ね合わせる回数（多いほど細かい起伏が増える）
        hNoise.SetFrequency(0.01f);                                // 波の大きさ（数値を小さくすると広大な大陸、大きくすると細かい島々になる）

        tNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2); // 滑らかなノイズ
        tNoise.SetFrequency(0.002f);                              // 波の大きさ（数値を小さくすると広大な大陸、大きくすると細かい島々になる）

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                byte height = (byte)((hNoise.GetNoise(x, y) + 1f) / 2f * 255);
                HeightMap[x, y] = height;

                float climateNoise = tNoise.GetNoise(x, y); 
                int thermalVariation = (int)(climateNoise * 2000); // 基準から ±5000 の揺らぎ

                int initialEnergy = BaseThermalEnergy + thermalVariation;

                // if (height >= SeaLevel)
                // {
                //     int altitudeDelta = height - SeaLevel;
                //     initialEnergy -= altitudeDelta * 120; // 標高が上がるほど冷える
                // }
                // else
                // {
                //     int depthDelta = SeaLevel - height;
                //     initialEnergy -= depthDelta * 40;
                // } //todo 高度による補正の追加

                ThermalEnergyMap[x, y] = Math.Max(0, initialEnergy);

                if (height < SeaLevel)
                {
                    WaterMap[x,y] = (byte)(SeaLevel - height); //海
                }
            }
        }
    }

    /// <span class="markdown-node"></span> 地形に応じた「比熱（温まりにくさ・冷めにくさの係数）」を返す
    public int GetSpecificHeat(int x, int y)
    {
        byte height = HeightMap[x, y];

        if (height < SeaLevel)
        {
            return 400; // 水
        }
        else if (height > 200)
        {
            return 200;
        }
        else
        {
            return 256;  // 通常の陸地（土・平原）
        }
    }

    /// <span class="markdown-node"></span> 熱エネルギーと比熱から、その場の「相対温度（指標）」を算出する
    public float GetTemperature(int x, int y)
    {
        int energy = ThermalEnergyMap[x, y];
        int specificHeat = GetSpecificHeat(x, y);

        // 熱力学の基本: 温度 = 熱エネルギー / 比熱
        // 描画やバイオーム判定用に使いやすい float の値に変換して返す
        return (float)energy / specificHeat;
    }



    public bool CanMove(Point nextPos)
    {
        if (!TerrainMap.IsInBounds(nextPos)) return false;
        var h = HeightMap[nextPos];
        return SeaLevel <= h; //水は歩行不可
    }
}