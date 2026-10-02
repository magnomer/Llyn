using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CEntryDraft(
    string CEntryDraftHeadword,
    string CEntryDraftNote,
    IReadOnlyList<CCardDraft> CEntryDraftMeanings,
    IReadOnlyList<CCardDraft> CEntryDraftCollocations,
    CEtymologyDraft CEntryDraftEtymology);
