namespace Llyn.Core;

public sealed record LSituationDraft(
    LStateValue LSituationDraftTitle,
    long LSituationDraftId,
    LStateValue LSituationDraftDescription,
    LStateValue LSituationDraftKind)
{
    public LStateValue LSituationDraftTitle { get; init; } =
        LSituationDraftTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LSituationDraftDescription { get; init; } =
        LSituationDraftDescription ?? LStateValue.LStateValueUnspecified;

    public LStateValue LSituationDraftKind { get; init; } =
        LSituationDraftKind ?? LStateValue.LStateValueUnspecified;

    public bool LSituationDraftStored => LSituationDraftId != 0;

    public LSituationDraft LSituationDraftNormalize()
    {
        return this with
        {
            LSituationDraftTitle = LSituationDraftTitle.LStateValueNormalize(),
            LSituationDraftDescription = LSituationDraftDescription.LStateValueNormalize(),
            LSituationDraftKind = LSituationDraftKind.LStateValueNormalize(),
        };
    }
}
