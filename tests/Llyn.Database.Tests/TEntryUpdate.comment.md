# TEntryUpdate.cs

## `public sealed class TEntryUpdate`

Covers the seam that changes a stored entry: the entry keeps its identity while its cards are reconciled to the draft, the revision says what actually moved, an entry that is no longer there is refused, and an update that fails part-way leaves the stored entry exactly as it was.

## Inline notes

### `LEntryDraft? loaded = engine.LEngineEntryLoad(entry.LEntryId);`

What the shell hands back: the loaded draft with its middle card dropped, its first card renamed, and a card the user added carrying no id of its own.

### `Assert.Equal(entry.LEntryId, updated.LEntryId);`

The entry is the same record: same id, same creation stamp, a fresh modification stamp.

### `Assert.Equal(firstId, stored[0].LSenseId);`

The cards the draft still named kept their rows, so anything pointing at them still points at the same Meaning; the card added got a row of its own.

### `Assert.Equal(revision.LRevisionId, engine.LEngineStateRead().LWorkspaceStateRevision);`

The workspace row moved onto the revision the update recorded.

### `engine.LEngineEntryUpdate(entry.LEntryId, loaded with`

The Examples are reordered and the second Tag cleared; the first Tag stays as it was.

### `Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));`

Reordering re-attached the rows that were already there rather than writing new ones.

### `Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_tag;"));`

The dropped Tag was detached, not deleted: it is independent data the card only referenced.

### `LEntryArchive entries = new(workspace.TWorkspaceDatabase);`

Another entry links to the second Meaning, so deleting that Meaning is refused — a failure reached only after the entry row and the first card have already been written.

### `Assert.Equal(before?.LRevisionId, engine.LEngineRevisionRead()?.LRevisionId);`

A refused update records no history at all.
