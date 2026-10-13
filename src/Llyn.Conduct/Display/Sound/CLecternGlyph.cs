using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CLecternGlyph(
    bool CLecternGlyphShown,
    string CLecternGlyphKey,
    string CLecternGlyphName,
    IReadOnlyList<CGlyphCell> CLecternGlyphCells,
    CFont CLecternGlyphFont);
