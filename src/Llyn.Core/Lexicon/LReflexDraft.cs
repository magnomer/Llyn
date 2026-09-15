namespace Llyn.Core;

public sealed record LReflexDraft(
    string LReflexDraftLanguage,
    string LReflexDraftKind = "",
    string LReflexDraftText = "",
    bool LReflexDraftMain = false,
    long LReflexDraftId = 0,
    string LReflexDraftNote = "",
    string LReflexDraftRespelling = "")
{
    public string LReflexDraftRespelling { get; init; } = LReflexDraftRespelling ?? string.Empty;

    public string LReflexDraftLanguage { get; init; } = LReflexDraftLanguage ?? string.Empty;

    public string LReflexDraftKind { get; init; } = LReflexDraftKind ?? string.Empty;

    public string LReflexDraftText { get; init; } = LReflexDraftText ?? string.Empty;

    public string LReflexDraftNote { get; init; } = LReflexDraftNote ?? string.Empty;

    public bool LReflexDraftEmpty =>
        LReflexDraftText.Trim().Length == 0
        && LReflexDraftLanguage.Trim().Length == 0
        && LReflexDraftKind.Trim().Length == 0
        && LReflexDraftNote.Trim().Length == 0;
}
