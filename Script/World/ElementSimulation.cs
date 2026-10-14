using System;
namespace Game.World;

public class ElementSimulation
{
    private const float Range = 0.4f;

    public void GenerateElementMap(WorldMap map, Random rand)
    {
        FastNoiseLite noise = new(rand.Next());

        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2); // 滑らかなノイズ
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);      // 波の重ね合わせ
        noise.SetFractalOctaves(2);                               // 重ね合わせる回数（多いほど細かい起伏が増える）
        noise.SetFrequency(0.01f);                                // 波の大きさ（数値を小さくすると広大な大陸、大きくすると細かい島々になる）

        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y <map.Height; y++)
            {
                float noiseVal = noise.GetNoise(x,y);
                byte intensity = (byte)((MathF.Abs(noiseVal) - Range) * 255 * 2);
                if (noiseVal < -Range)
                {
                    map.IceLayer[x,y] = intensity;
                }
                else if (noiseVal > Range)
                {
                    map.FireLayer[x,y] = intensity;
                }
            }
        }
    }
}