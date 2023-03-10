using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal readonly record struct Attribute
{
    [YamlMember(Alias = "name")]
    public string Name { get; init; }
    [YamlMember(Alias = "message_key")]
    public string MessageKey { get; init; }
}
