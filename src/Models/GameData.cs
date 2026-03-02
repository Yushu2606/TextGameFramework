using System.Diagnostics.CodeAnalysis;
using TextGameFramework.Utils;

namespace TextGameFramework.Models;

internal record GameData(
    [property: HoconPropertyName("name")]
    string Name,
    [property: HoconPropertyName("init_scene")]
    string InitScene,
    [property: HoconPropertyName("inputs")]
    [DisallowNull]
    Dictionary<string, InputField>? Inputs,
    [property: HoconPropertyName("scenes")]
    Dictionary<string, Scene> Scenes,
    [property: HoconPropertyName("achievements")]
    [DisallowNull]
    Dictionary<string, Achievement>? Achievements,
    [property: HoconPropertyName("attributes")]
    [DisallowNull]
    Dictionary<string, GameAttribute>? Attributes);
