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

    public string LUsageEpithet { get; init; } = string.Empty;

    public LStateValue LUsageTitle { get; init; } = LUsageTitle ?? LStateValue.LStateValueUnspecified;

    public bool LUsageCollocated => LUsageOwner == LOwner.LOwnerCollocation;

    public bool LUsageQuoted => LUsageOwner == LOwner.LOwnerExample;
}
