using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed record CTranscriptionDraft(
    long CTranscriptionDraftId,
    string CTranscriptionDraftScheme,
    string CTranscriptionDraftText)
{
    public string CTranscriptionDraftKey => CScheme.CSchemeKeyRead(CTranscriptionDraftScheme);

    internal static IReadOnlyList<CTranscriptionDraft> CTranscriptionDraftRead(
        IReadOnlyList<LTranscriptionDraft> transcriptions)
    {
        ArgumentNullException.ThrowIfNull(transcriptions);

        return transcriptions
            .Select(static row => new CTranscriptionDraft(
                row.LTranscriptionDraftId, row.LTranscriptionDraftScheme, row.LTranscriptionDraftText))
            .ToList();
    }
}
