namespace Llyn.Core;

public sealed record LVideo(
    long LVideoId,
    LStateValue LVideoLocation,
    LStateValue LVideoSpan)
{
    public LStateValue LVideoLocation { get; init; } = LVideoLocation ?? LStateValue.LStateValueUnspecified;

    public LStateValue LVideoSpan { get; init; } = LVideoSpan ?? LStateValue.LStateValueUnspecified;
}
