using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal readonly struct Attribute
{
    [YamlMember(Alias = "name")]
    public required string Name { get; init; }
}
