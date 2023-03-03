using TextGameFramework.Types;
using TextGameFramework.Utils;

namespace TextGameFramework.Functions;
internal static class Plot
{
    internal static void Handle(string key, params string[] args)
    {
        Console.Clear();
        Process process = PublicData.Gamedata.Processes[key];
        if (process.Achievements is not null)
        {
            foreach (string achievementKey in process.Achievements)
            {
                PublicData.gettedAchievement.Add(PublicData.Gamedata.Achievements[achievementKey].Name);
                Handle("on_get_achievement", PublicData.Gamedata.Achievements[achievementKey].Name);
                Thread.Sleep(PublicData.playSpeed * 4);
            }
        }
        foreach (char text in string.Format(process.Description, args))
        {
            Console.Write(text);
            Thread.Sleep(PublicData.playSpeed);
        }
        Console.WriteLine();
        if (process.Options is null)
        {
            if (key is "end" or "on_get_achievement")
            {
                return;
            }
            Thread.Sleep(PublicData.playSpeed * 20);
            Handle("end", string.Join('，', PublicData.gettedAchievement));
            Console.ReadKey(true);
            return;
        }
        (_, int top) = Console.GetCursorPosition();
        Thread.Sleep(PublicData.playSpeed * 4);
        ConsoleHelper.Write(process.Options.Keys.First(), PublicData.choosenForegroundColor, PublicData.choosenBackgroundColor, (default, top));
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
                    ConsoleHelper.Write(process.Options.Keys.ToArray()[index], null, null, (default, top + index));
                    --index;
                    break;
                case ConsoleKey.DownArrow:
                    if (index + 1 >= process.Options.Count)
                    {
                        break;
                    }
                    ConsoleHelper.Write(process.Options.Keys.ToArray()[index], null, null, (default, top + index));
                    ++index;
                    break;
                case ConsoleKey.Enter:
                    Handle(process.Options.Values.ToArray()[index], args);
                    return;
                default: continue;
            }
            ConsoleHelper.Write(process.Options.Keys.ToArray()[index], PublicData.choosenForegroundColor, PublicData.choosenBackgroundColor, (default, top + index));
        }
    }
}
