namespace TextGameFramework.Functions;

internal sealed class NarrativePresentedEventArgs(string text) : EventArgs
{
    public string Text { get; } = text;
}
