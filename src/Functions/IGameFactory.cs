namespace TextGameFramework.Functions;

internal interface IGameFactory
{
    Task<IGameEngine> CreateFromFileAsync(string filePath, CancellationToken cancellationToken = default);
    IGameEngine CreateFromText(string plotText);
}
