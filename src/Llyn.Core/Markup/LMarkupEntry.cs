using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LMarkupEntry(
    string LMarkupEntryHeadword,
    string LMarkupEntryLanguage,
    IReadOnlyList<string>? LMarkupEntrySpeech = null,
    IReadOnlyList<LForm>? LMarkupEntryForm = null,
    IReadOnlyList<LMarkupInflection>? LMarkupEntryInflection = null,
    IReadOnlyList<LPronunciationDraft>? LMarkupEntryPronunciation = null,
    IReadOnlyList<LTranscriptionDraft>? LMarkupEntryTranscription = null,
    IReadOnlyList<LMarkupCard>? LMarkupEntryMeaning = null,
    IReadOnlyList<LMarkupCard>? LMarkupEntryCollocation = null,
    string LMarkupEntryNote = "",
    int LMarkupEntryLine = 0) : IEquatable<LMarkupEntry>
{
    public string LMarkupEntryHeadword { get; init; } = LMarkupEntryHeadword ?? string.Empty;

    public string LMarkupEntryLanguage { get; init; } = LMarkupEntryLanguage ?? string.Empty;

    public IReadOnlyList<string> LMarkupEntrySpeech { get; init; } = LMarkupEntrySpeech ?? [];

    public IReadOnlyList<LForm> LMarkupEntryForm { get; init; } = LMarkupEntryForm ?? [];

    public IReadOnlyList<LMarkupInflection> LMarkupEntryInflection { get; init; } = LMarkupEntryInflection ?? [];

    public IReadOnlyList<LPronunciationDraft> LMarkupEntryPronunciation { get; init; } =
        LMarkupEntryPronunciation ?? [];

    public IReadOnlyList<LTranscriptionDraft> LMarkupEntryTranscription { get; init; } =
        LMarkupEntryTranscription ?? [];

    public IReadOnlyList<LMarkupCard> LMarkupEntryMeaning { get; init; } = LMarkupEntryMeaning ?? [];

    public IReadOnlyList<LMarkupCard> LMarkupEntryCollocation { get; init; } = LMarkupEntryCollocation ?? [];

    public string LMarkupEntryNote { get; init; } = LMarkupEntryNote ?? string.Empty;

    public bool Equals(LMarkupEntry? other)
    {
        return other is not null
            && LMarkupEntryHeadword == other.LMarkupEntryHeadword
            && LMarkupEntryLanguage == other.LMarkupEntryLanguage
            && LMarkupEntrySpeech.SequenceEqual(other.LMarkupEntrySpeech)
            && LMarkupEntryForm.SequenceEqual(other.LMarkupEntryForm)
            && LMarkupEntryInflection.SequenceEqual(other.LMarkupEntryInflection)
            && LMarkupEntryPronunciation.Count == other.LMarkupEntryPronunciation.Count
            && LMarkupEntryPronunciation
                .Zip(other.LMarkupEntryPronunciation, LMarkupPronunciationMatch)
                .All(static same => same)
            && LMarkupEntryTranscription.SequenceEqual(other.LMarkupEntryTranscription)
            && LMarkupEntryMeaning.SequenceEqual(other.LMarkupEntryMeaning)
            && LMarkupEntryCollocation.SequenceEqual(other.LMarkupEntryCollocation)
            && LMarkupEntryNote == other.LMarkupEntryNote;
    }

    private static bool LMarkupPronunciationMatch(LPronunciationDraft first, LPronunciationDraft second)
    {
        return first.LPronunciationDraftIpa == second.LPronunciationDraftIpa
            && first.LPronunciationDraftVariety == second.LPronunciationDraftVariety
            && first.LPronunciationDraftAudio == second.LPronunciationDraftAudio
            && first.LPronunciationDraftSource == second.LPronunciationDraftSource
            && first.LPronunciationDraftSyllables.SequenceEqual(second.LPronunciationDraftSyllables);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            LMarkupEntryHeadword,
            LMarkupEntryLanguage,
            LMarkupEntryMeaning.Count,
            LMarkupEntryCollocation.Count,
            LMarkupEntryNote);
    }
}
