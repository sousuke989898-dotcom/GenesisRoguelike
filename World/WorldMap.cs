namespace Game.World;

public class WorldMap
{
    public int Width { get; }
    public int Height { get; }

    public int[] TerrainMap { get; }
    public int[] HeightMap {get; }

    public WorldMap(int width, int height)
    {
        Width = width;
        Height = height;
        TerrainMap = new int[width * height];
        HeightMap = new int[width * height];

        Array.Fill(TerrainMap, 1);
        for (int y = 1; y < Height-1; y++) TerrainMap.AsSpan(y * Width + 1, Width -2).Clear(); //外側を残し床(0)で埋める
    }
}