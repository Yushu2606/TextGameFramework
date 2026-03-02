using TextGameFramework.Models;

namespace TextGameFramework.Functions;

internal interface IGameEngine
{
    string Name { get; }
    Dictionary<string, InputField>? InputDefinitions { get; }
    IReadOnlyDictionary<string, GameAttribute>? AttributeDefinitions { get; }
    IReadOnlyDictionary<string, Achievement>? AchievementDefinitions { get; }
    IReadOnlyDictionary<string, long> AttributeValues { get; }
    IReadOnlyCollection<string> EarnedAchievements { get; }

    void Start();
    void ChooseByTitle(string title);
    void SetInput(string key, string value);

    event EventHandler<ScenePresentedEventArgs>? ScenePresented;
    event EventHandler<NarrativePresentedEventArgs>? NarrativePresented;
}
