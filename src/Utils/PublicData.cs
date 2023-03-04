using System.Numerics;
using TextGameFramework.Types;

namespace TextGameFramework.Utils;
internal static class PublicData
{
    public static readonly string programName = "TextGameFramework";
    public static readonly int playSpeed =
#if DEBUG
        0
#else
        50
#endif
        ;
    public static readonly ConsoleColor choosenBackgroundColor = ConsoleColor.DarkGray;
    public static readonly ConsoleColor choosenForegroundColor = ConsoleColor.White;
    public static GameData Gamedata { get; set; }
    public static readonly List<string> gettedAchievement = new();
    public static readonly Dictionary<string, BigInteger> attributeLevels = new();
}
