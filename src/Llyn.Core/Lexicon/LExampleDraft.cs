namespace Llyn.Core;

public sealed record LExampleDraft(
    LStateValue LExampleDraftText,
    long LExampleDraftId,
    LStateAnchor LExampleDraftReference,
    LStateValue LExampleDraftTranslation,
    string LExampleDraftLanguage = "")
{
    public LStateValue LExampleDraftText { get; init; } =
        LExampleDraftText ?? LStateValue.LStateValueUnspecified;

    public LStateAnchor LExampleDraftReference { get; init; } =
        LExampleDraftReference ?? LStateAnchor.LStateAnchorUnspecified;

    public LStateValue LExampleDraftTranslation { get; init; } =
        LExampleDraftTranslation ?? LStateValue.LStateValueUnspecified;

    public string LExampleDraftLanguage { get; init; } = LExampleDraftLanguage ?? string.Empty;

    public static LExampleDraft LExampleDraftCreate(string text)
    {
        return new LExampleDraft(
            LStateValue.LStateValueRead(text),
            0,
            LStateAnchor.LStateAnchorUnspecified,
            LStateValue.LStateValueUnspecified);
    }
}
