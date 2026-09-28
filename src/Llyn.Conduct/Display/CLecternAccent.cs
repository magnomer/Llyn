using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CLecternAccent(
    CRespellingMark CLecternAccentMark,
    bool CLecternAccentTonal,
    string CLecternAccentText,
    bool CLecternAccentSpoken,
    CVariety CLecternAccentPrimary,
    IReadOnlyList<CAccent> CLecternAccentRows,
    bool CLecternAccentFlagged);
