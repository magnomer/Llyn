namespace Llyn.Core;

public sealed record LExampleDraft(
    LStateValue LExampleDraftText,
    string LExampleDraftId,
    LStateValue LExampleDraftReference,
    LStateValue LExampleDraftParticle,
    LStateValue LExampleDraftDependence,
    LExampleDraft? LExampleDraftRevision)
{
    public LStateValue LExampleDraftText { get; init; } =
        LExampleDraftText ?? LStateValue.LStateValueUnspecified;

    public LStateValue LExampleDraftReference { get; init; } =
        LExampleDraftReference ?? LStateValue.LStateValueUnspecified;

    public LStateValue LExampleDraftParticle { get; init; } =
        LExampleDraftParticle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LExampleDraftDependence { get; init; } =
        LExampleDraftDependence ?? LStateValue.LStateValueUnspecified;

    public static LExampleDraft LExampleDraftCreate(string text)
    {
        return new LExampleDraft(
            LStateValue.LStateValueRead(text),
            string.Empty,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            null);
    }
}
