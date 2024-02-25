using System.Numerics;
using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal record Process(
    [property: YamlMember(Alias = "description")]
    string Description,
    [property: YamlMember(Alias = "achievements")]
    string[]? Achievements,
    [property: YamlMember(Alias = "attributes")]
    Dictionary<string, BigInteger>? Attributes,
    [property: YamlMember(Alias = "options")]
    Dictionary<string, object>? Options);