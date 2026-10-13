using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LTally(
    string LTallyHeading,
    IReadOnlyList<LTallyLine> LTallyLines)
{
    public string LTallyHeading { get; init; } = LTallyHeading ?? string.Empty;

    public IReadOnlyList<LTallyLine> LTallyLines { get; init; } = LTallyLines ?? [];

}
