using System;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed record CAccent(long CAccentId, CVariety CAccentVariety, string CAccentText, string CAccentAudio)
{
    internal static CAccent CAccentRead(string language, LAccentRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        return new CAccent(
            row.LAccentRowId,
            CVariety.CVarietyRead(language, row.LAccentRowVariety),
            row.LAccentRowText,
            row.LAccentRowAudio);
    }
}
