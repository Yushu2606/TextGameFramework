using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using TextGameFramework.Views;

ConfigurationManager.Enable(ConfigLocations.All);
Application.Create().Run<MainView>().Dispose();
