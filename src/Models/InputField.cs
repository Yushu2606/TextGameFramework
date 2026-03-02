using TextGameFramework.Utils;

namespace TextGameFramework.Models;

internal record InputField(
    [property: HoconPropertyName("label")]
    string Label,
    [property: HoconPropertyName("required")]
    bool Required);
