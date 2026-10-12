namespace Game;

public class WorldMap
{
    public int Width { get; }
    public int Height { get; }

    public MapLayer<byte> TerrainMap { get; }
    public MapLayer<byte> HeightMap {get; } //100が基準

    public WorldMap(int width, int height)
    {
        Width = width;
        Height = height;
        TerrainMap = new(width, height);
        HeightMap = new(width, height);

        TerrainMap.Fill(1);
        for (int y = 1; y < Height-1; y++) TerrainMap.AsSpan(y * Width + 1, Width -2).Clear(); //外側を残し床(0)で埋める

        Random rand = new();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                byte testHeight = (byte)rand.Next(0, 200);
                HeightMap[x, y] = testHeight;
            }
        }

    }

    private void CreateTerrain()
    {
        
    }



    public bool CanMove(Point nextPoint)
    {
        return TerrainMap[nextPoint] != 1;
    }//高低差で移動不可とする可能性があるが、高低差が激しいマップ生成を行わない可能性もある。
}