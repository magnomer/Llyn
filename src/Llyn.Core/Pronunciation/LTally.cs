using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LTally(
    string LTallyDivision,
    IReadOnlyList<LTallyLine> LTallyLines)
{
    public string LTallyDivision { get; init; } = LTallyDivision ?? string.Empty;

    public IReadOnlyList<LTallyLine> LTallyLines { get; init; } = LTallyLines ?? [];
}
