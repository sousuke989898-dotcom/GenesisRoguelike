namespace Game;

class Program
{

    const int Width = 32;
    const int Height = 32;


    static WorldMap worldMap = new (Width, Height);

    static Point PlayerPos = new(1,1);

    static Random random = new();

    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        Console.Clear();
        RenderMap(worldMap);



        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();

            Point movement = keyInfo.Key switch
            {
                ConsoleKey.UpArrow => Point.up,
                ConsoleKey.LeftArrow => Point.left,
                ConsoleKey.DownArrow => Point.down,
                ConsoleKey.RightArrow => Point.right,
                _ => Point.zero
            };

            // switch (keyInfo.Key)
            // {
            //     case ConsoleKey.Spacebar:
            //     {
            //         int idx = PlayerPos.ToIndex(Width);
            //         map[idx] = 2; // 炎・溶岩
            //     }break;
            // }

            var newPos = PlayerPos + movement;
            if (worldMap.CanMove(newPos)) PlayerPos = newPos;

            RenderMap(worldMap);

        }
    }

    static void Genesis(){} //TODO イベントとして火山の噴火・地震・津波などを用意する



    static void RenderMap(WorldMap worldMap)
    {
        Console.SetCursorPosition(0, 0);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var height = worldMap.HeightMap[x,y];
                Console.BackgroundColor = MapRenderer.GetHeightColor(height);

                var terrainType = worldMap.TerrainMap[x,y];
                string display = MapRenderer.GetTextToDisplay(terrainType);

                Console.Write(display);
            }
            Console.WriteLine();
        }


        Console.BackgroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(PlayerPos.X * 2,PlayerPos.Y);
        Console.Write(" @");

        Console.ResetColor();
    }

}
