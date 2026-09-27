using System.Text.Json.Serialization;

namespace Llyn.UIDeportment.Capsule;

public sealed record LCapsuleColumn(
    [property: JsonPropertyName("tab")] string LCapsuleColumnTab,
    [property: JsonPropertyName("left")] double? LCapsuleColumnLeft,
    [property: JsonPropertyName("middle")] double? LCapsuleColumnMiddle);
