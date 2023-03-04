using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal readonly struct Process
{
    [YamlMember(Alias = "description")]
    public required string Description { get; init; }
    [YamlMember(Alias = "achievements")]
    public string[] Achievements { get; init; }
    [YamlMember(Alias = "attributes")]
    public Dictionary<string, int> Attributes { get; init; }
    [YamlMember(Alias = "options")]
    public Dictionary<string, string> Options { get; init; }
}
