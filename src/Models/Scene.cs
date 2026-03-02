using System.Diagnostics.CodeAnalysis;
using TextGameFramework.Utils;

namespace TextGameFramework.Models;

internal record Scene(
    [property: HoconPropertyName("description")]
    string Description,
    [property: HoconPropertyName("achievements")]
    string[]? Achievements,
    [property: HoconPropertyName("attributes")]
    [DisallowNull]
    Dictionary<string, long>? Attributes,
    [property: HoconPropertyName("choices")]
    [DisallowNull]
    Dictionary<string, Union<string, Dictionary<string, Union<long, string[]>>>>? Choices);
