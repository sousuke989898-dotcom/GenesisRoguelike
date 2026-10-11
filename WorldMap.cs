namespace Game;

public class WorldMap
{
    public int Width { get; }
    public int Height { get; }

    public int[] TerrainMap { get; }

    public WorldMap(int width, int height)
    {
        Width = width;
        Height = height;
        TerrainMap = new int[width*height];
    }
}