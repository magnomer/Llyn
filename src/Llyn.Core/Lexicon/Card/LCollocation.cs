namespace Llyn.Core;

public sealed record LCollocation(
    long LCollocationId,
    long LCollocationEntryId,
    int LCollocationPosition,
    LStateValue LCollocationTitle,
    LStateValue LCollocationExpression,
    LStateValue LCollocationMeaning)
{
    public LStateValue LCollocationTitle { get; init; } = LCollocationTitle ?? LStateValue.LStateValueUnspecified;

    public LStateValue LCollocationExpression { get; init; } =
        LCollocationExpression ?? LStateValue.LStateValueUnspecified;

    public LStateValue LCollocationMeaning { get; init; } = LCollocationMeaning ?? LStateValue.LStateValueUnspecified;
}

