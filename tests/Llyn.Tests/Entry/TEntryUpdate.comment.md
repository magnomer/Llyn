# TEntryUpdate.cs

## `public sealed class TEntryUpdate`

Covers the seam that changes a stored entry.
The entry keeps its identity while its cards are reconciled to the draft.
The revision says what actually moved.
An entry that is no longer there is refused.
An update that fails part-way leaves the stored entry exactly as it was.
The update stamp moves only when the draft differs from what is stored.
A change to any card alone moves it, because the stamp belongs to the whole entry.
A card changed through its own seam, outside the draft, moves it as well.

## Inline notes

### `LEntryDraft? loaded = engine.LEngineEntryLoad(entry.LEntryId);`

What the shell hands back.
It is the loaded draft with its middle card dropped and its first card renamed.
It also carries a card the user added, carrying no id of its own.

### `Assert.Equal(entry.LEntryId, updated.LEntryId);`

The entry is the same record: same id, same creation stamp, a fresh modification stamp.

### `Assert.Equal(firstId, stored[0].LMeaningId);`

The cards the draft still named kept their rows.
So anything pointing at them still points at the same Meaning.
The card added got a row of its own.

### `Assert.Equal(revision.LRevisionId, engine.LEngineWorkspace.LEngineStateRead().LWorkspaceStateRevision);`

The workspace row moved onto the revision the update recorded.

### `engine.LEngineEntryUpdate(entry.LEntryId, loaded with`

The Examples are reordered and the second Tag cleared.
The first Tag stays as it was.

### `Assert.Equal(2, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM example;"));`

Reordering re-attached the rows that were already there rather than writing new ones.

### `Assert.Equal(1, workspace.TWorkspaceCountRead("SELECT COUNT(*) FROM sense_tag;"));`

The dropped Tag was detached, not deleted: it is independent data the card only referenced.

### `LRefusal refusal = Assert.Throws<LRefusal>(() => engine.LEngineEntryUpdate(`

The first card names a Tag row nobody stores, so the tag write is refused.
That failure is reached only after the entry row and the card text have been written.

### `Assert.Equal(before?.LRevisionId, engine.LEngineRevisionRead()?.LRevisionId);`

A refused update records no history at all.
