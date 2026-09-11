namespace Llyn.Core;

public sealed record LVideoDraft(
    LStateValue LVideoDraftLocation,
    LStateValue LVideoDraftSpan,
    long LVideoDraftId = 0)
{
    public LStateValue LVideoDraftLocation { get; init; } =
        LVideoDraftLocation ?? LStateValue.LStateValueUnspecified;

    public LStateValue LVideoDraftSpan { get; init; } =
        LVideoDraftSpan ?? LStateValue.LStateValueUnspecified;

    public static LVideoDraft LVideoDraftCreate(LStateValue location)
    {
        return new LVideoDraft(location, LStateValue.LStateValueUnspecified);
    }

    public bool LVideoDraftEmpty => LVideoDraftLocation.LStateValueEmpty;
}
