namespace Llyn.Core;

public sealed record LRevisionChange(
    int LRevisionChangePosition,
    long LRevisionChangeTarget,
    string LRevisionChangeType,
    string LRevisionChangeKind,
    string? LRevisionChangeSummary);
