using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LEntryDraft(
    string LEntryDraftHeadword,
    string LEntryDraftLanguage,
    LPronunciationDraft? LEntryDraftPronunciation,
    string LEntryDraftNote,
    IReadOnlyList<LCardDraft> LEntryDraftMeanings,
    IReadOnlyList<LCardDraft> LEntryDraftCollocations,
    IReadOnlyList<LSpeechDraft>? LEntryDraftSpeeches = null,
    IReadOnlyList<LForm>? LEntryDraftForms = null,
    IReadOnlyList<LInflection>? LEntryDraftInflections = null)
{
    public IReadOnlyList<LSpeechDraft> LEntryDraftSpeeches { get; init; } = LEntryDraftSpeeches ?? [];

    public IReadOnlyList<LForm> LEntryDraftForms { get; init; } = LEntryDraftForms ?? [];

    public IReadOnlyList<LInflection> LEntryDraftInflections { get; init; } = LEntryDraftInflections ?? [];

    public string LEntryDraftIpa => LEntryDraftPronunciation?.LPronunciationDraftIpa ?? string.Empty;

    public string LEntryDraftAudio => LEntryDraftPronunciation?.LPronunciationDraftAudio ?? string.Empty;
}
