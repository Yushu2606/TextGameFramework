using System.Reflection;
using System.Text;
using TextGameFramework.Functions;
using TextGameFramework.Types;
using TextGameFramework.Utils;
using YamlDotNet.Serialization;

Console.Title = Assembly.GetExecutingAssembly().GetName().Name!;

Console.InputEncoding = Encoding.Unicode;
if (args.Length <= 0)
{
    Console.Write("文件路径 ");
    args = [Console.ReadLine()];
}

string fileText = File.ReadAllText(args[0]);
Shared.GameData = new Deserializer().Deserialize<GameData>(fileText);
Console.Title =
    $"{Shared.GameData.Name}{(OperatingSystem.IsWindows() ? $" · {Console.Title}" : Assembly.GetExecutingAssembly().GetName().Name)}";

if (Shared.GameData.Input is not null)
{
    Shared.Arguments = new string[Shared.GameData.Input.Length];
    for (int i = 0; i < Shared.Arguments.Length; ++i)
    {
        Console.Write($"{Shared.GameData.Input[i]} ");
        Shared.Arguments[i] = Console.ReadLine();
    }
}

_ = Task.Run(() => Plot.Process(Shared.GameData.Init))
    .ContinueWith(task => Environment.FailFast(default, task.Exception), TaskContinuationOptions.OnlyOnFaulted);

int index = default;
while (true)
{
    ConsoleKeyInfo inputKey = Console.ReadKey(true);
    if (Shared.CurrentOptions is null)
    {
        continue;
    }

    switch (Shared.CurrentOptions.Length)
    {
        case <= 0:
            return;
        case < 2:
            {
                KeyValuePair<string, object> cache = Shared.CurrentOptions[0];
                Shared.CurrentOptions = null;
                _ = Task.Run(() => Plot.Process(cache.Value));
                continue;
            }
    }

    switch (inputKey.Key)
    {
        case ConsoleKey.UpArrow:
        case ConsoleKey.DownArrow:
            Console.Out.Rewrite(Shared.CurrentOptions[index].Key, (default, Shared.Top + index));
            break;
    }

    switch (inputKey.Key)
    {
        case ConsoleKey.UpArrow:
            if (index > 0)
            {
                --index;
                break;
            }

            index = Shared.CurrentOptions.Length - 1;
            break;
        case ConsoleKey.DownArrow:
            if (index < (Shared.CurrentOptions.Length - 1))
            {
                ++index;
                break;
            }

            index = 0;
            break;
        case ConsoleKey.Enter:
            {
                KeyValuePair<string, object> cache = Shared.CurrentOptions[index];
                Shared.CurrentOptions = null;
                index = default;
                _ = Task.Run(() => Plot.Process(cache.Value)).ContinueWith(
                    task => Environment.FailFast(default, task.Exception), TaskContinuationOptions.OnlyOnFaulted);
                continue;
            }
        default:
            continue;
    }

    Console.Out.Rewrite(Shared.CurrentOptions[index].Key, (default, Shared.Top + index), Shared.ChosenForegroundColor,
        Shared.ChosenBackgroundColor);
}