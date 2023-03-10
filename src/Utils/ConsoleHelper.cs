namespace TextGameFramework.Utils;

internal static class ConsoleExtension
{
    internal static void Rewrite(this TextWriter writer, string text, (int Left, int Top) pos = default, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
    {
        Console.SetCursorPosition(pos.Left, pos.Top);
        ConsoleColor defaultBackgroundColor = Console.BackgroundColor;
        ConsoleColor defaultForegroundColor = Console.ForegroundColor;
        Console.BackgroundColor = backgroundColor ?? defaultBackgroundColor;
        Console.ForegroundColor = foregroundColor ?? defaultForegroundColor;
        writer.Write(text);
        Console.BackgroundColor = defaultBackgroundColor;
        Console.ForegroundColor = defaultForegroundColor;
    }
    internal static void WritePerChar(this TextWriter writer, string @string, int timeout = 0)
    {
        foreach (char @char in @string)
        {
            writer.Write(@char);
            Thread.Sleep(timeout);
        }
    }
}
