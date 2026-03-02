using System.Collections.Concurrent;
using System.Text;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TextGameFramework.Functions;

namespace TextGameFramework.Views;

internal class GameWindow : Window
{
    private const int TypewriterDelay = 50;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly View _choicesPanel;
    private readonly TextView _narrativeTextView;
    private readonly ConcurrentQueue<(string Text, bool RevealChoicesAfter)> _typewriterQueue = [];
    private readonly AutoResetEvent _typewriterSignal = new(false);
    private List<string> _choiceTitles = [];
    private volatile bool _isTyping;
    private volatile bool _typewriterSkip;

    public GameWindow(IGameEngine engine)
    {
        Engine = engine;
        Initialized += (sender, e) => Engine.Start();
        Disposing += async (sender, e) =>
        {
            Engine.ScenePresented -= OnScenePresented;
            Engine.NarrativePresented -= OnNarrativePresented;
            await _cancellationTokenSource.CancelAsync();
            _typewriterSignal.Set();
        };
        Engine.ScenePresented += OnScenePresented;
        Engine.NarrativePresented += OnNarrativePresented;
        Task.Factory.StartNew(TypewriterLoop, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning,
            TaskScheduler.Default).ConfigureAwait(false);

        Title = Engine.Name;

        _narrativeTextView = new()
        {
            ReadOnly = true
        };
        _choicesPanel = new();

        _narrativeTextView.X = 0;
        _narrativeTextView.Y = 0;
        _narrativeTextView.Width = Dim.Fill();
        _narrativeTextView.Height = 12;

        _choicesPanel.X = 0;
        _choicesPanel.Y = Pos.Bottom(_narrativeTextView) + 1;
        _choicesPanel.Width = Dim.Fill();
        _choicesPanel.Height = Dim.Fill();

        Add(_narrativeTextView, _choicesPanel);
    }

    public IGameEngine Engine { get; }

    private void OnScenePresented(object? sender, ScenePresentedEventArgs e)
    {
        StartTypewriter(e.Text, e.Choices.Count > 1);
        if (e.Choices.Count <= 0)
        {
            _choiceTitles = [];
            _choicesPanel.RemoveAll();
            _choicesPanel.Visible = false;
            return;
        }

        if (e.Choices.Count < 2)
        {
            string only = e.Choices[0];
            Engine.ChooseByTitle(only);
            return;
        }

        _choicesPanel.RemoveAll();
        _choicesPanel.Visible = false;
        _choiceTitles = [.. e.Choices];
    }

    private void OnNarrativePresented(object? sender, NarrativePresentedEventArgs e)
    {
        StartTypewriter(e.Text);
    }

    private void StartTypewriter(string text, bool revealChoicesAfter = false)
    {
        _typewriterSkip = false;
        _typewriterQueue.Enqueue((text, revealChoicesAfter));
        if (_isTyping)
        {
            return;
        }

        _typewriterSignal.Set();
    }

    private void EndTypingIfRunning()
    {
        if (!_isTyping)
        {
            return;
        }

        _typewriterSkip = true;
    }

    private void TypewriterLoop()
    {
        WaitHandle[] waits = [_typewriterSignal, _cancellationTokenSource.Token.WaitHandle];
        while (true)
        {
            int signaled = WaitHandle.WaitAny(waits);
            if (signaled is 1) // cancellation requested
            {
                break;
            }

            if (_typewriterQueue.IsEmpty)
            {
                Thread.Yield();
                continue;
            }

            StringBuilder text = new();
            App?.Invoke(() => _narrativeTextView.Text = text.ToString());
            while (_typewriterQueue.TryDequeue(out (string Text, bool RevealChoicesAfter) item))
            {
                _isTyping = true;
                Thread.Sleep(TypewriterDelay * 13);
                foreach (char ch in item.Text)
                {
                    text.Append(ch);
                    App?.Invoke(() => _narrativeTextView.Text = text.ToString());
                    if (!_typewriterSkip)
                    {
                        Thread.Sleep(TypewriterDelay);
                    }
                }

                text.Append(Environment.NewLine);
                App?.Invoke(() => _narrativeTextView.Text = text.ToString());
                _isTyping = false;
                _typewriterSkip = false;
                if (item.RevealChoicesAfter)
                {
                    Thread.Sleep(TypewriterDelay * 5);
                    App?.Invoke(BuildAndShowChoices);
                }
            }
        }
    }

    private void BuildAndShowChoices()
    {
        _choicesPanel.RemoveAll();
        int row = 0;
        foreach (string title in _choiceTitles)
        {
            Button btn = new()
            {
                Text = title,
                X = Pos.Center(),
                Y = row
            };
            btn.Accepting += (s, e2) =>
            {
                EndTypingIfRunning();
                Engine.ChooseByTitle(title);
            };
            _choicesPanel.Add(btn);
            row += 2;
        }

        _choicesPanel.Visible = true;
    }
}
