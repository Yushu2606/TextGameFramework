using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal struct Process
{
    [YamlMember(Alias = "description")]
    public string Description { get; set; }
    [YamlMember(Alias = "achievements")]
    public string[] Achievements { get; set; }
    [YamlMember(Alias = "attributes")]
    public Dictionary<string, int> Attributes { get; set; }
    [YamlMember(Alias = "options")]
    public Dictionary<string, string> Options { get; set; }
}
