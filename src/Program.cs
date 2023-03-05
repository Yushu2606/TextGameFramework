using System.Reflection;
using System.Text;
using TextGameFramework.Functions;
using TextGameFramework.Types;
using TextGameFramework.Utils;
using YamlDotNet.Serialization;

Console.InputEncoding = Encoding.Unicode;
Console.CursorVisible = false;

Console.Title = Assembly.GetExecutingAssembly().GetName().Name;
Console.Write("文件目录: ");
string filePath = args.Length is 1 ? args[0] : Console.ReadLine();
Console.Clear();

string fileText = File.ReadAllText(filePath);
PublicData.Gamedata = new Deserializer().Deserialize<GameData>(fileText);
Console.Title = $"{PublicData.Gamedata.Name} · {Console.Title}";

if (PublicData.Gamedata.Input is null)
{
    Plot.Perform(PublicData.Gamedata.Init);
    return;
}
string[] @params = new string[PublicData.Gamedata.Input.Length];
for (int i = 0; i < @params.Length; ++i)
{
    Console.Write($"{PublicData.Gamedata.Input[i]}: ");
    @params[i] = Console.ReadLine();
}

Plot.Perform(PublicData.Gamedata.Init, @params);
