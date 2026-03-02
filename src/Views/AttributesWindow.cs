using System.Collections.ObjectModel;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TextGameFramework.Functions;
using TextGameFramework.Models;

namespace TextGameFramework.Views;

internal class AttributesWindow : Window
{
    public AttributesWindow(IGameEngine engine)
    {
        Title = "属性";

        ListView list = new()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        ObservableCollection<string> items = new(
        (
            engine.AttributeDefinitions is null || engine.AttributeValues.Count <= 0
        )
            ? ["暂无属性"]
            : engine.AttributeValues
                .OrderBy(kv => kv.Key)
                .Select(kv =>
                {
                    string key = kv.Key;
                    long val = kv.Value;
                    string name = engine.AttributeDefinitions.TryGetValue(key, out GameAttribute? def) ? def.Name : key;
                    return $"{name}：{val}";
                })
                .ToArray()
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
