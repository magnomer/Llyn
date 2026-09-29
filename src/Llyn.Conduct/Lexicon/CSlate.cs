using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CSlate(
    string CSlateText,
    IReadOnlyList<CSlateRow> CSlateRows,
    bool CSlateShown);
