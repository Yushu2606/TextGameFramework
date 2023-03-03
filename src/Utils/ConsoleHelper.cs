namespace TextGameFramework.Utils;
internal static class ConsoleHelper
{
    internal static void Write(string text, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, (int Left, int Top) pos)
    {
        Console.SetCursorPosition(pos.Left, pos.Top);
        ConsoleColor defaultBackgroundColor = Console.BackgroundColor;
        ConsoleColor defaultForegroundColor = Console.ForegroundColor;
        Console.BackgroundColor = backgroundColor ?? defaultBackgroundColor;
        Console.ForegroundColor = foregroundColor ?? defaultForegroundColor;
        Console.Write(text);
        Console.BackgroundColor = defaultBackgroundColor;
        Console.ForegroundColor = defaultForegroundColor;
    }
}
