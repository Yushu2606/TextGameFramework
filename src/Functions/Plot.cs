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
                if (PublicData.Gamedata.Achievements[achievementKey].MessageKey is null)
                {
                    continue;
                }
                Perform(PublicData.Gamedata.Achievements[achievementKey].MessageKey, PublicData.Gamedata.Achievements[achievementKey].Name);
                Thread.Sleep(PublicData.playSpeed * 4);
            }
        }
        if (process.Attributes is not null)
        {
            foreach ((string attributesKey, BigInteger level) in process.Attributes)
            {
                PublicData.attributeLevels.TryGetValue(attributesKey, out BigInteger oldLevel);
                PublicData.attributeLevels[attributesKey] = oldLevel + level;
                if (PublicData.Gamedata.Attributes[attributesKey].MessageKey is null)
                {
                    continue;
                }
                Perform(PublicData.Gamedata.Attributes[attributesKey].MessageKey, PublicData.Gamedata.Attributes[attributesKey].Name, PublicData.attributeLevels[attributesKey]);
                Thread.Sleep(PublicData.playSpeed * 4);
            }
        }
        List<object> newArgs = args.ToList();
        newArgs.Add(string.Join(", ", PublicData.gettedAchievement));
        Console.Out.WritePerChar(string.Format(process.Description, newArgs.ToArray()), PublicData.playSpeed);
        Console.WriteLine();
        if (process.Options is null || process.Options.Count <= 0)
        {
            Thread.Sleep(PublicData.playSpeed * 60);
            return;
        }
        else if (process.Options.Count < 2)
        {
            Thread.Sleep(PublicData.playSpeed * 20);
            RandOption(process.Options.First().Value, key => Perform(key, args));
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
                        index = process.Options.Count - 1;
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
                    RandOption(process.Options.Values.ToArray()[index], key => Perform(key, args));
                    return;
                default: continue;
            }
            Console.Out.Rewrite(process.Options.Keys.ToArray()[index], (default, top + index), PublicData.choosenForegroundColor, PublicData.choosenBackgroundColor);
        }
    }
    private static void RandOption(object option, Action<string> action)
    {
        if (option is string str)
        {
            action(str);
        }
        else if (option is Dictionary<object, object> dic)
        {
            long sum = default;
            foreach (object value in dic.Values)
            {
                switch (value)
                {
                    case List<object>:
                        foreach (string att in ((List<object>)value).Cast<string>())
                        {
                            sum += PublicData.attributeLevels.TryGetValue(att.ToString(), out BigInteger bigint) ? (long)bigint : Convert.ToInt64(att);
                        }
                        break;
                    default:
                        sum += Convert.ToInt64(value);
                        break;
                }
            }
            long rand = Random.Shared.NextInt64(sum);
            BigInteger testedWeight = default;
            foreach ((object optionKey, object weight) in dic)
            {
                BigInteger testWeight = default;
                switch (weight)
                {
                    case List<object>:
                        foreach (string att in ((List<object>)weight).Cast<string>())
                        {
                            testWeight += PublicData.attributeLevels.TryGetValue(att.ToString(), out BigInteger bigint) ? bigint : BigInteger.Parse(att.ToString());
                        }
                        break;
                    default:
                        testWeight = BigInteger.Parse(weight.ToString());
                        break;
                }
                if (rand >= (testedWeight += testWeight))
                {
                    continue;
                }
                action(optionKey.ToString());
                return;
            }
        }
    }
}
