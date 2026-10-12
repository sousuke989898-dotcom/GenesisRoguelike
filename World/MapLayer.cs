namespace Game;

public class MapLayer<T>
{
    public int Width { get; }
    public int Height { get; }
    private readonly T[] _data;

    public MapLayer(int width, int height)
    {
        Width = width;
        Height = height;
        _data = new T[width * height];
    }

    public T this[int x, int y]
    {
        get => _data[ToIndex(x, y)];
        set => _data[ToIndex(x, y)] = value;
    }

    public T this[Point p]
    {
        get => _data[ToIndex(p.X, p.Y)];
        set => _data[ToIndex(p.X, p.Y)] = value;
    }

    public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
    public bool IsInBounds(Point p) => IsInBounds(p.X, p.Y);

    private int ToIndex(int x, int y) => x + (y * Width);

    public Span<T> AsSpan() => _data.AsSpan();
    public Span<T> AsSpan(int start, int length) => _data.AsSpan(start,length);
    public Span<T> GetRowSpan(int y) => _data.AsSpan(y * Width, Width);

    public void Fill(T value) => Array.Fill(_data, value);

    public MapLayer<T> Clone()
    {
        var clone = new MapLayer<T>(Width, Height);
        Array.Copy(_data, clone._data, _data.Length);
        return clone;
    }
}