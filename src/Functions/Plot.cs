using SmartFormat;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using TextGameFramework.Models;
using TextGameFramework.Utils;

namespace TextGameFramework.Functions;

internal sealed class Plot : IGameEngine
{
    private readonly GameData _gameData;
    private readonly Dictionary<string, Union<string, Dictionary<string, Union<long, string[]>>>> _choicesByTitle = [];
    private readonly ConcurrentDictionary<string, long> _attributeValues = [];
    private readonly ConcurrentBag<string> _earnedAchievements = [];

    internal Plot(GameData gameData)
    {
        _gameData = gameData;
        InitializeAttributes();
    }

    public string Name => _gameData.Name;

    [MaybeNull]
    public Dictionary<string, InputField> InputDefinitions => _gameData.Inputs;

    [MaybeNull]
    public IReadOnlyDictionary<string, Achievement>? AchievementDefinitions => _gameData.Achievements;

    [MaybeNull]
    public IReadOnlyDictionary<string, GameAttribute>? AttributeDefinitions => _gameData.Attributes;

    public Dictionary<string, string> InputValues { get; } = [];
    public IReadOnlyCollection<string> EarnedAchievements => [.. _earnedAchievements];
    public IReadOnlyDictionary<string, long> AttributeValues => new ReadOnlyDictionary<string, long>(_attributeValues);
    public event EventHandler<ScenePresentedEventArgs>? ScenePresented;
    public event EventHandler<NarrativePresentedEventArgs>? NarrativePresented;

    public void Start() => ApplyChoice(_gameData.InitScene);

    public void ApplyChoice(Union<string, Dictionary<string, Union<long, string[]>>> choice)
    {
        switch (choice.Value)
        {
            case null:
                break;
            case string sceneName:
                {
                    Scene scene = GetScene(sceneName);
                    ApplySceneEffects(scene);
                    PresentScene(scene);
                    break;
                }
            case Dictionary<string, Union<long, string[]>> dic:
                {
                    if (dic.Count <= 0)
                    {
                        break;
                    }

                    string sceneName = SelectSceneByWeight(dic);
                    Scene scene = GetScene(sceneName);
                    ApplySceneEffects(scene);
                    PresentScene(scene);
                    break;
                }
        }
    }

    private long ResolveChoiceWeight(Union<long, string[]> weight) => weight.Value switch
    {
        string[] attrs => Math.Max(0,
            attrs.Aggregate(0L,
                (current, attr) => _attributeValues.TryGetValue(attr, out long num) ? current + num : current)),
        long v => Math.Max(0, v),
        _ => 0
    };

    private string SelectSceneByWeight(Dictionary<string, Union<long, string[]>> sceneWeights)
    {
        long sum = sceneWeights.Values.Aggregate(0L, (acc, v) => acc + ResolveChoiceWeight(v));
        if (sum <= 0)
        {
            return sceneWeights.First().Key;
        }

        long rand = Random.Shared.NextInt64(sum);
        long tested = 0;
        foreach ((string optionKey, Union<long, string[]> weight) in sceneWeights)
        {
            tested += ResolveChoiceWeight(weight);

            if (rand >= tested)
            {
                continue;
            }

            return optionKey;
        }

        return sceneWeights.First().Key;
    }

    public Scene GetScene(string sceneName) => _gameData.Scenes[sceneName];

    public void ChooseByTitle(string title)
    {
        if (!_choicesByTitle.TryGetValue(title, out Union<string, Dictionary<string, Union<long, string[]>>>? choice))
        {
            return;
        }

        ApplyChoice(choice);
    }

    public void SetInput(string key, string value)
    {
        InputValues[key] = value;
    }

    private void InitializeAttributes()
    {
        if (_gameData.Attributes is null)
        {
            return;
        }

        foreach ((string key, GameAttribute def) in _gameData.Attributes)
        {
            long initial = def.Default ?? def.Min ?? 0;
            _attributeValues[key] = initial;
        }
    }

    private void ApplySceneEffects(Scene scene)
    {
        if (scene.Attributes is not null && AttributeDefinitions is not null)
        {
            foreach ((string attributesKey, long level) in scene.Attributes)
            {
                if (!_attributeValues.TryGetValue(attributesKey, out long oldLevel))
                {
                    continue;
                }

                long newLevel = oldLevel + level;
                if (!AttributeDefinitions.TryGetValue(attributesKey, out GameAttribute? attribute))
                {
                    continue;
                }

                if (attribute.Min.HasValue && newLevel < attribute.Min.Value)
                {
                    newLevel = attribute.Min.Value;
                }

                if (attribute.Max.HasValue && newLevel > attribute.Max.Value)
                {
                    newLevel = attribute.Max.Value;
                }

                _attributeValues[attributesKey] = newLevel;
                if (attribute.Scene is null)
                {
                    continue;
                }

                Scene nextScene = GetScene(attribute.Scene);
                string text = FormatDescription(nextScene.Description, NarrativeKind.AttributeChanged, attribute.Name,
                    _attributeValues[attributesKey]);
                NarrativePresented?.Invoke(this, new(text));
            }
        }

        if (scene.Achievements is not null && AchievementDefinitions is not null)
        {
            foreach (string achievementKey in scene.Achievements)
            {
                if (!AchievementDefinitions.TryGetValue(achievementKey, out Achievement? achievement))
                {
                    continue;
                }

                if (_earnedAchievements.Contains(achievement.Name))
                {
                    continue;
                }

                _earnedAchievements.Add(achievement.Name);
                if (achievement.Scene is null)
                {
                    continue;
                }

                Scene nextScene = GetScene(achievement.Scene);
                string text = FormatDescription(nextScene.Description, NarrativeKind.AchievementEarned,
                    achievementName: achievement.Name);
                NarrativePresented?.Invoke(this, new(text));
            }
        }
    }

    private void PresentScene(Scene scene)
    {
        _choicesByTitle.Clear();
        if (scene.Choices is not null)
        {
            foreach ((string key, Union<string, Dictionary<string, Union<long, string[]>>> value) in scene.Choices)
            {
                _choicesByTitle[key] = value;
            }
        }

        string text = FormatDescription(scene.Description);
        IReadOnlyList<string> choices = [.. _choicesByTitle.Keys];
        ScenePresented?.Invoke(this, new(text, choices));
    }

    private string FormatDescription(string description, NarrativeKind? kind = null, string? attributeName = null,
        long? attributeValue = null, string? achievementName = null)
    {
        Dictionary<string, string> args = new(InputValues, StringComparer.Ordinal);
        foreach ((string key, string value) in ComposeArgs(kind, attributeName, attributeValue, achievementName))
        {
            args[key] = value;
        }

        return Smart.Format(description, args);
    }

    private static Dictionary<string, string> ComposeArgs(NarrativeKind? kind, string? attributeName,
        long? attributeValue, string? achievementName)
        => kind switch
        {
            NarrativeKind.AttributeChanged => new()
            {
                ["attribute"] = attributeName ?? string.Empty,
                ["level"] = attributeValue.GetValueOrDefault().ToString()
            },
            NarrativeKind.AchievementEarned => new()
            {
                ["achievement"] = achievementName ?? string.Empty
            },
            _ => []
        };

}
