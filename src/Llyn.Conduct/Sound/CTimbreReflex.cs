using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CTimbreReflex(
    bool CTimbreReflexShown,
    IReadOnlyList<CReflex> CTimbreReflexRows,
    CLecternAnchor CTimbreReflexAnchor,
    bool CTimbreReflexOpened,
    bool CTimbreReflexPending,
    bool CTimbreReflexFoldable);
