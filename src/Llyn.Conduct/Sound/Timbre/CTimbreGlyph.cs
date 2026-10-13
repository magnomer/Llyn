using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CTimbreGlyph(
    bool CTimbreGlyphShown,
    bool CTimbreGlyphSourced,
    IReadOnlyList<CTranscriptionDraft> CTimbreGlyphRows);
