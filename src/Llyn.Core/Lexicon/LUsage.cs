namespace Llyn.Core;

public sealed record LUsage(
    string LUsageId,
    LOwner LUsageOwner,
    string LUsageEntry,
    string LUsageHeadword,
    string LUsageLanguage,
    LStateValue LUsageTitle)
{
    public LStateValue LUsageTitle { get; init; } = LUsageTitle ?? LStateValue.LStateValueUnspecified;
}
