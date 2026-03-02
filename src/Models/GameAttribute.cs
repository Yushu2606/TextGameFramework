using System.Diagnostics.CodeAnalysis;
using TextGameFramework.Utils;

namespace TextGameFramework.Models;

internal record GameAttribute(
    [property: HoconPropertyName("name")]
    string Name,
    [property: HoconPropertyName("scene")]
    [DisallowNull]
    string? Scene,
    [property: HoconPropertyName("max")]
    [DisallowNull]
    long? Max,
    [property: HoconPropertyName("min")]
    [DisallowNull]
    long? Min,
    [property: HoconPropertyName("default")]
    [DisallowNull]
    long? Default);
