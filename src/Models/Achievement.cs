using TextGameFramework.Utils;

namespace TextGameFramework.Models;

internal record Achievement(
    [property: HoconPropertyName("name")]
    string Name,
    [property: HoconPropertyName("scene")]
    string? Scene,
    [property: HoconPropertyName("hide")]
    bool Hide);
