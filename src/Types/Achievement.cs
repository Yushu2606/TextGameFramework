using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal readonly record struct Achievement
{
    [YamlMember(Alias = "name")]
    public required string Name { get; init; }
    [YamlMember(Alias = "message_key")]
    public required string MessageKey { get; init; }
}
