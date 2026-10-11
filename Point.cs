namespace Game;

public readonly record struct Point(int X, int Y)
{
    public int ToIndex(int width) => X + (Y * width);

    public static Point zero = new(0,0);
    public static Point up = new(0,-1);
    public static Point left = new(-1,0);
    public static Point down = new(0,1);
    public static Point right = new(1,0);

    public static Point operator +(Point a, Point b)=> new(a.X + b.X, a.Y + b.Y);
    public static Point operator -(Point a, Point b)=> new(a.X - b.X, a.Y - b.Y);

    public static Point operator *(Point p, int scalar) => new(p.X * scalar, p.Y * scalar);
    public static Point operator *(int scalar, Point p) => new(p.X * scalar, p.Y * scalar);

    public static Point operator /(Point p, int divisor)=> new(p.X / divisor, p.Y / divisor);
}