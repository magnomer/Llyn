namespace Llyn.Core;

public sealed record LRevisionChange(
    long LRevisionChangeTarget,
    string LRevisionChangeSubject,
    string LRevisionChangeKind,
    string? LRevisionChangeSummary);
