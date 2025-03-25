using VYaml.Annotations;

namespace TextGameFramework.Types;

internal record Attribute(
    [property: YamlMember("name")] string Name,
    [property: YamlMember("message_key")]
    string? MessageKey);