namespace Llyn.Core;

public sealed record LImage(
    string LImageId,
    LStateValue LImageLocation)
{
    public LStateValue LImageLocation { get; init; } = LImageLocation ?? LStateValue.LStateValueUnspecified;
}
