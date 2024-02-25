using System.Collections.Concurrent;
using System.Numerics;
using TextGameFramework.Types;

namespace TextGameFramework.Utils;

internal static class Shared
{
    public const int PlaySpeed = 50;
    public const ConsoleColor ChosenBackgroundColor = ConsoleColor.DarkGray;
    public const ConsoleColor ChosenForegroundColor = ConsoleColor.White;
    public static GameData? GameData;
    public static KeyValuePair<string, object>[]? CurrentOptions;
    public static readonly ConcurrentBag<string> GotAchievement = [];
    public static readonly ConcurrentDictionary<string, BigInteger> AttributeLevels = [];
    public static string?[]? Arguments;
    public static int Top;
}