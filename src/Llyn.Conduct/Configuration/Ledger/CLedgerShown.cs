using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CLedgerShown(IReadOnlyList<string> CLedgerShownChildren, bool CLedgerShownEmpty);
