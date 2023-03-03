using YamlDotNet.Serialization;

namespace TextGameFramework.Types;

internal struct Achievement
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; }
}
