﻿using TextGameFramework.Types;

namespace TextGameFramework.Utils;
internal readonly struct PublicData
{
    public static readonly string programName = "TextGameFramework";
    public static readonly int playSpeed = 50;
    public static readonly ConsoleColor choosenBackgroundColor = ConsoleColor.DarkGray;
    public static readonly ConsoleColor choosenForegroundColor = ConsoleColor.White;
    public static GameData Gamedata { get; set; }
    public static readonly List<string> gettedAchievement = new();
}
