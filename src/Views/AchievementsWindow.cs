using System.Collections.ObjectModel;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TextGameFramework.Functions;

namespace TextGameFramework.Views;

internal class AchievementsWindow : Window
{
    public AchievementsWindow(IGameEngine engine)
    {
        Title = "成就";

        ListView list = new()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        ObservableCollection<string> items = new(
        (
            engine.AchievementDefinitions is null || engine.AchievementDefinitions.Count <= 0
        )
            ? ["暂无成就"]
            : (
                engine.AchievementDefinitions.Values
                    .Where(a => !a.Hide || engine.EarnedAchievements.Contains(a.Name))
                    .OrderBy(a => a.Hide)
                    .ThenBy(a => a.Name)
                    .Select(a => $"{a.Name}{(engine.EarnedAchievements.Contains(a.Name) ? "（已获得）" : string.Empty)}")
                    .ToArray()
            ) is { Length: > 0 } visible ? visible : ["暂无成就"]
        );

        list.SetSource(items);
        Add(list);

        Button close = new()
        {
            Text = "关闭"
        };
        close.Accepting += (s, e) => SuperView?.Remove(this);
        close.Y = Pos.Bottom(list) - 1;
        close.X = Pos.Center();
        Add(close);
    }
}
