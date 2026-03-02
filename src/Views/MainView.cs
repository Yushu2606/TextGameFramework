using Hocon;
using System.Reflection;
using System.Text.Json;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TextGameFramework.Functions;

namespace TextGameFramework.Views;

internal class MainView : Runnable
{
    private Window? _currentWindow;
    private readonly IGameFactory _gameFactory;

    public MainView() : this(null) { }
    public MainView(IGameFactory? gameFactory = null)
    {
        _gameFactory = gameFactory ?? new DefaultGameFactory();
        Title = Assembly.GetExecutingAssembly().GetName().Name ?? "TextGameFramework";

        MenuBar menuBar = new([
            new()
            {
                Title = "文件(_F)",
                PopoverMenu = new([
                    new MenuBarItem
                    {
                        Title = "打开(_O)…",
                        Action = async () =>
                        {
                            OpenDialog dialog = new()
                            {
                                AllowedTypes = [new AllowedType("Text Game Files", ".tgf")],
                                OpenMode = OpenMode.File,
                                AllowsMultipleSelection = false
                            };
                            App!.Run(dialog);

                            if (dialog.Canceled || !dialog.FilePaths.Any())
                            {
                                return;
                            }

                            string? path = dialog.FilePaths[0];
                            IGameEngine engine;
                            try
                            {
                                engine = await _gameFactory.CreateFromFileAsync(path);
                            }
                            catch (HoconException)
                            {
                                MessageBox.ErrorQuery(App, "加载失败", "抱歉，这不是一个有效的游戏文件", "好的");
                                return;
                            }
                            catch (JsonException)
                            {
                                MessageBox.ErrorQuery(App, "加载失败", "抱歉，这可能是一个过时的游戏文件", "好的");
                                return;
                            }
                            catch (Exception)
                            {
                                MessageBox.ErrorQuery(App, "加载失败", "抱歉，我们遇到了未知的问题", "好的");
                                return;
                            }

                            if (engine.InputDefinitions is not null && engine.InputDefinitions.Count > 0)
                            {
                                RequestInputWindow inputWindow = new(engine)
                                {
                                    X = 0,
                                    Y = 1,
                                    Width = Dim.Fill(),
                                    Height = Dim.Fill()
                                };
                                inputWindow.Completed += (sender, e) =>
                                {
                                    GameWindow gw = new(engine)
                                    {
                                        X = 0,
                                        Y = 1,
                                        Width = Dim.Fill(),
                                        Height = Dim.Fill()
                                    };
                                    Add(gw);
                                    _currentWindow = gw;
                                };
                                Add(inputWindow);
                                _currentWindow = inputWindow;
                                return;
                            }

                            GameWindow window = new(engine)
                            {
                                X = 0,
                                Y = 1,
                                Width = Dim.Fill(),
                                Height = Dim.Fill()
                            };
                            Add(window);
                            _currentWindow = window;
                        }
                    },
                    new MenuBarItem
                    {
                        Title = "关闭(_C)",
                        Action = () =>
                        {
                            if (_currentWindow is null)
                            {
                                return;
                            }

                            Remove(_currentWindow);
                            _currentWindow.Dispose();
                            _currentWindow = null;
                        }
                    },
                    new Line(),
                    new MenuBarItem
                    {
                        Title = "退出(_X)",
                        Action = () => App!.RequestStop()
                    }
                ])
            },
            new()
            {
                Title = "游戏(_G)",
                PopoverMenu = new([
                    new MenuBarItem
                    {
                        Title = "属性(_A)",
                        Action = () =>
                        {
                            if (_currentWindow is not GameWindow gw)
                            {
                                return;
                            }

                            AttributesWindow w = new(gw.Engine)
                            {
                                X = 0,
                                Y = 1,
                                Width = Dim.Fill(),
                                Height = Dim.Fill()
                            };
                            Add(w);
                        }
                    },
                    new MenuBarItem
                    {
                        Title = "成就(_J)",
                        Action = () =>
                        {
                            if (_currentWindow is not GameWindow gw)
                            {
                                return;
                            }

                            AchievementsWindow w = new(gw.Engine)
                            {
                                X = 0,
                                Y = 1,
                                Width = Dim.Fill(),
                                Height = Dim.Fill()
                            };
                            Add(w);
                        }
                    }
                ])
            }
        ]);

        Add(menuBar);
    }
}
