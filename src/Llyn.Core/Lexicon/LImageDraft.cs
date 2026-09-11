namespace Llyn.Core;

public sealed record LImageDraft(
    LStateValue LImageDraftLocation,
    long LImageDraftId = 0)
{
    public LStateValue LImageDraftLocation { get; init; } =
        LImageDraftLocation ?? LStateValue.LStateValueUnspecified;

    public static LImageDraft LImageDraftCreate(LStateValue location)
    {
        return new LImageDraft(location);
    }

    public bool LImageDraftEmpty => LImageDraftLocation.LStateValueEmpty;
}
