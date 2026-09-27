using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CPortraitLegend(
    string CPortraitLegendUnknown,
    string CPortraitLegendUntitled,
    string CPortraitLegendUnwritten,
    string CPortraitLegendUnused,
    string CPortraitLegendOnce,
    string CPortraitLegendUses,
    string CPortraitLegendTranslation,
    string CPortraitLegendSource,
    string CPortraitLegendAuthor,
    string CPortraitLegendYear,
    string CPortraitLegendUrl,
    string CPortraitLegendNote,
    string CPortraitLegendDescription,
    IReadOnlyDictionary<string, string> CPortraitLegendKind);
