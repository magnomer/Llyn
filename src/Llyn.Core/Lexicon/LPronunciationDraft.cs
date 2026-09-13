using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPronunciationDraft(
    string LPronunciationDraftIpa,
    IReadOnlyList<LSyllable>? LPronunciationDraftSyllables = null,
    string LPronunciationDraftAudio = "",
    string? LPronunciationDraftSource = null,
    long LPronunciationDraftId = 0,
    string LPronunciationDraftVariety = "",
    bool LPronunciationDraftSeeded = false)
{
    public string LPronunciationDraftIpa { get; init; } = LPronunciationDraftIpa ?? string.Empty;

    public string LPronunciationDraftAudio { get; init; } = LPronunciationDraftAudio ?? string.Empty;

    public string LPronunciationDraftVariety { get; init; } = LPronunciationDraftVariety ?? string.Empty;

    public IReadOnlyList<LSyllable> LPronunciationDraftSyllables { get; init; } =
        LPronunciationDraftSyllables ?? [];

    public static LPronunciationDraft? LPronunciationDraftCreate(string ipa, string audio, string? source)
    {
        LPronunciationDraft written = new(ipa, LPronunciationDraftAudio: audio, LPronunciationDraftSource: source);
        return written.LPronunciationDraftEmpty ? null : written;
    }

    public bool LPronunciationDraftEmpty =>
        LPronunciationDraftIpa.Length == 0
        && LPronunciationDraftAudio.Length == 0
        && LPronunciationDraftSyllables.Count == 0;
}
