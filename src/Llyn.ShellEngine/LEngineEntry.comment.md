# LEngineEntry.cs

## `public sealed partial class LEngine`

The entry half of the engine.
An Entry is created, read, searched for, and loaded back into the draft the form saved.
It is deleted with the history that deletion leaves behind.
It sits apart from the composition root because it is one area of the model.
It is not a fact about the engine itself.
The delete is also the shape every spanning operation here follows.
That shape is several stores, one session, one decision.

## `public LEntry LEngineEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)`

Creates `entry` with `forms` and `speeches` as its ordered child rows.
Returns the stored entry with its assigned id and timestamps.

## `public LEntry? LEngineEntryRead(string id)`

Reads the entry for `id`, or `null` when no entry has that id.

## `public IReadOnlyList<LEntry> LEngineEntryFind(string query)`

Returns the entries whose headword contains `query`, ordered by headword.
It returns every entry when `query` is empty or all whitespace.
That is the list a browsing or searching pane shows.
Matching is a contains whose case is folded over the whole of Unicode.
So an accented headword is found typed in either case.

## `public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag)`

Returns the entries carrying `tag`, ordered by headword.
An empty tag text stands for no tag chosen and returns every entry.
That is the list the taxonomy panel shows beside its tag catalog.
The overload takes a tag rather than text so the two searches cannot be confused.

## `public LEntryDraft? LEngineEntryLoad(string id)`

Reads the entry identified by `id` back into the draft the input form saved.
It returns `null` when no entry has that id.
This is the inverse of `LEngineEntrySave`.
It composes the entry row, its meanings and collocations, its note and pronunciation.
It also composes the Examples, Situations and Tags each card references.
All of them arrive in stored order, as one value the shell can put back on screen.
It is read as a single consistent snapshot.
A card's synonym comes back empty, because no card writes one.
The links a Meaning or Collocation holds are read through the engine's relation seam instead.
That seam returns targets rather than text.

## `public LRevision LEngineEntryDelete(string id)`

Deletes the entry identified by `id` with everything it owns.
It then writes the history the deletion leaves behind.
That is a revision carrying one delete change for the entry.
It is also a tombstone filed under that revision, and the workspace row moved onto it.
Returns the recorded revision.

The delete runs first.
So a refused delete records no history at all and throws its own message through.
A delete is refused when another entry still links to the entry.

All four writes share one session, so they are one transaction.
Those are the deleted entry, the revision recording it, and the tombstone filed under it.
The fourth is the workspace row moved onto that revision.
Either all land or none of them do.
Without it a failure part-way would leave an entry deleted with no tombstone naming it.
That is history that no longer describes the file.

## `public LTombstone? LEngineTombstoneRead(string entryId)`

Reads the tombstone left by deleting the Entry identified by `entryId`, or `null` when that Entry has never been deleted.

## `public LRevision? LEngineRevisionRead()`

Reads the most recently opened revision, or `null` when the workspace has none yet.

## `public IReadOnlyList<LRevisionChange> LEngineChangeRead(string revisionId)`

Reads the changes recorded under `revisionId`, in the order recorded.

## Inline notes

### `return draft with { LEntryDraftAudio = LEngineRecordingResolve(draft.LEntryDraftAudio) };`

Stored relative, handed out full.
The shell plays a file, so it never has to know the workspace folder.
The same entry opened from a moved workspace still resolves.

## `public IReadOnlyList<LEntry> LEngineEntryFind(string query, LCatalogOrder order)`

The entries answering `query`, in `order`.
The store answers which entries match, and the ordering is applied over what it returned.
