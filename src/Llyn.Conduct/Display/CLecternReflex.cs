using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CLecternReflex(
    IReadOnlyList<CReflex> CLecternReflexRows,
    CLecternAnchor CLecternReflexAnchor,
    bool CLecternReflexPending);
