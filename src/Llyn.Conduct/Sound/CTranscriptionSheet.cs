using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CTranscriptionSheet(
    bool CTranscriptionSheetShown,
    bool CTranscriptionSheetFree,
    IReadOnlyList<CTranscriptionRow> CTranscriptionSheetRows);
