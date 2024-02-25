using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal record GameData(
    [property: YamlMember(Alias = "name")] string Name,
    [property: YamlMember(Alias = "input")]
    string[]? Input,
    [property: YamlMember(Alias = "init")] string Init,
    [property: YamlMember(Alias = "processes")]
    Dictionary<string, Process> Processes,
    [property: YamlMember(Alias = "achievements")]
    Dictionary<string, Achievement>? Achievements,
    [property: YamlMember(Alias = "attributes")]
    Dictionary<string, Attribute>? Attributes);