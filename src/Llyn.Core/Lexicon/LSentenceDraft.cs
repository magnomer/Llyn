namespace Llyn.Core;

public sealed record LSentenceDraft(
    LExampleDraft? LSentenceDraftExample,
    LStateValue LSentenceDraftParticle,
    LStateValue LSentenceDraftDependence,
    long LSentenceDraftId = 0)
{
    public LStateValue LSentenceDraftParticle { get; init; } =
        LSentenceDraftParticle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LSentenceDraftDependence { get; init; } =
        LSentenceDraftDependence ?? LStateValue.LStateValueUnspecified;

    public bool LSentenceDraftEmpty =>
        (LSentenceDraftExample is null
            || (LSentenceDraftExample.LExampleDraftText.LStateValueEmpty
                && LSentenceDraftExample.LExampleDraftId <= 0))
        && LSentenceDraftParticle.LStateValueEmpty
        && LSentenceDraftDependence.LStateValueEmpty;

    public LSentenceDraft LSentenceDraftNormalize()
    {
        return this with
        {
            LSentenceDraftExample = LSentenceDraftExample?.LExampleDraftNormalize(),
            LSentenceDraftParticle = LSentenceDraftParticle.LStateValueNormalize(),
            LSentenceDraftDependence = LSentenceDraftDependence.LStateValueNormalize(),
        };
    }

    public static LSentenceDraft LSentenceDraftCreate(string text)
    {
        return new LSentenceDraft(
            LExampleDraft.LExampleDraftCreate(text),
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified);
    }
}
