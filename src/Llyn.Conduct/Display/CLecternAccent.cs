using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CLecternAccent(
    CRespellingMark CLecternAccentMark,
    IReadOnlyList<CContour> CLecternAccentContour,
    string CLecternAccentText,
    bool CLecternAccentSpoken,
    CVariety CLecternAccentPrimary,
    IReadOnlyList<CAccent> CLecternAccentRows,
    bool CLecternAccentFlagged);
