using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LReflexDraft(
    string LReflexDraftLanguage,
    string LReflexDraftKind = "",
    string LReflexDraftText = "",
    bool LReflexDraftMain = false,
    long LReflexDraftId = 0,
    string LReflexDraftRomanization = "",
    string LReflexDraftMeaning = "",
    bool LReflexDraftOwned = false,
    string LReflexDraftNote = "",
    string LReflexDraftRespelling = "",
    string LReflexDraftRegion = "",
    LAnatomy? LReflexDraftAnatomy = null,
    IReadOnlyList<long>? LReflexDraftAnchors = null)
{
    public string LReflexDraftRespelling { get; init; } = LReflexDraftRespelling ?? string.Empty;

    public string LReflexDraftLanguage { get; init; } = LReflexDraftLanguage ?? string.Empty;

    public string LReflexDraftKind { get; init; } = LReflexDraftKind ?? string.Empty;

    public string LReflexDraftText { get; init; } = LReflexDraftText ?? string.Empty;

    public string LReflexDraftRomanization { get; init; } = LReflexDraftRomanization ?? string.Empty;

    public string LReflexDraftMeaning { get; init; } = LReflexDraftMeaning ?? string.Empty;

    public string LReflexDraftNote { get; init; } = LReflexDraftNote ?? string.Empty;

    public string LReflexDraftRegion { get; init; } = LReflexDraftRegion ?? string.Empty;

    public LAnatomy LReflexDraftAnatomy { get; init; } = LReflexDraftAnatomy ?? LAnatomy.LAnatomyEmpty;

    public IReadOnlyList<long> LReflexDraftAnchors { get; init; } = LAnchor.LAnchorNormalize(LReflexDraftAnchors);

    public bool LReflexDraftWritten => LReflexDraftText.Trim().Length > 0;

    public bool LReflexDraftEmpty =>
        LReflexDraftText.Trim().Length == 0
        && LReflexDraftLanguage.Trim().Length == 0
        && LReflexDraftKind.Trim().Length == 0
        && LReflexDraftRomanization.Trim().Length == 0
        && LReflexDraftMeaning.Trim().Length == 0
        && LReflexDraftRegion.Trim().Length == 0
        && LReflexDraftNote.Trim().Length == 0;
}
