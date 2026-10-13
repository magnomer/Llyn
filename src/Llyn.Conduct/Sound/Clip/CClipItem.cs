using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CClipItem(
    string CClipItemSource,
    int CClipItemOrder,
    IReadOnlyList<CClipReading> CClipItemReading,
    string CClipItemNotice,
    bool CClipItemReady);
