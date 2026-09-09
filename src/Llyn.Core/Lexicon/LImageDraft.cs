namespace Llyn.Core;

public sealed record LImageDraft(
    LStateValue LImageDraftLocation,
    string LImageDraftId = "")
{
    public LStateValue LImageDraftLocation { get; init; } =
        LImageDraftLocation ?? LStateValue.LStateValueUnspecified;

    public string LImageDraftId { get; init; } = LImageDraftId ?? string.Empty;

    public static LImageDraft LImageDraftCreate(LStateValue location)
    {
        return new LImageDraft(location);
    }

    public bool LImageDraftEmpty => LImageDraftLocation.LStateValueEmpty;
}
