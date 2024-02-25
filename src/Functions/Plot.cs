using System.Numerics;
using TextGameFramework.Types;
using TextGameFramework.Utils;

namespace TextGameFramework.Functions;

internal static class Plot
{
    private static void Perform(string key, params object[] args)
    {
        if (Shared.GameData is null)
        {
            throw new NullReferenceException();
        }

        if (args.Length <= 0)
        {
            Console.Clear();
        }

        Process process = Shared.GameData.Processes[key];
        if (process.Attributes is not null && Shared.GameData.Attributes is not null)
        {
            foreach ((string attributesKey, BigInteger level) in process.Attributes)
            {
                Shared.AttributeLevels.TryGetValue(attributesKey, out BigInteger oldLevel);
                Shared.AttributeLevels[attributesKey] = oldLevel + level;
                if (Shared.GameData.Attributes[attributesKey].MessageKey is null)
                {
                    continue;
                }

                Perform(Shared.GameData.Attributes[attributesKey].MessageKey!,
                    Shared.GameData.Attributes[attributesKey].Name, Shared.AttributeLevels[attributesKey]);
            }
        }

        if (process.Achievements is not null && Shared.GameData.Achievements is not null)
        {
            foreach (string achievementKey in process.Achievements)
            {
                Shared.GotAchievement.Add(Shared.GameData.Achievements[achievementKey].Name);
                if (Shared.GameData.Achievements[achievementKey].MessageKey is null)
                {
                    continue;
                }

                Perform(Shared.GameData.Achievements[achievementKey].MessageKey!,
                    Shared.GameData.Achievements[achievementKey].Name);
            }
        }

        List<string?> newArgs = Shared.Arguments is null ? [] : Shared.Arguments.ToList();
        newArgs.Add(string.Join(", ", Shared.GotAchievement));
        newArgs.AddRange(args.Cast<string>());
        Console.CursorVisible = true;
        Console.Out.WritePerChar(string.Format(process.Description, [..newArgs]), Shared.PlaySpeed);
        if (args.Length > 0)
        {
            Console.WriteLine();
            Thread.Sleep(Shared.PlaySpeed * 20);
            return;
        }

        Console.CursorVisible = false;
        if (process.Options is null || (process.Options.Count <= 0))
        {
            Shared.CurrentOptions = [];
            return;
        }

        KeyValuePair<string, object>[] options = process.Options.ToArray();
        if (process.Options.Count < 2)
        {
            Shared.CurrentOptions = options;
            return;
        }

        Thread.Sleep(Shared.PlaySpeed * 5);
        Shared.Top = Console.GetCursorPosition().Top + 1;
        for (int i = 0; i < process.Options.Count; ++i)
        {
            Console.WriteLine();
            Console.Out.Rewrite(options[i].Key, (default, Shared.Top + i));
        }

        Console.Out.Rewrite(options[0].Key, (default, Shared.Top), Shared.ChosenForegroundColor,
            Shared.ChosenBackgroundColor);
        Shared.CurrentOptions = options;
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
                            sum = attrs.Aggregate(sum,
                                (current, attr) =>
                                    current + (Shared.AttributeLevels.TryGetValue(attr.ToString()!,
                                        out BigInteger bigint)
                                        ? bigint
                                        : BigInteger.Parse(attr.ToString()!)));
                            break;
                        default:
                            sum += BigInteger.Parse(value.ToString()!);
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
                            testWeight = attrs.Aggregate(testWeight,
                                (current, attr) =>
                                    current + (Shared.AttributeLevels.TryGetValue(attr.ToString()!,
                                        out BigInteger bigint)
                                        ? bigint
                                        : BigInteger.Parse(attr.ToString()!)));
                            break;
                        default:
                            testWeight = BigInteger.Parse(weight.ToString()!);
                            break;
                    }

                    if (rand >= (testedWeight += testWeight))
                    {
                        continue;
                    }

                    Perform(optionKey.ToString()!);
                    break;
                }

                break;
            default:
                throw new ArgumentException(option.ToString());
        }
    }
}