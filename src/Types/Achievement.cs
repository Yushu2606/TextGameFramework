using VYaml.Annotations;

namespace TextGameFramework.Types;

internal record Achievement(
    [property: YamlMember("name")] string Name,
    [property: YamlMember("message_key")]
    string? MessageKey);