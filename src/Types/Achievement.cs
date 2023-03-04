using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal readonly struct Achievement
{
    [YamlMember(Alias = "name")]
    public required string Name { get; init; }
}
