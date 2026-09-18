namespace Llyn.Core;

public sealed record LUsage(
    long LUsageId,
    LOwner LUsageOwner,
    long LUsageEntry,
    string LUsageHeadword,
    string LUsageLanguage,
    LStateValue LUsageTitle)
{
    public string LUsageName { get; init; } = LUsageHeadword;

    public LStateValue LUsageTitle { get; init; } = LUsageTitle ?? LStateValue.LStateValueUnspecified;
}
