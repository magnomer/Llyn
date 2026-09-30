using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CTimbreAccent(
    CRespellingMark CTimbreAccentMark,
    CVariety CTimbreAccentPrimary,
    IReadOnlyList<CAccent> CTimbreAccentRows,
    bool CTimbreAccentFlagged);
