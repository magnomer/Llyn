# LWorkspaceState.cs

## `public sealed record LWorkspaceState(`

The shell's own view state for one workspace.
It holds which Entry each duplex side shows.
The session revision is held with them.
So is the identity floor: the lowest temporary id this workspace has ever issued.
This is session data, not lexical data.
It owns nothing and defines no ownership, so clearing it loses no dictionary content.
Deleting an Entry simply empties whichever duplex side was showing it.

**Where view state lives**

State that names a row of this workspace's database is stored here, beside that database.
The Entry a duplex side stands on names such a row, and so does the revision.
State that names only panels is stored in [`LSettings`](LSettings.comment.md) instead.
That is the open tab, its split, each ordering and each hidden language.
No query reads it back, so the database has no use for it.

**Parameters**

- `LWorkspaceStateLeft` — Id of the Entry the left duplex side shows, and `null` when that side is empty.
- `LWorkspaceStateRight` — Id of the Entry the right duplex side shows, and `null` when that side is empty.
- `LWorkspaceStateRevision` — Id of the current revision, and `null` before any revision is recorded.
- `LWorkspaceStateFloor` — Lowest temporary id ever issued in this workspace, and 0 before any was.
