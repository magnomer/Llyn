using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LEntryDraft(
    string LEntryDraftHeadword,
    string LEntryDraftLanguage,
    string LEntryDraftPronunciation,
    string LEntryDraftNote,
    IReadOnlyList<LCardDraft> LEntryDraftMeanings,
    IReadOnlyList<LCardDraft> LEntryDraftCollocations,
    string LEntryDraftAudio = "",
    string? LEntryDraftSource = null,
    IReadOnlyList<string>? LEntryDraftSpeeches = null);
