using System.Collections.Generic;
using System.Linq;
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
    IReadOnlyList<LTranscriptionDraft>? LEntryDraftTranscriptions = null,
    IReadOnlyList<LReflexDraft>? LEntryDraftReflexes = null)
{
    public IReadOnlyList<LPronunciationDraft> LEntryDraftPronunciations { get; init; } =
        LEntryDraftPronunciations ?? [];

    public IReadOnlyList<LTranscriptionDraft> LEntryDraftTranscriptions { get; init; } =
        LEntryDraftTranscriptions ?? [];

    public IReadOnlyList<LReflexDraft> LEntryDraftReflexes { get; init; } = LEntryDraftReflexes ?? [];

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

    [JsonIgnore]
    public IReadOnlyList<LPronunciationDraft> LEntryDraftAccents =>
        LEntryDraftPronunciations.Count <= 1 ? [] : LEntryDraftPronunciations.Skip(1).ToList();

    [JsonIgnore]
    public bool LEntryDraftNoted => LEntryDraftNote.Length > 0;

    [JsonIgnore]
    public bool LEntryDraftMarked => LEntryDraftSpeeches.Count > 0;

    [JsonIgnore]
    public bool LEntryDraftDefined => LEntryDraftMeanings.Count > 0;

    [JsonIgnore]
    public bool LEntryDraftCollocated => LEntryDraftCollocations.Count > 0;

    [JsonIgnore]
    public bool LEntryDraftReflected => LEntryDraftReflexes.Count > 0;

    [JsonIgnore]
    public bool LEntryDraftTargeted => LEntryDraftTargets.Count > 0;

    [JsonIgnore]
    public IReadOnlyList<long> LEntryDraftTargets =>
        LEntryDraftMeanings.Concat(LEntryDraftCollocations)
            .SelectMany(static card => card.LCardDraftTranslation)
            .Distinct()
            .ToList();

    public LEntryDraft LEntryDraftNormalize()
    {
        return this with
        {
            LEntryDraftMeanings = LEntryDraftMeanings.Select(static card => card.LCardDraftNormalize()).ToList(),
            LEntryDraftCollocations = LEntryDraftCollocations
                .Select(static card => card.LCardDraftNormalize())
                .ToList(),
        };
    }
}
