namespace Llyn.Core;

public sealed record LRevisionChange(
    int LRevisionChangePosition,
    long LRevisionChangeTarget,
    string LRevisionChangeSubject,
    string LRevisionChangeKind,
    string? LRevisionChangeSummary);
