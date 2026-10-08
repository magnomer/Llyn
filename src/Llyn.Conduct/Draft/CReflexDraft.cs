using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed record CReflexDraft(
    long CReflexDraftId,
    string CReflexDraftLanguage,
    string CReflexDraftKind,
    string CReflexDraftText,
    string CReflexDraftRespelling,
    string CReflexDraftRomanization,
    string CReflexDraftMeaning,
    string CReflexDraftNote,
    bool CReflexDraftMain,
    string CReflexDraftRegion,
    IReadOnlyList<long> CReflexDraftAnchors)
{
    internal static IReadOnlyList<CReflexDraft> CReflexDraftRead(IReadOnlyList<LReflexDraft> reflexes)
    {
        ArgumentNullException.ThrowIfNull(reflexes);

        return reflexes.Select(CReflexDraftRead).ToList();
    }

    private static CReflexDraft CReflexDraftRead(LReflexDraft reflex)
    {
        return new CReflexDraft(
            reflex.LReflexDraftId,
            reflex.LReflexDraftLanguage,
            reflex.LReflexDraftKind,
            reflex.LReflexDraftText,
            reflex.LReflexDraftRespelling,
            reflex.LReflexDraftRomanization,
            reflex.LReflexDraftMeaning,
            reflex.LReflexDraftNote,
            reflex.LReflexDraftMain,
            reflex.LReflexDraftRegion,
            reflex.LReflexDraftAnchors);
    }
}
