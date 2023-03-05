using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal readonly struct Achievement
{
    [YamlMember(Alias = "name")]
    public required string Name { get; init; }
    [YamlMember(Alias = "message_key")]
    public string MessageKey { get; init; }
}
