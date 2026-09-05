namespace Llyn.Core;

public sealed record LSituationDraft(
    LStateValue LSituationDraftText,
    string LSituationDraftId)
{
    public LStateValue LSituationDraftText { get; init; } =
        LSituationDraftText ?? LStateValue.LStateValueUnspecified;

    public static LSituationDraft LSituationDraftCreate(string text)
    {
        return new LSituationDraft(LStateValue.LStateValueRead(text), string.Empty);
    }
}
