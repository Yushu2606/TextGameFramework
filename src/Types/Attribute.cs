using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal record Attribute(
    [property: YamlMember(Alias = "name")] string Name,
    [property: YamlMember(Alias = "message_key")]
    string? MessageKey);