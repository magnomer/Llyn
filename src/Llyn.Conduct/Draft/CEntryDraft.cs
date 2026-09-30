using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CEntryDraft(
    string CEntryDraftHeadword,
    string CEntryDraftLanguage,
    string CEntryDraftNote,
    CPronunciationDraft? CEntryDraftPronunciation,
    IReadOnlyList<CPronunciationDraft> CEntryDraftAccents,
    IReadOnlyList<CCardDraft> CEntryDraftMeanings,
    IReadOnlyList<CCardDraft> CEntryDraftCollocations,
    IReadOnlyList<CTranscriptionDraft> CEntryDraftTranscriptions,
    IReadOnlyList<CReflexDraft> CEntryDraftReflexes,
    CEtymologyDraft CEntryDraftEtymology)
{
    public bool CEntryDraftReflected => CEntryDraftReflexes.Count > 0;
}
