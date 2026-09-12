namespace Llyn.Core;

public sealed record LTranscriptionDraft(
    string LTranscriptionDraftScheme,
    string LTranscriptionDraftText = "",
    long LTranscriptionDraftId = 0)
{
    public string LTranscriptionDraftScheme { get; init; } = LTranscriptionDraftScheme ?? string.Empty;

    public string LTranscriptionDraftText { get; init; } = LTranscriptionDraftText ?? string.Empty;

    public bool LTranscriptionDraftEmpty => LTranscriptionDraftText.Trim().Length == 0;
}
