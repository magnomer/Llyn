namespace Llyn.Core;

public sealed record LCollocation(
    string LCollocationId,
    string LCollocationEntryId,
    int LCollocationPosition,
    string? LCollocationTitle,
    string? LCollocationExpression,
    string? LCollocationMeaning);
