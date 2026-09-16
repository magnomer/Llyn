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

## `public LEntry? LEngineEntryRead(long id)`

Reads the entry for `id`, or `null` when no entry has that id.

## `public IReadOnlyList<LEntry> LEngineEntryFind(string query)`

Returns the entries whose headword contains `query`, ordered by headword.
It returns every entry when `query` is empty or all whitespace.
That is the list a browsing or searching pane shows.
Matching is a contains whose case is folded over the whole of Unicode.
So an accented headword is found typed in either case.

## `public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag)`

Returns the entries carrying `tag`, matched by its id, ordered by headword.
A zero id stands for no tag chosen and returns every entry.
That is the list the taxonomy panel shows beside its tag catalog.
The overload takes a tag rather than text so the two searches cannot be confused.

## `public IReadOnlyList<LEntry> LEngineEntryFind(string query, LCatalogOrder order, LCatalogFilter filter)`

The ordered search with the entries in a hidden language left out.
The library panel lists through this, so the filter is applied here and never in the shell.

## `public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag, LCatalogFilter filter)`

The entries carrying `tag` with those in a hidden language left out.

## `public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag, string query, LCatalogFilter filter)`

The filtered entries carrying `tag` narrowed to those whose headword matches `query`.
The match is the catalog match, so `*` and `?` work as in every search field.
An empty or blank `query` narrows nothing.

## `public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register, LCatalogFilter filter)`

The entries marked with `register` with those in a hidden language left out.

## `public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register, string query, LCatalogFilter filter)`

The filtered entries marked with `register` narrowed to those whose headword matches `query`.
The match is the catalog match, so `*` and `?` work as in every search field.
An empty or blank `query` narrows nothing.

## `public IReadOnlyList<LEntry> LEngineEntryFind(LSituation situation)`

Returns the entries referencing `situation`, matched by its id, ordered by headword.
A zero id stands for no situation chosen and returns every entry.
That is the list the repertoire panel shows beside its situation catalog.

## `public IReadOnlyList<LEntry> LEngineEntryFind(LSituation situation, string query, LCatalogFilter filter)`

The entries referencing `situation`, those in a hidden language left out, narrowed to headwords matching `query`.
The match is the catalog match, and an empty or blank `query` narrows nothing.

## `public IReadOnlyList<LEntry> LEngineEntryFind(LExample example)`

Returns the entries quoting `example`, matched by its id, ordered by headword.
A zero id stands for no example chosen and returns every entry.
That is the list the corpus panel shows beside its example catalog.

## `public IReadOnlyList<LEntry> LEngineEntryFind(LExample example, string query, LCatalogFilter filter)`

The entries quoting `example`, those in a hidden language left out, narrowed to headwords matching `query`.
The match is the catalog match, and an empty or blank `query` narrows nothing.

## `public IReadOnlyList<LEntry> LEngineEntryFind(LReference reference)`

Returns the entries citing `reference`, matched by its id, ordered by headword.
A card cites a Source through the Example it holds, so the walk goes through that Example.
A zero id stands for no source chosen and returns every entry.
That is the list the sources panel shows beside its shelf.

## `public IReadOnlyList<LEntry> LEngineEntryFind(LReference reference, string query, LCatalogFilter filter)`

The entries citing `reference`, those in a hidden language left out, narrowed to headwords matching `query`.
The match is the catalog match, and an empty or blank `query` narrows nothing.

## `public LEntryDraft? LEngineEntryLoad(long id)`

The stored entry as the input form would have handed it over.
Every recording comes out of the store relative to the workspace and leaves here as a full path.
The shell deals in full paths, and only the engine knows which folder the workspace stands in.
A pronunciation row carrying no recording is handed back untouched.
Every string comes out as stored, and nothing is derived on the way out.
The respelling was derived when the entry was saved, so a load runs no rule.

## `private LEntryDraft LEngineRespellingUpdate(LEntryDraft draft)`

Derives the respelling of every pronunciation row and reflex row that carries none, before the draft is stored.
A row that already carries one is left alone, since the user may have written it by hand.
Every reflex row then has its anatomy cut under the entry's own pack rules, whatever it carried.
It runs on the save and update seams, so the store always holds what the reading view will print.

## `public LRevision LEngineEntryDelete(long id)`

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

## `public LTombstone? LEngineTombstoneRead(long entryId)`

Reads the tombstone left by deleting the Entry identified by `entryId`, or `null` when that Entry has never been deleted.

## `public LRevision? LEngineRevisionRead()`

Reads the revision the workspace row points at, or `null` when the workspace has none yet.
The pointer is what every save and delete moves.
The current revision is read from where it is recorded rather than guessed from the newest row.

## `public IReadOnlyList<LRevisionChange> LEngineChangeRead(long revisionId)`

Reads the changes recorded under `revisionId`, in the order recorded.

## Inline notes

### `return draft with { LEntryDraftAudio = LEngineRecordingResolve(draft.LEntryDraftAudio) };`

Stored relative, handed out full.
The shell plays a file, so it never has to know the workspace folder.
The same entry opened from a moved workspace still resolves.

## `public IReadOnlyList<LEntry> LEngineEntryFind(string query, LCatalogOrder order)`

The entries answering `query`, in `order`.
The store answers which entries match, and the ordering is applied over what it returned.
