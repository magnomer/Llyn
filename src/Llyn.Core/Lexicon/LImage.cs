namespace Llyn.Core;

public sealed record LImage(
    long LImageId,
    LStateValue LImageLocation)
{
    public LStateValue LImageLocation { get; init; } = LImageLocation ?? LStateValue.LStateValueUnspecified;
}
