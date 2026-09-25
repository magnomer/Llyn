namespace Llyn.Core;

public sealed record LImageDraft(
    LStateValue LImageDraftLocation,
    long LImageDraftId = 0)
{
    public LStateValue LImageDraftLocation { get; init; } =
        LImageDraftLocation ?? LStateValue.LStateValueUnspecified;

    public LImageDraft LImageDraftNormalize()
    {
        return this with { LImageDraftLocation = LImageDraftLocation.LStateValueNormalize() };
    }

    public bool LImageDraftEmpty => LImageDraftLocation.LStateValueEmpty;
}
