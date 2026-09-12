namespace Llyn.Core;

public sealed record LSituation(
    long LSituationId,
    LStateValue LSituationTitle,
    LStateValue LSituationDescription,
    LStateValue LSituationKind)
{
    public LStateValue LSituationTitle { get; init; } = LSituationTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LSituationDescription { get; init; } = LSituationDescription ?? LStateValue.LStateValueUnspecified;

    public LStateValue LSituationKind { get; init; } = LSituationKind ?? LStateValue.LStateValueUnspecified;

    public LSituation LSituationNormalize()
    {
        return this with
        {
            LSituationTitle = LSituationTitle.LStateValueNormalize(),
            LSituationDescription = LSituationDescription.LStateValueNormalize(),
            LSituationKind = LSituationKind.LStateValueNormalize(),
        };
    }
}

