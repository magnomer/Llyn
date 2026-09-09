namespace Llyn.Core;

public sealed record LExampleDraft(
    LStateValue LExampleDraftText,
    string LExampleDraftId,
    LStateValue LExampleDraftReference,
    LStateValue LExampleDraftTranslation,
    string LExampleDraftLanguage = "")
{
    public LStateValue LExampleDraftText { get; init; } =
        LExampleDraftText ?? LStateValue.LStateValueUnspecified;

    public LStateValue LExampleDraftReference { get; init; } =
        LExampleDraftReference ?? LStateValue.LStateValueUnspecified;

    public LStateValue LExampleDraftTranslation { get; init; } =
        LExampleDraftTranslation ?? LStateValue.LStateValueUnspecified;

    public string LExampleDraftLanguage { get; init; } = LExampleDraftLanguage ?? string.Empty;

    public static LExampleDraft LExampleDraftCreate(string text)
    {
        return new LExampleDraft(
            LStateValue.LStateValueRead(text),
            string.Empty,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified);
    }
}
