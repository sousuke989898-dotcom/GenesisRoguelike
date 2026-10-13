using Microsoft.Xna.Framework;
namespace Game;

[System.Flags]
public enum Direction {
    None = 0,
    Upper = 1,
    Right = 2,
    Down = 4,
    Left = 8,
}

public static class DirectionTool
{
    public static readonly Direction[] baseDirs = [Direction.Left, Direction.Right, Direction.Down, Direction.Upper];

    public static readonly Direction[] AllDirs = {
        Direction.Upper, Direction.Right | Direction.Upper,
        Direction.Right, Direction.Right | Direction.Down,
        Direction.Down,  Direction.Left  | Direction.Down,
        Direction.Left,  Direction.Left  | Direction.Upper
    };

}

public static class DirectionExtensions
{
    public static Direction GetOpposite(this Direction dir)
    {
        Direction result = Direction.None;
        if (dir.HasFlag(Direction.Upper)) result |= Direction.Down;
        if (dir.HasFlag(Direction.Down))  result |= Direction.Upper;
        if (dir.HasFlag(Direction.Left))  result |= Direction.Right;
        if (dir.HasFlag(Direction.Right)) result |= Direction.Left;
        return result;
    }

    public static Point ToPoint(this Direction dir)
    {
        Point point = Point.Zero;
        if (dir.HasFlag(Direction.Upper)) point.Y --;
        if (dir.HasFlag(Direction.Down)) point.Y ++;
        if (dir.HasFlag(Direction.Left)) point.X --;
        if (dir.HasFlag(Direction.Right)) point.X ++;
        return point;
    }

    public static Direction ValidatedDir(this Direction dir)
    {
        if (dir.HasFlag(Direction.Upper) && dir.HasFlag(Direction.Down))
            dir &= ~(Direction.Upper | Direction.Down);
        if (dir.HasFlag(Direction.Right) && dir.HasFlag(Direction.Left))
            dir &= ~(Direction.Right | Direction.Left);
        return dir;
    }
}
