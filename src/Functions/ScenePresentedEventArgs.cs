namespace TextGameFramework.Functions;

internal sealed class ScenePresentedEventArgs(string text, IReadOnlyList<string> choices) : EventArgs
{
    public string Text { get; } = text;
    public IReadOnlyList<string> Choices { get; } = choices;
}
