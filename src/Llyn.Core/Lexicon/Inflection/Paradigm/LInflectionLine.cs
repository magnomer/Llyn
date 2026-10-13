using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LInflectionLine(
    string LInflectionLineGroup,
    string LInflectionLineLabel,
    IReadOnlyList<IReadOnlyList<long>> LInflectionLineCells);
