using System.Numerics;
using TextGameFramework.Types;
using TextGameFramework.Utils;

namespace TextGameFramework.Functions;
internal static class Plot
{
    internal static void Perform(string key, params object[] args)
    {
        Console.Clear();
        Process process = PublicData.Gamedata.Processes[key];
        if (process.Achievements is not null)
        {
            foreach (string achievementKey in process.Achievements)
            {
                PublicData.gettedAchievement.Add(PublicData.Gamedata.Achievements[achievementKey].Name);
                Perform("on_get_achievement", PublicData.Gamedata.Achievements[achievementKey].Name);
                Thread.Sleep(PublicData.playSpeed * 4);
            }
        }
        if (process.Attributes is not null)
        {
            foreach ((string attributesKey, BigInteger level) in process.Attributes)
            {
                PublicData.attributeLevels.TryGetValue(attributesKey, out var oldLevel);
                PublicData.attributeLevels[attributesKey] = oldLevel + level;
                Perform("on_level_up", PublicData.Gamedata.Attributes[attributesKey].Name, PublicData.attributeLevels[attributesKey]);
                Thread.Sleep(PublicData.playSpeed * 4);
            }
        }
        List<object> newArgs = args.ToList();
        newArgs.Add(string.Join(", ", PublicData.gettedAchievement));
        Console.Out.WritePerChar(string.Format(process.Description, newArgs.ToArray()), PublicData.playSpeed);
        Console.WriteLine();
        if (process.Options is null || process.Options.Count <= 0)
        {
            if (key is "on_get_achievement" or "on_level_up")
            {
                return;
            }
            Thread.Sleep(PublicData.playSpeed * 60);
            return;
        }
        else if (process.Options.Count < 2)
        {
            Thread.Sleep(PublicData.playSpeed * 20);
            if (process.Options.First().Value is string)
            {
                Perform(process.Options.First().Value.ToString(), args);
            }
            return;
        }
        (_, int top) = Console.GetCursorPosition();
        Thread.Sleep(PublicData.playSpeed * 4);
        Console.Out.Rewrite(process.Options.Keys.First(), (default, top), PublicData.choosenForegroundColor, PublicData.choosenBackgroundColor);
        foreach (string option in process.Options.Keys.Skip(1))
        {
            Thread.Sleep(PublicData.playSpeed * 2);
            Console.WriteLine();
            Console.Write(option);
        }
        int index = 0;
        while (true)
        {
            if (!Console.KeyAvailable)
            {
                continue;
            }
            ConsoleKeyInfo inputKey = Console.ReadKey(true);
            switch (inputKey.Key)
            {
                case ConsoleKey.UpArrow:
                    Console.Out.Rewrite(process.Options.Keys.ToArray()[index], (default, top + index));
                    if (index <= 0)
                    {
                        index = process.Options.Count;
                        break;
                    }
                    --index;
                    break;
                case ConsoleKey.DownArrow:
                    Console.Out.Rewrite(process.Options.Keys.ToArray()[index], (default, top + index));
                    if (index + 1 >= process.Options.Count)
                    {
                        index = 0;
                        break;
                    }
                    ++index;
                    break;
                case ConsoleKey.Enter:
                    if (process.Options.Values.ToArray()[index] is string)
                    {
                        Perform(process.Options.Values.ToArray()[index].ToString(), args);
                    }
                    return;
                default: continue;
            }
            Console.Out.Rewrite(process.Options.Keys.ToArray()[index], (default, top + index), PublicData.choosenForegroundColor, PublicData.choosenBackgroundColor);
        }
    }
}
