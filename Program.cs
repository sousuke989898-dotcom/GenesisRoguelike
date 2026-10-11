namespace Game;

class Program
{

    const int Width = 16;
    const int Height = 16;

    static int[] map = new int[Width * Height];

    static Point PlayerPos = new(1,1);

    static Random random = new();

    static void Main(string[] args)
    {
        Array.Fill(map,1);
        for (int y = 1; y < Height-1; y++)
        {
            map.AsSpan(y * Width + 1, Width -2).Clear();
        }

        Console.CursorVisible = false;
        RenderMap();

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();

            Point movement = keyInfo.Key switch
            {
                ConsoleKey.W => Point.up,
                ConsoleKey.A => Point.left,
                ConsoleKey.S => Point.down,
                ConsoleKey.D => Point.right,
                _ => Point.zero
            };

            switch (keyInfo.Key)
            {
                case ConsoleKey.Spacebar:
                {
                    int idx = PlayerPos.ToIndex(Width);
                    map[idx] = 2; // 炎・溶岩
                }break;
            }

            var newPos = PlayerPos + movement;
            if (CanMove(newPos)) PlayerPos = newPos;

            RenderMap();

            Genesis();  

        }
    }

    static void Genesis()
    {
        int[] nextMap = (int[])map.Clone();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int index = ToIndex(x,y);
                if (map[index] == 2)
                {
                    int r = random.Next(32);
                    Point burnpos = r switch
                    {
                        0 => Point.up,
                        1 => Point.left,
                        2 => Point.down,
                        3 => Point.right,
                        _ => Point.zero
                    };

                    Point targetPos = new Point(x, y) + burnpos;

                    if (targetPos.X >= 0 && targetPos.X < Width && targetPos.Y >= 0 && targetPos.Y < Height)
                    {
                        int targetIndex = ToIndex(targetPos);

                        if (map[targetIndex] == 0)
                        {
                            nextMap[targetIndex] = 2;
                        }
                    }
                }
            }
        }
        map = nextMap;
    }



    static void RenderMap()
    {
        Console.Clear();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int val = map[ToIndex(x, y)];
                string display = val switch
                {
                    0 => " .",// 床
                    1 => " ■",// 壁
                    2 => "炎",// 溶岩・イベント用
                    _ => " ？",
                };
                Console.Write(display);
            }
            Console.WriteLine();
        }

        Console.SetCursorPosition(PlayerPos.X * 2,PlayerPos.Y);
        Console.Write(" @");
    }




    public static int ToIndex(int x, int y) => x + (y * Width);
    public static int ToIndex(Point point) => point.ToIndex(Width);
    public static Point ToPoint(int index) => new(index % Width, index/Width);
    public static bool CanMove(Point p) => map[p.ToIndex(Width)] != 1;
}
