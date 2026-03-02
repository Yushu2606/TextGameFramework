using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TextGameFramework.Functions;
using TextGameFramework.Models;

namespace TextGameFramework.Views;

internal class RequestInputWindow : Window
{
    private readonly Dictionary<string, TextField> _fields = [];
    private readonly IGameEngine _engine;

    public RequestInputWindow(IGameEngine engine)
    {
        _engine = engine;
        Title = "输入信息";

        int row = 0;
        if (_engine.InputDefinitions is not null)
        {
            foreach ((string key, InputField input) in _engine.InputDefinitions)
            {
                Label label = new()
                {
                    Text = input.Required ? $"{input.Label} *" : input.Label,
                    Y = row
                };
                TextField field = new()
                {
                    Width = Dim.Fill(),
                    X = 0,
                    Y = row + 1
                };
                _fields[key] = field;
                Add(label);
                Add(field);
                row += 2;
            }
        }

        Button ok = new()
        {
            Text = "开始",
            Y = row + 1
        };
        ok.Accepting += (sender, e) =>
        {
            if (_engine.InputDefinitions is not null)
            {
                foreach ((string key, InputField input) in _engine.InputDefinitions)
                {
                    TextField tf = _fields[key];
                    if (!input.Required || !string.IsNullOrWhiteSpace(tf.Text))
                    {
                        continue;
                    }

                    MessageBox.ErrorQuery(App!, "提示", $"必填项“{input.Label}”未填", "好的");
                    tf.SetFocus();
                    return;
                }
            }

            foreach ((string key, TextField tf) in _fields)
            {
                _engine.SetInput(key, tf.Text);
            }

            Completed?.Invoke(this, EventArgs.Empty);
            SuperView?.Remove(this);
        };

        Add(ok);
    }

    public event EventHandler? Completed;
}
