namespace Llyn.Core;

/// <summary>
/// The workspace's operational state: which Entry each pane shows, the display mode, the split, and
/// the revision the session is on. This is session data, not lexical data — it owns nothing and
/// defines no ownership, so clearing it loses no dictionary content, and deleting an Entry simply
/// empties whichever pane was showing it.
/// </summary>
/// <param name="LWorkspaceStateId">Identity of the workspace row.</param>
/// <param name="LWorkspaceStateLeft">Id of the Entry the left pane shows; <c>null</c> when the pane is empty.</param>
/// <param name="LWorkspaceStateRight">Id of the Entry the right pane shows; <c>null</c> when the pane is empty.</param>
/// <param name="LWorkspaceStateMode">The workspace mode; <c>null</c> when none has been chosen.</param>
/// <param name="LWorkspaceStateSplit">The split state; <c>null</c> when none has been chosen.</param>
/// <param name="LWorkspaceStateRevision">Id of the current revision; <c>null</c> before any revision is recorded.</param>
public sealed record LWorkspaceState(
    string LWorkspaceStateId,
    string? LWorkspaceStateLeft,
    string? LWorkspaceStateRight,
    string? LWorkspaceStateMode,
    string? LWorkspaceStateSplit,
    string? LWorkspaceStateRevision);
