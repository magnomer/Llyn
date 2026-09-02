namespace Llyn.Core;

public sealed record LCollocation(
    string LCollocationId,
    string LCollocationEntryId,
    int LCollocationPosition,
    LStateValue LCollocationTitle,
    LStateValue LCollocationExpression,
    LStateValue LCollocationMeaning)
{
    public LStateValue LCollocationTitle { get; init; } = LCollocationTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LCollocationExpression { get; init; } = LCollocationExpression ?? LStateValue.LStateValueUnspecified;

    public LStateValue LCollocationMeaning { get; init; } = LCollocationMeaning ?? LStateValue.LStateValueUnspecified;
}

