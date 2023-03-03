using TextGameFramework.Functions;
using TextGameFramework.Types;
using TextGameFramework.Utils;
using YamlDotNet.Serialization;

Console.InputEncoding = System.Text.Encoding.Unicode;

Console.Title = PublicData.programName;
Console.Write("请输入游戏文件目录：");
string filePath = Console.ReadLine();
Console.Clear();
string fileText = File.ReadAllText(filePath);
PublicData.Gamedata = new Deserializer().Deserialize<GameData>(fileText);
Console.Title = $"{PublicData.Gamedata.Name} · 由{PublicData.programName}驱动";
string[] @params = new string[PublicData.Gamedata.Input.Length];
for (int i = 0; i < @params.Length; ++i)
{
    Console.Write($"请输入{PublicData.Gamedata.Input[i]}：");
    @params[i] = Console.ReadLine();
}
Plot.Handle("main", @params);
