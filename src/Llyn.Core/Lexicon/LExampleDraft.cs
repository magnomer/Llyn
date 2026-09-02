namespace Llyn.Core;

public sealed record LExampleDraft(
    LStateValue LExampleDraftText,
    string LExampleDraftId,
    LStateValue LExampleDraftReference)
{
    public LStateValue LExampleDraftText { get; init; } =
        LExampleDraftText ?? LStateValue.LStateValueUnspecified;

    public LStateValue LExampleDraftReference { get; init; } =
        LExampleDraftReference ?? LStateValue.LStateValueUnspecified;

    public static LExampleDraft LExampleDraftCreate(string text)
    {
        return new LExampleDraft(
            LStateValue.LStateValueRead(text), string.Empty, LStateValue.LStateValueUnspecified);
    }
}
