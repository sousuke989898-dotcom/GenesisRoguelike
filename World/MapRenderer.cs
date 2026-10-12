namespace Game;

public class MapRenderer
{
    public static ConsoleColor GetHeightColor(short height)
    {
        if (height < 100)return ConsoleColor.DarkBlue;            // 海（深度）
        else if (height <= 125) return ConsoleColor.Green;    // 海岸・平地
        else if (height <= 150) return ConsoleColor.DarkGreen;
        else if (height <= 175) return ConsoleColor.DarkGray;  // 山岳
        else return ConsoleColor.White;                         // 雪山・険しい峰
    }

    public static string GetTextToDisplay(byte terrainType)
    {
        return terrainType switch
        {
            0 => " .",// 床
            1 => " ■",// 壁
            2 => "炎",// 溶岩・イベント用
            _ => " ？",
        };
    }
}