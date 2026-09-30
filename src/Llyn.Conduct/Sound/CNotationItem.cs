using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CNotationItem(
    string CNotationItemSource,
    int CNotationItemOrder,
    IReadOnlyList<CNotationReading> CNotationItemReading,
    string CNotationItemNotice,
    bool CNotationItemReady);
