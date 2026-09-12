using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Llyn.Core;

public sealed record LEntryDraft(
    string LEntryDraftHeadword,
    string LEntryDraftLanguage,
    IReadOnlyList<LPronunciationDraft>? LEntryDraftPronunciations,
    string LEntryDraftNote,
    IReadOnlyList<LCardDraft> LEntryDraftMeanings,
    IReadOnlyList<LCardDraft> LEntryDraftCollocations,
    IReadOnlyList<LSpeechDraft>? LEntryDraftSpeeches = null,
    IReadOnlyList<LForm>? LEntryDraftForms = null,
    IReadOnlyList<LInflection>? LEntryDraftInflections = null,
    IReadOnlyList<LTranscriptionDraft>? LEntryDraftTranscriptions = null)
{
    public IReadOnlyList<LPronunciationDraft> LEntryDraftPronunciations { get; init; } =
        LEntryDraftPronunciations ?? [];

    public IReadOnlyList<LTranscriptionDraft> LEntryDraftTranscriptions { get; init; } =
        LEntryDraftTranscriptions ?? [];

    public IReadOnlyList<LSpeechDraft> LEntryDraftSpeeches { get; init; } = LEntryDraftSpeeches ?? [];

    public IReadOnlyList<LForm> LEntryDraftForms { get; init; } = LEntryDraftForms ?? [];

    public IReadOnlyList<LInflection> LEntryDraftInflections { get; init; } = LEntryDraftInflections ?? [];

    [JsonIgnore]
    public LPronunciationDraft? LEntryDraftPronunciation =>
        LEntryDraftPronunciations.Count == 0 ? null : LEntryDraftPronunciations[0];

    [JsonIgnore]
    public string LEntryDraftIpa => LEntryDraftPronunciation?.LPronunciationDraftIpa ?? string.Empty;

    [JsonIgnore]
    public string LEntryDraftAudio => LEntryDraftPronunciation?.LPronunciationDraftAudio ?? string.Empty;
}
