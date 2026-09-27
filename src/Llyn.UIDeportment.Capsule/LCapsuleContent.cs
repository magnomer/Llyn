using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Llyn.UIDeportment.Capsule;

public sealed record LCapsuleContent(
    [property: JsonPropertyName("window")] LCapsuleWindow? LCapsuleContentWindow = null,
    [property: JsonPropertyName("linked")] bool LCapsuleContentLinked = true,
    [property: JsonPropertyName("columns")] IReadOnlyList<LCapsuleColumn>? LCapsuleContentColumn = null);
