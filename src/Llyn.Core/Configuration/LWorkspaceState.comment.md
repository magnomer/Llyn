# LWorkspaceState.cs

## `public sealed record LWorkspaceState(`

The shell's own view state for one workspace.
It holds which Entry each duplex side shows and which tab stands open.
It also holds whether that tab shows its editor and the ordering each browse panel lists by.
The session revision is held with them.
So is the identity floor: the lowest temporary id this workspace has ever issued.
This is session data, not lexical data.
It owns nothing and defines no ownership, so clearing it loses no dictionary content.
Deleting an Entry simply empties whichever duplex side was showing it.

**Where view state lives**

State that names this workspace's data or its panels is stored here, in the workspace database.
The Entry a duplex side stands on names a row of this database.
An ordering names a catalog of this workspace's records.
The open tab and its split name panels over that same data, so they are stored here too.
State tied to the installation rather than to any workspace is stored in [`LSettings`](LSettings.comment.md) instead.
That is the interface language and the window geometry, which mean the same thing whichever workspace is open.

**Parameters**

- `LWorkspaceStateId` — Id of the workspace row, always 1.
- `LWorkspaceStateLeft` — Id of the Entry the left duplex side shows, and `null` when that side is empty.
- `LWorkspaceStateRight` — Id of the Entry the right duplex side shows, and `null` when that side is empty.
- `LWorkspaceStateRevision` — Id of the current revision, and `null` before any revision is recorded.
- `LWorkspaceStateFloor` — Lowest temporary id ever issued in this workspace, and 0 before any was.
- `LWorkspaceStateMode` — Name of the tab standing open, and `null` before any tab is chosen.
- `LWorkspaceStateSplit` — Whether the open tab shows its editor rather than its read area.
- `LWorkspaceStateOrder` — Ordering the library panel lists entries in.
- `LWorkspaceStateSequence` — Ordering the phonology panel lists pronunciations in.
- `LWorkspaceStateSeries` — Ordering the favorites panel lists favorite entries in.
- `LWorkspaceStateFunnel` — Ordering the taxonomy panel lists tags in.
- `LWorkspaceStateTier` — Ordering the repertoire panel lists situations in.
- `LWorkspaceStateGrade` — Ordering the sources panel lists sources in.
- `LWorkspaceStateRank` — Ordering the corpus panel lists examples in.
