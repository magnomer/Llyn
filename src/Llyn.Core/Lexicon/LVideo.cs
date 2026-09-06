namespace Llyn.Core;

public sealed record LVideo(
    string LVideoId,
    LStateValue LVideoLocation)
{
    public LStateValue LVideoLocation { get; init; } = LVideoLocation ?? LStateValue.LStateValueUnspecified;
}
