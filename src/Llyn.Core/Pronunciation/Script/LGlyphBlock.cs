using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LGlyphBlock(
    bool LGlyphBlockShown,
    bool LGlyphBlockSourced,
    IReadOnlyList<LTranscriptionDraft> LGlyphBlockRows,
    IReadOnlyList<LTranscriptionDraft> LGlyphBlockOther);
