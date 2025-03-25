using System.Numerics;
using VYaml.Annotations;

namespace TextGameFramework.Types;

internal record Process(
    [property: YamlMember("description")]
    string Description,
    [property: YamlMember("achievements")]
    string[]? Achievements,
    [property: YamlMember("attributes")]
    Dictionary<string, BigInteger>? Attributes,
    [property: YamlMember("options")]
    Dictionary<string, object>? Options);