namespace TextGameFramework.Utils;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class HoconPropertyNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
