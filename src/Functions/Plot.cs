using TextGameFramework.Types;
using TextGameFramework.Utils;

namespace TextGameFramework.Functions;
internal static class Plot
{
    internal static void Perform(string key, params string[] args)
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
        Console.Out.WritePerChar(string.Format(process.Description, args), PublicData.playSpeed);
        Console.WriteLine();
        if (process.Options is null || process.Options.Count <= 0)
        {
            if (key is "end" or "on_get_achievement")
            {
                return;
            }
            Thread.Sleep(PublicData.playSpeed * 20);
            Perform("end", string.Join('，', PublicData.gettedAchievement));
            Console.ReadKey(true);
            return;
        }
        else if (process.Options.Count < 2)
        {
            Thread.Sleep(PublicData.playSpeed * 20);
            Perform(process.Options.First().Value, args);
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
            ConsoleKeyInfo inputKey = Console.ReadKey(true);
            switch (inputKey.Key)
            {
                case ConsoleKey.UpArrow:
                    if (index <= 0)
                    {
                        break;
                    }
                    Console.Out.Rewrite(process.Options.Keys.ToArray()[index], (default, top + index));
                    --index;
                    break;
                case ConsoleKey.DownArrow:
                    if (index + 1 >= process.Options.Count)
                    {
                        break;
                    }
                    Console.Out.Rewrite(process.Options.Keys.ToArray()[index], (default, top + index));
                    ++index;
                    break;
                case ConsoleKey.Enter:
                    Perform(process.Options.Values.ToArray()[index], args);
                    return;
                default: continue;
            }
            Console.Out.Rewrite(process.Options.Keys.ToArray()[index], (default, top + index), PublicData.choosenForegroundColor, PublicData.choosenBackgroundColor);
        }
    }
}
