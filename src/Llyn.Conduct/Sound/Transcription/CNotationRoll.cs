using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CNotationRoll(
    IReadOnlyList<CNotationItem> CNotationRollRows,
    bool CNotationRollEmpty,
    bool CNotationRollSearching,
    string CNotationRollNotice);
