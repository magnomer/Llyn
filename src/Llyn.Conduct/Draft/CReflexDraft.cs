using System.Collections.Generic;

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
    IReadOnlyList<long> CReflexDraftAnchors);
