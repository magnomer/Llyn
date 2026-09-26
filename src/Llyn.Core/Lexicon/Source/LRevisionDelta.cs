namespace Llyn.Core;

public sealed record LRevisionDelta(
    long LRevisionDeltaTarget,
    string LRevisionDeltaSubject,
    string LRevisionDeltaKind,
    string? LRevisionDeltaSummary);
