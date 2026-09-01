# LEngineEntry.cs

## `public sealed partial class LEngine`

The entry half of the engine: an Entry created, read, searched for, loaded back into the draft the input form saved, and deleted with the history that deletion leaves behind. It sits apart from the composition root because it is one area of the model rather than a fact about the engine itself, and because the delete is the shape every spanning operation here follows — several stores, one session, one decision.

## `public LEntry LEngineEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)`

Creates `entry` with `forms` and `speeches` as its ordered child rows, and returns the stored entry with its assigned id and timestamps.

## `public LEntry? LEngineEntryRead(string id)`

Reads the entry for `id`, or `null` when no entry has that id.

## `public IReadOnlyList<LEntry> LEngineEntryFind(string query)`

Returns the entries whose headword contains `query`, ordered by headword, or every entry when `query` is empty or all whitespace — the list a browsing or searching pane shows. Matching is a contains whose case is folded over the whole of Unicode, so an accented headword is found typed in either case.

## `public LEntryDraft? LEngineEntryLoad(string id)`

Reads the entry identified by `id` back into the draft the input form saved, or `null` when no entry has that id. This is the inverse of `LEngineEntrySave`: it composes the entry row, its meanings and collocations, its note and pronunciation, and the Examples, Situations and Tags each card references — all of them, in stored order — into one value the shell can put back on screen, read as a single consistent snapshot. A card's synonym comes back empty: no card writes one, and the links a Meaning or Collocation holds are read through the engine's relation seam instead, which returns targets rather than text.

## `public LRevision LEngineEntryDelete(string id)`

Deletes the entry identified by `id` with everything it owns, then writes the history the deletion leaves behind: a revision carrying one delete change for the entry, a tombstone filed under that revision, and the workspace row moved onto it. Returns the recorded revision.

The delete runs first, so a refused delete — an entry another entry still links to — records no history at all and throws its own message through.

All four writes share one session, so they are one transaction: the deleted entry, the revision recording it, the tombstone filed under that revision, and the workspace row moved onto it either all land or none of them do. Without it a failure part-way would leave an entry deleted with no tombstone naming it — history that no longer describes the file.

## `public LTombstone? LEngineTombstoneRead(string entryId)`

Reads the tombstone left by deleting the Entry identified by `entryId`, or `null` when that Entry has never been deleted.

## `public LRevision? LEngineRevisionRead()`

Reads the most recently opened revision, or `null` when the workspace has none yet.

## `public IReadOnlyList<LRevisionChange> LEngineChangeRead(string revisionId)`

Reads the changes recorded under `revisionId`, in the order recorded.

## Inline notes

### `return draft with { LEntryDraftAudio = LEngineRecordingResolve(draft.LEntryDraftAudio) };`

Stored relative, handed out full: the shell plays a file, so it never has to know the workspace folder, and the same entry opened from a moved workspace still resolves.
