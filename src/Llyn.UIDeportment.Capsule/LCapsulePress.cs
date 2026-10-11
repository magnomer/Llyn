using System.Text.Json.Serialization;

namespace Llyn.UIDeportment.Capsule;

public sealed record LCapsulePress(
    [property: JsonPropertyName("width")] double LCapsulePressWidth,
    [property: JsonPropertyName("height")] double LCapsulePressHeight);
