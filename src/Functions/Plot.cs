using System.Numerics;
using TextGameFramework.Types;
using TextGameFramework.Utils;

namespace TextGameFramework.Functions;

internal static class Plot
{
    private static void Perform(string key, params object[] args)
    {
        if (args.Length <= 0)
        {
            Console.Clear();
        }
        Process process = PublicData.Gamedata.Processes[key];
        if (process.Attributes is not null)
        {
            foreach ((string attributesKey, BigInteger level) in process.Attributes)
            {
                PublicData.Gamedata.AttributeLevels.TryGetValue(attributesKey, out BigInteger oldLevel);
                PublicData.Gamedata.AttributeLevels[attributesKey] = oldLevel + level;
                if (PublicData.Gamedata.Attributes[attributesKey].MessageKey is null)
                {
                    continue;
                }
                Perform(PublicData.Gamedata.Attributes[attributesKey].MessageKey, PublicData.Gamedata.Attributes[attributesKey].Name, PublicData.Gamedata.AttributeLevels[attributesKey]);
            }
        }
        if (process.Achievements is not null)
        {
            foreach (string achievementKey in process.Achievements)
            {
                PublicData.Gamedata.GettedAchievement.Add(PublicData.Gamedata.Achievements[achievementKey].Name);
                if (PublicData.Gamedata.Achievements[achievementKey].MessageKey is null)
                {
                    continue;
                }
                Perform(PublicData.Gamedata.Achievements[achievementKey].MessageKey, PublicData.Gamedata.Achievements[achievementKey].Name);
            }
        }
        List<object> newArgs = PublicData.Gamedata.Arguments.ToList();
        newArgs.Add(string.Join(", ", PublicData.Gamedata.GettedAchievement));
        newArgs.AddRange(args);
        Console.CursorVisible = true;
        Console.Out.WritePerChar(string.Format(process.Description, newArgs.ToArray()), PublicData.playSpeed);
        if (args.Length > 0)
        {
            Console.WriteLine();
            Thread.Sleep(PublicData.playSpeed * 20);
            return;
        }
        Console.CursorVisible = false;
        if (process.Options is null || process.Options.Count <= 0)
        {
            PublicData.Gamedata.CurrectOptions = Array.Empty<KeyValuePair<string, object>>();
            return;
        }
        KeyValuePair<string, object>[] options = process.Options.ToArray();
        if (process.Options.Count < 2)
        {
            PublicData.Gamedata.CurrectOptions = options;
            return;
        }
        Thread.Sleep(PublicData.playSpeed * 5);
        PublicData.Top = Console.GetCursorPosition().Top + 1;
        for (int i = 0; i < process.Options.Count; ++i)
        {
            Console.WriteLine();
            Console.Out.Rewrite(options[i].Key, (default, PublicData.Top + i));
        }
        Console.Out.Rewrite(options[0].Key, (default, PublicData.Top), PublicData.choosenForegroundColor, PublicData.choosenBackgroundColor);
        PublicData.Gamedata.CurrectOptions = options;
    }
    internal static void Process(object option)
    {
        switch (option)
        {
            case string str:
                Perform(str);
                break;
            case Dictionary<object, object> dic:
                BigInteger sum = default;
                foreach (object value in dic.Values)
                {
                    switch (value)
                    {
                        case List<object> attrs:
                            foreach (string attr in attrs.Cast<string>())
                            {
                                sum += PublicData.Gamedata.AttributeLevels.TryGetValue(attr, out BigInteger bigint) ? bigint : BigInteger.Parse(attr);
                            }
                            break;
                        default:
                            sum += BigInteger.Parse(value.ToString());
                            break;
                    }
                }
                long rand = Random.Shared.NextInt64((long)sum);
                BigInteger testedWeight = default;
                foreach ((object optionKey, object weight) in dic)
                {
                    BigInteger testWeight = default;
                    switch (weight)
                    {
                        case List<object> attrs:
                            foreach (string attr in attrs.Cast<string>())
                            {
                                testWeight += PublicData.Gamedata.AttributeLevels.TryGetValue(attr, out BigInteger bigint) ? bigint : BigInteger.Parse(attr);
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
                    Perform(optionKey.ToString());
                    break;
                }
                break;
            default:
                throw new ArgumentException(option.ToString());
        }
    }
}
