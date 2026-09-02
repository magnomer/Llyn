namespace Llyn.Core;

public sealed record LSituationDraft(
    LStateValue LSituationDraftText,
    string LSituationDraftId,
    LStateValue LSituationDraftReference)
{
    public LStateValue LSituationDraftText { get; init; } =
        LSituationDraftText ?? LStateValue.LStateValueUnspecified;

    public LStateValue LSituationDraftReference { get; init; } =
        LSituationDraftReference ?? LStateValue.LStateValueUnspecified;

    public static LSituationDraft LSituationDraftCreate(string text)
    {
        return new LSituationDraft(
            LStateValue.LStateValueRead(text), string.Empty, LStateValue.LStateValueUnspecified);
    }
}
