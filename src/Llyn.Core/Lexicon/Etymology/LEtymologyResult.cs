using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LEtymologyResult(
    IReadOnlyList<LTranslationTarget> LEtymologyResultTargets,
    bool LEtymologyResultNarrated)
{
    public bool LEtymologyResultLinked => LEtymologyResultTargets.Count > 0;

    public bool LEtymologyResultFilled => LEtymologyResultNarrated || LEtymologyResultLinked;
}
