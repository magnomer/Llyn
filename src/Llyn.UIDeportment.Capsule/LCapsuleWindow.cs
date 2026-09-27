using System.Text.Json.Serialization;

namespace Llyn.UIDeportment.Capsule;

public sealed record LCapsuleWindow(
    [property: JsonPropertyName("left")] double LCapsuleWindowLeft,
    [property: JsonPropertyName("top")] double LCapsuleWindowTop,
    [property: JsonPropertyName("width")] double LCapsuleWindowWidth,
    [property: JsonPropertyName("height")] double LCapsuleWindowHeight,
    [property: JsonPropertyName("maximized")] bool LCapsuleWindowMaximized);
