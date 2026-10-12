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

        SmoothingHeight();
    }

    private void GenerateHeightMap(Random rand)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                HeightMap[x, y] = (byte)rand.Next(0, 200);
            }
        }
    }



    private void SmoothingHeight()
    {
        MapLayer<byte> tmp = new(Width,Height);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                tmp[x,y] = GetHeightAverage(new(x,y));
            }
        }

        HeightMap = tmp;
    }


    private byte GetHeightAverage(Point center) //周囲3*3マスの平均を返す
    {
        int sum = 0;
        int count = 0;

        for (int x = center.X-1; x <= center.X +1; x++)
        {
            for (int y = center.Y-1; y <= center.Y +1; y++)
            {
                if (HeightMap.IsInBounds(x, y))
                {
                    sum += HeightMap[x,y];
                    count ++;
                }
            }
        }

        return (byte)(sum/count);
    }



    public bool CanMove(Point nextPoint)
    {
        if (!TerrainMap.IsInBounds(nextPoint)) return false;
        return TerrainMap[nextPoint] != 1;
    }//高低差で移動不可とする可能性があるが、高低差が激しいマップ生成を行わない可能性もある。
}