namespace Llyn.Core;

public sealed record LWorkspaceState(
    long LWorkspaceStateId,
    long? LWorkspaceStateLeft = null,
    long? LWorkspaceStateRight = null,
    long? LWorkspaceStateRevision = null,
    long LWorkspaceStateFloor = 0);
