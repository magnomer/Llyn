# LEngineEntry.cs

## `public sealed partial class LEngine`

The entry facade of the engine.
Every call takes the gate and hands the work to `LEntryClerk`, which holds the rules.
The facade stays because the shell calls the engine, and the engine alone holds the gate and the observers.
The one thing done here beyond the clerk is resolving a recording to a full path on load.

## `internal LEntry LEngineEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)`

Creates `entry` with `forms` and `speeches` as its ordered child rows, under the gate.

## `public LEntry? LEngineEntryRead(long id)`

Reads the entry for `id`, or `null` when no entry has that id.

## `public IReadOnlyList<LEntry> LEngineEntryFind(string query)`

The clerk's search under the gate.
Every other `LEngineEntryFind` overload is the same relay for the clerk's overload of the same shape.

## `public LEntryDraft? LEngineEntryLoad(long id)`

The stored entry as the input form would have handed it over.
Every recording comes out of the store relative to the workspace and leaves here as a full path.
The shell deals in full paths, and only the engine knows which folder the workspace stands in.
A pronunciation row carrying no recording is handed back untouched.
The respelling was derived when the entry was saved, so a load runs no rule.

## `internal LRevision LEngineEntryDelete(long id)`

The clerk's delete under the gate, then the entry bulletin raised outside it.
Returns the recorded revision.

## `internal LTombstone? LEngineTombstoneRead(long entryId)`

Reads the tombstone left by deleting the Entry identified by `entryId`, or `null` when that Entry has never been deleted.

## `internal LRevision? LEngineRevisionRead()`

Reads the revision the workspace row points at, or `null` when the workspace has none yet.

## `internal IReadOnlyList<LRevisionChange> LEngineChangeRead(long revisionId)`

Reads the changes recorded under `revisionId`, in the order recorded.
