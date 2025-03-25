using VYaml.Annotations;

namespace TextGameFramework.Types;

internal record GameData(
    [property: YamlMember("name")] string Name,
    [property: YamlMember("input")]
    string[]? Input,
    [property: YamlMember("init")] string Init,
    [property: YamlMember("processes")]
    Dictionary<string, Process> Processes,
    [property: YamlMember("achievements")]
    Dictionary<string, Achievement>? Achievements,
    [property: YamlMember("attributes")]
    Dictionary<string, Attribute>? Attributes);