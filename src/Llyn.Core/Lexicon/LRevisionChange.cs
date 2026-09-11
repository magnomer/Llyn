namespace Llyn.Core;

public sealed record LRevisionChange(
    int LRevisionChangePosition,
    long LRevisionChangeTargetId,
    string LRevisionChangeTargetType,
    string LRevisionChangeKind,
    string? LRevisionChangeSummary);
