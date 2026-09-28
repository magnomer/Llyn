using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LAccentSheet(
    string LAccentSheetLanguage,
    bool LAccentSheetFlagged,
    bool LAccentSheetTonal,
    bool LAccentSheetRespelled,
    string LAccentSheetOpener,
    string LAccentSheetCloser,
    LAccentRow LAccentSheetPrimary,
    IReadOnlyList<LAccentRow> LAccentSheetRows)
{
    public bool LAccentSheetSpoken => LAccentSheetPrimary.LAccentRowText.Length > 0;

    public IReadOnlyList<string> LAccentSheetVarieties =>
        LAccentSheetRows
            .Append(LAccentSheetPrimary)
            .Select(static row => row.LAccentRowVariety)
            .Where(static variety => variety.Length > 0)
            .ToList();
}
