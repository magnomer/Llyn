using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LTranscriptionSheet(
    bool LTranscriptionSheetShown,
    string? LTranscriptionSheetScheme,
    IReadOnlyList<LTranscriptionRow> LTranscriptionSheetRows)
{
    public bool LTranscriptionSheetFree => LTranscriptionSheetScheme is not null;
}
