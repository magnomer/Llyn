using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Core;

public sealed record LPortraitLegend(
    string LPortraitLegendUnknown,
    string LPortraitLegendUntitled,
    string LPortraitLegendUnwritten,
    string LPortraitLegendUnused,
    string LPortraitLegendOnce,
    string LPortraitLegendUses,
    string LPortraitLegendTranslation,
    string LPortraitLegendSource,
    string LPortraitLegendAuthor,
    string LPortraitLegendYear,
    string LPortraitLegendUrl,
    string LPortraitLegendNote,
    string LPortraitLegendDescription,
    IReadOnlyDictionary<LReferenceKind, string> LPortraitLegendKind)
{
    public string LPortraitTallyFormat(int count)
    {
        return count switch
        {
            0 => LPortraitLegendUnused,
            1 => LPortraitLegendOnce,
            _ => count.ToString(CultureInfo.InvariantCulture) + " " + LPortraitLegendUses,
        };
    }

    public string LPortraitKindFormat(LReferenceKind kind)
    {
        return LPortraitLegendKind.TryGetValue(kind, out string? named)
            ? named
            : LReference.LReferenceKindFormat(kind);
    }
}
