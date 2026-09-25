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

    public LVideoDraft LVideoDraftNormalize()
    {
        return this with
        {
            LVideoDraftLocation = LVideoDraftLocation.LStateValueNormalize(),
            LVideoDraftSpan = LVideoDraftSpan.LStateValueNormalize(),
        };
    }

    public bool LVideoDraftEmpty => LVideoDraftLocation.LStateValueEmpty;
}
