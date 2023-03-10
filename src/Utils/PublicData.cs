using TextGameFramework.Types;

namespace TextGameFramework.Utils;

internal static class PublicData
{
    public static readonly int playSpeed = 50;
    public static readonly ConsoleColor choosenBackgroundColor = ConsoleColor.DarkGray;
    public static readonly ConsoleColor choosenForegroundColor = ConsoleColor.White;
    public static GameData Gamedata { get; set; }
    public static int Top { get; set; }
}
