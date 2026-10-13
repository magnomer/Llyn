using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LTranscriptionRow(
    LTranscriptionDraft LTranscriptionRowDraft,
    IReadOnlyList<LSchemeRow> LTranscriptionRowSchemes);
