using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LGlyphPort
{
    LGlyph? LEngineGlyphRead(LEntryDraft draft);

    IReadOnlyList<LGlyphCell> LEngineGlyphDivide(LEntryDraft draft);

    IReadOnlyList<LTranscriptionDraft> LEngineTranscriptionRead(LEntryDraft draft);
}
