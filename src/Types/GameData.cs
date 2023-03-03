using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal struct GameData
{
    [YamlMember(Alias = "name")]
    public required string Name { get; set; }
    [YamlMember(Alias = "input")]
    public string[] Input { get; set; }
    [YamlMember(Alias = "processes")]
    public required Dictionary<string, Process> Processes { get; set; }
    [YamlMember(Alias = "achievements")]
    public Dictionary<string, Achievement> Achievements { get; set; }
    [YamlMember(Alias = "attributes")]
    public Dictionary<string, Attribute> Attributes { get; set; }
}
