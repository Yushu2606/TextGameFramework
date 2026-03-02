using Hocon;
using TextGameFramework.Models;
using TextGameFramework.Utils;

namespace TextGameFramework.Functions;

internal sealed class DefaultGameFactory : IGameFactory
{
    public async Task<IGameEngine> CreateFromFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        string fileText = await File.ReadAllTextAsync(filePath, cancellationToken);
        return CreateFromText(fileText);
    }

    public IGameEngine CreateFromText(string plotText)
    {
        HoconRoot hocon = HoconParser.Parse(plotText);
        GameData gameData = hocon.As<GameData>();
        return new Plot(gameData);
    }
}
