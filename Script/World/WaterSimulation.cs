using System;
using Microsoft.Xna.Framework;
namespace Game.World;

public class WaterSimulation
{
    // public void GenerateWaterSourceMap(Random rand,WorldMap map)
    // {
    //     FastNoiseLite sourceNoise = new(rand.Next());
    //     sourceNoise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
    //     sourceNoise.SetFrequency(0.1f);

    //     for (int x = 0; x < map.Width; x++)
    //     {
    //         for (int y = 0; y < map.Height; y++)
    //         {
    //             byte height = map.HeightMap[x, y];
    //             if (150 <= height && height < 200)
    //             {
    //                 float srcVal = sourceNoise.GetNoise(x, y);
    //                 map.WaterSourceMap[x,y] = srcVal < -0.99f;
    //             }
    //         }
    //     }
    // }

    public void RunWater(WorldMap map) //水を流す
    {
        Span<Point> directions =
        [
            new Point(0, -1), // 上
            new Point(0, 1),  // 下
            new Point(-1, 0), // 左
            new Point(1, 0)   // 右
        ];

        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                byte cHeight = map.HeightMap[x,y];
                if (cHeight < 100) continue; //海
                //if (worldMap.WaterSourceMap[x,y]) worldMap.WaterMap[x,y] += 2;

                byte cWater  = map.WaterMap[x,y];
                if (cWater == 0) continue; //水がない


                int cWaterLevel = cHeight + cWater; //水位
                int minWaterLevel = cWaterLevel;

                int bestX = x;
                int bestY = y;

                foreach (Point dir in directions)
                {
                    int nx = x + dir.X;
                    int ny = y + dir.Y;
                    if (!map.HeightMap.IsInBounds(nx, ny)) continue;

                    byte nWater = map.WaterMap[nx,ny];
                    byte nHeifht = map.HeightMap[nx,ny];
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
                    map.WaterMap[x, y]--;
                    
                    if (map.HeightMap[bestX, bestY] >= 100) //海は除外
                    {
                        map.WaterMap[bestX, bestY]++;
                    }
                }
            }
        }

    }
}