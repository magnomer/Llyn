using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CStencilSide(
    int CStencilSideIndex,
    IReadOnlyList<CStencilChip> CStencilSideChips,
    bool CStencilSideCustom);
