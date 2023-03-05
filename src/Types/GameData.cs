using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal readonly struct GameData
{
    [YamlMember(Alias = "name")]
    public required string Name { get; init; }
    [YamlMember(Alias = "input")]
    public string[] Input { get; init; }
    [YamlMember(Alias = "init")]
    public required string Init { get; init; }
    [YamlMember(Alias = "processes")]
    public required Dictionary<string, Process> Processes { get; init; }
    [YamlMember(Alias = "achievements")]
    public Dictionary<string, Achievement> Achievements { get; init; }
    [YamlMember(Alias = "attributes")]
    public Dictionary<string, Attribute> Attributes { get; init; }
}
