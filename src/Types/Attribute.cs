using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal struct Attribute
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; }
}
