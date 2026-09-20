# LEntryClerk.cs

## `public sealed class LEntryClerk`

The interactor over the entry lifecycle the engine does not own itself.
An Entry is created, read, searched for, loaded back as a draft and deleted with the history that leaves.
It runs over the ports of one rig and holds no gate, no observer and no cache.
The engine calls it under its own gate and raises the bulletin a delete deserves.
The delete is the shape every spanning operation here follows.
That shape is several stores, one session, one decision.
The commit of a draft into an entry is not here yet, since it still spans half the engine.

## `public LEntryClerk(LRig rig)`

Reads the five ports the lifecycle touches out of `rig`.
Those are the root, the entries, the revisions, the tombstones and the workspace row.

## `public LEntry LEntryClerkCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)`

Creates `entry` with `forms` and `speeches` as its ordered child rows.
Returns the stored entry with its assigned id and timestamps.

## `public LEntry? LEntryClerkRead(long id)`

Reads the entry for `id`, or `null` when no entry has that id.

## `public LEntryDraft? LEntryClerkLoad(long id)`

The stored entry as the input form would have handed it over, or `null` when no entry has that id.
Every string comes out as stored, and nothing is derived on the way out.
A recording comes out relative to the workspace, and the engine resolves it, since only it knows the folder.

## `public IReadOnlyList<LEntry> LEntryClerkFind(string query)`

Returns the entries whose headword contains `query`, ordered by headword.
It returns every entry when `query` is empty or all whitespace.
Matching is a contains whose case is folded over the whole of Unicode.
So an accented headword is found typed in either case.

## `public IReadOnlyList<LEntry> LEntryClerkFind(string query, LCatalogOrder order)`

The entries answering `query`, in `order`.
The store answers which entries match, and the ordering is applied over what it returned.

## `public IReadOnlyList<LEntry> LEntryClerkFind(string query, LCatalogOrder order, LCatalogFilter filter)`

The ordered search with the entries in a hidden language left out.
The library panel lists through this, so the filter is applied here and never in the shell.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LTag tag)`

Returns the entries carrying `tag`, matched by its id, ordered by headword.
A zero id stands for no tag chosen and returns every entry.
The overload takes a tag rather than text so the two searches cannot be confused.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LTag tag, LCatalogFilter filter)`

The entries carrying `tag` with those in a hidden language left out.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LTag tag, string query, LCatalogFilter filter)`

The filtered entries carrying `tag` narrowed to those whose headword matches `query`.
The match is the catalog match, so `*` and `?` work as in every search field.
An empty or blank `query` narrows nothing.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LRegister register)`

Returns the entries marked with `register`, matched by its id, ordered by headword.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LRegister register, LCatalogFilter filter)`

The entries marked with `register` with those in a hidden language left out.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LRegister register, string query, LCatalogFilter filter)`

The filtered entries marked with `register` narrowed to those whose headword matches `query`.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LSituation situation)`

Returns the entries referencing `situation`, matched by its id, ordered by headword.
A zero id stands for no situation chosen and returns every entry.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LSituation situation, string query, LCatalogFilter filter)`

The entries referencing `situation`, those in a hidden language left out, narrowed to headwords matching `query`.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LExample example)`

Returns the entries quoting `example`, matched by its id, ordered by headword.
A zero id stands for no example chosen and returns every entry.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LExample example, string query, LCatalogFilter filter)`

The entries quoting `example`, those in a hidden language left out, narrowed to headwords matching `query`.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LReference reference)`

Returns the entries citing `reference`, matched by its id, ordered by headword.
A card cites a Source through the Example it holds, so the walk goes through that Example.
A zero id stands for no source chosen and returns every entry.

## `public IReadOnlyList<LEntry> LEntryClerkFind(LReference reference, string query, LCatalogFilter filter)`

The entries citing `reference`, those in a hidden language left out, narrowed to headwords matching `query`.

## `public static IReadOnlyList<LEntry> LEntryClerkMatch(IReadOnlyList<LEntry> entries, string query)`

The entries whose headword matches `query` by the catalog match.
An empty or blank `query` narrows nothing.
The engine's vista narrows a child list the same way, so it calls here.

## `public LRevision LEntryClerkDelete(long id)`

Deletes the entry identified by `id` with everything it owns.
It then writes the history the deletion leaves behind.
That is a revision carrying one delete change for the entry.
It is also a tombstone filed under that revision, and the workspace row moved onto it.
Returns the recorded revision.

The delete runs first.
So a refused delete records no history at all and throws its own message through.
A delete is refused when another entry still links to the entry.

All four writes share one session, so they are one transaction.
Either all land or none of them do.
Without it a failure part-way would leave an entry deleted with no tombstone naming it.
That is history that no longer describes the file.

## `public LTombstone? LTombstoneRead(long entryId)`

Reads the tombstone left by deleting the Entry identified by `entryId`, or `null` when that Entry has never been deleted.

## `public LRevision? LRevisionRead()`

Reads the revision the workspace row points at, or `null` when the workspace has none yet.
The pointer is what every save and delete moves.
The current revision is read from where it is recorded rather than guessed from the newest row.

## `public IReadOnlyList<LRevisionChange> LRevisionChangeRead(long revisionId)`

Reads the changes recorded under `revisionId`, in the order recorded.
