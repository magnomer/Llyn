namespace Llyn.Core;

public sealed record LWorkspaceState(
    string LWorkspaceStateId,
    string? LWorkspaceStateLeft,
    string? LWorkspaceStateRight,
    string? LWorkspaceStateMode,
    string? LWorkspaceStateSplit,
    string? LWorkspaceStateRevision);
