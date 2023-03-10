using System.Reflection;
using System.Text;
using TextGameFramework.Functions;
using TextGameFramework.Types;
using TextGameFramework.Utils;
using YamlDotNet.Serialization;

Console.Title = Assembly.GetExecutingAssembly().GetName().Name;

Console.InputEncoding = Encoding.Unicode;
if (args.Length <= 0)
{
    Console.Write("文件路径 ");
    args = new[] { Console.ReadLine() };
}

string fileText = File.ReadAllText(args[0]);
PublicData.Gamedata = new Deserializer().Deserialize<GameData>(fileText);
Console.Title = $"{PublicData.Gamedata.Name} · {Console.Title}";

if (PublicData.Gamedata.Input is not null)
{
    PublicData.Gamedata.Arguments = new string[PublicData.Gamedata.Input.Length];
    for (int i = 0; i < PublicData.Gamedata.Arguments.Length; ++i)
    {
        Console.Write($"{PublicData.Gamedata.Input[i]} ");
        PublicData.Gamedata.Arguments[i] = Console.ReadLine();
    }
}

Task.Run(() => Plot.Process(PublicData.Gamedata.Init)).ContinueWith(task =>
{
    Console.WriteLine(task.Exception);
    Environment.Exit(task.Exception.HResult);
}, TaskContinuationOptions.OnlyOnFaulted);

int index = default;
while (true)
{
    ConsoleKeyInfo inputKey = Console.ReadKey(true);
    if (PublicData.Gamedata.CurrectOptions is null)
    {
        continue;
    }
    else if (PublicData.Gamedata.CurrectOptions.Length <= 0)
    {
        return;
    }
    else if (PublicData.Gamedata.CurrectOptions.Length < 2)
    {
        KeyValuePair<string, object> cache = PublicData.Gamedata.CurrectOptions[0];
        PublicData.Gamedata.CurrectOptions = null;
        Task.Run(() => Plot.Process(cache.Value));
        continue;
    }
    switch (inputKey.Key)
    {
        case ConsoleKey.UpArrow:
        case ConsoleKey.DownArrow:
            Console.Out.Rewrite(PublicData.Gamedata.CurrectOptions[index].Key, (default, PublicData.Top + index));
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
            index = PublicData.Gamedata.CurrectOptions.Length - 1;
            break;
        case ConsoleKey.DownArrow:
            if (index < PublicData.Gamedata.CurrectOptions.Length - 1)
            {
                ++index;
                break;
            }
            index = 0;
            break;
        case ConsoleKey.Enter:
            KeyValuePair<string, object> cache = PublicData.Gamedata.CurrectOptions[index];
            PublicData.Gamedata.CurrectOptions = null;
            index = default;
            Task.Run(() => Plot.Process(cache.Value)).ContinueWith(task =>
            {
                Console.WriteLine(task.Exception);
                Environment.Exit(task.Exception.HResult);
            }, TaskContinuationOptions.OnlyOnFaulted);
            continue;
        default:
            continue;
    }
    Console.Out.Rewrite(PublicData.Gamedata.CurrectOptions[index].Key, (default, PublicData.Top + index), PublicData.choosenForegroundColor, PublicData.choosenBackgroundColor);
}
