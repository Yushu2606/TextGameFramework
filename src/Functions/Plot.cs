using System.Numerics;
using TextGameFramework.Types;
using TextGameFramework.Utils;

namespace TextGameFramework.Functions;
internal static class Plot
{
    internal static void Perform(string key, bool isCancelled = true, params object[] args)
    {
        Process process = PublicData.Gamedata.Processes[key];
        int top = default;
        bool isOutputing = true;
        Console.Clear();
        Task.Run(() =>
        {
            if (process.Achievements is not null)
            {
                foreach (string achievementKey in process.Achievements)
                {
                    PublicData.gettedAchievement.Add(PublicData.Gamedata.Achievements[achievementKey].Name);
                    if (PublicData.Gamedata.Achievements[achievementKey].MessageKey is null)
                    {
                        continue;
                    }
                    Perform(PublicData.Gamedata.Achievements[achievementKey].MessageKey, true, PublicData.Gamedata.Achievements[achievementKey].Name);
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
                    Perform(PublicData.Gamedata.Attributes[attributesKey].MessageKey, true, PublicData.Gamedata.Attributes[attributesKey].Name, PublicData.attributeLevels[attributesKey]);
                }
            }
            List<object> newArgs = args.ToList();
            newArgs.Add(string.Join(", ", PublicData.gettedAchievement));
            lock (Console.Out)
            {
                Console.Out.WritePerChar(string.Format(process.Description, newArgs.ToArray()), PublicData.playSpeed);
            }
            if (process.Options is null || process.Options.Count <= 0)
            {
                Thread.Sleep(PublicData.playSpeed * 60);
                return;
            }
            else if (process.Options.Count < 2)
            {
                Thread.Sleep(PublicData.playSpeed * 20);
                RandOption(process.Options.First().Value, key => Perform(key, default, args));
                return;
            }
            Console.WriteLine();
            (_, top) = Console.GetCursorPosition();
            Thread.Sleep(PublicData.playSpeed * 4);
            Console.Out.Rewrite(process.Options.Keys.First(), (default, top), PublicData.choosenForegroundColor, PublicData.choosenBackgroundColor);
            foreach (string option in process.Options.Keys.Skip(1))
            {
                Thread.Sleep(PublicData.playSpeed * 2);
                Console.WriteLine();
                Console.Write(option);
            }
            isOutputing = false;
        }, CancellationToken.None);
        while (isOutputing && !isCancelled)
        {
            Thread.Yield();
        }
        if (isCancelled)
        {
            return;
        }
        int index = 0;
        Console.CursorVisible = false;
        while (true)
        {
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
                    Console.CursorVisible = true;
                    RandOption(process.Options.Values.ToArray()[index], key => Perform(key, default, args));
                    return;
                default: continue;
            }
            Console.Out.Rewrite(process.Options.Keys.ToArray()[index], (default, top + index), PublicData.choosenForegroundColor, PublicData.choosenBackgroundColor);
        }
    }
    private static void RandOption(object option, Action<string> action)
    {
        switch (option)
        {
            case string str:
                action(str);
                break;
            case Dictionary<object, object> dic:
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
                break;
        }
    }
}
