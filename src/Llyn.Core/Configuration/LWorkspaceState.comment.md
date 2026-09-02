# LWorkspaceState.cs

## `public sealed record LWorkspaceState(`

The workspace's operational state.
It holds which Entry each pane shows, the display mode, the split, and the session revision.
This is session data, not lexical data.
It owns nothing and defines no ownership, so clearing it loses no dictionary content.
Deleting an Entry simply empties whichever pane was showing it.

**Parameters**

- `LWorkspaceStateId` — Identity of the workspace row.
- `LWorkspaceStateLeft` — Id of the Entry the left pane shows, and `null` when the pane is empty.
- `LWorkspaceStateRight` — Id of the Entry the right pane shows, and `null` when the pane is empty.
- `LWorkspaceStateMode` — The workspace mode, and `null` when none has been chosen.
- `LWorkspaceStateSplit` — The split state, and `null` when none has been chosen.
- `LWorkspaceStateRevision` — Id of the current revision, and `null` before any revision is recorded.
