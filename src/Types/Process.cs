using System.Numerics;
using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal record Process
{
    [YamlMember(Alias = "description")]
    public required string Description { get; init; }
    [YamlMember(Alias = "achievements")]
    public string[] Achievements { get; init; }
    [YamlMember(Alias = "attributes")]
    public Dictionary<string, BigInteger> Attributes { get; init; }
    [YamlMember(Alias = "options")]
    public Dictionary<string, object> Options { get; init; }
}
