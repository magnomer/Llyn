namespace Llyn.Core;

public sealed record LRevisionChange(
    int LRevisionChangePosition,
    string LRevisionChangeTarget,
    string LRevisionChangeType,
    string LRevisionChangeKind,
    string? LRevisionChangeSummary);
