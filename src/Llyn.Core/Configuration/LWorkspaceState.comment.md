# LWorkspaceState.cs

## `public sealed record LWorkspaceState(`

The workspace's operational state: which Entry each pane shows, the display mode, the split, and the revision the session is on. This is session data, not lexical data — it owns nothing and defines no ownership, so clearing it loses no dictionary content, and deleting an Entry simply empties whichever pane was showing it.

**Parameters**

- `LWorkspaceStateId` — Identity of the workspace row.
- `LWorkspaceStateLeft` — Id of the Entry the left pane shows; `null` when the pane is empty.
- `LWorkspaceStateRight` — Id of the Entry the right pane shows; `null` when the pane is empty.
- `LWorkspaceStateMode` — The workspace mode; `null` when none has been chosen.
- `LWorkspaceStateSplit` — The split state; `null` when none has been chosen.
- `LWorkspaceStateRevision` — Id of the current revision; `null` before any revision is recorded.
