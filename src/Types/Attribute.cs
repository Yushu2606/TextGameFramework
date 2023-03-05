using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal readonly struct Attribute
{
    [YamlMember(Alias = "name")]
    public string Name { get; init; }
    [YamlMember(Alias = "message_key")]
    public string MessageKey { get; init; }
}
