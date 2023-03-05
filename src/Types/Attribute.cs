using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal readonly struct Attribute
{
    [YamlMember(Alias = "name")]
    public string Name { get; init; }
    [YamlMember(Alias = "should_be_shown")]
    public required bool ShouldBeShown { get; init; }
}
