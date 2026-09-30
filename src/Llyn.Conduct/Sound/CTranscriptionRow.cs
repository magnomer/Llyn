using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CTranscriptionRow(
    CTranscriptionDraft CTranscriptionRowDraft,
    IReadOnlyList<CScheme> CTranscriptionRowSchemes);
