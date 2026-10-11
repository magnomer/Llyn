using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed record LPressDigest(
    LPressPrinter? LPressDigestPrinter,
    IReadOnlyList<LPressPaper> LPressDigestPaper,
    int? LPressDigestChosen,
    IReadOnlyList<bool> LPressDigestIncluded,
    int LPressDigestSheets);
