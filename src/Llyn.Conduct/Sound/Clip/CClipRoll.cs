using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CClipRoll(
    IReadOnlyList<CClipItem> CClipRollRows,
    bool CClipRollEmpty,
    bool CClipRollSearching,
    string CClipRollNotice);
