using System.Text.Json.Serialization;

namespace Llyn.UIDemeanor.Capsule;

public sealed record LCapsuleSetting(
    [property: JsonPropertyName("theme")] string? LCapsuleSettingTheme = null,
    [property: JsonPropertyName("glyph")] string? LCapsuleSettingGlyph = null,
    [property: JsonPropertyName("ambiguous")] string? LCapsuleSettingAmbiguous = null,
    [property: JsonPropertyName("folder")] string? LCapsuleSettingFolder = null);
