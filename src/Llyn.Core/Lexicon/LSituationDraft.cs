namespace Llyn.Core;

public sealed record LSituationDraft(
    LStateValue LSituationDraftTitle,
    string LSituationDraftId,
    LStateValue LSituationDraftDescription,
    LStateValue LSituationDraftKind)
{
    public LStateValue LSituationDraftTitle { get; init; } =
        LSituationDraftTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LSituationDraftDescription { get; init; } =
        LSituationDraftDescription ?? LStateValue.LStateValueUnspecified;

    public LStateValue LSituationDraftKind { get; init; } =
        LSituationDraftKind ?? LStateValue.LStateValueUnspecified;

    public static LSituationDraft LSituationDraftCreate(string text)
    {
        return new LSituationDraft(
            LStateValue.LStateValueRead(text),
            string.Empty,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified);
    }
}
