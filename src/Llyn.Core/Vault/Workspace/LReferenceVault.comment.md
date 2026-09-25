# LReferenceVault.cs

## `public interface LReferenceVault`

The persistence port for the Reference rows the engine reads and writes.
It lists exactly what the engine asks of reference storage, and nothing about how rows are kept.
`LReferenceArchive` in Infrastructure is its adapter over the workspace database.

## `LReference LReferenceCreate(LReference reference);`

Inserts `reference` and returns the stored Reference with its id filled in.
A Reference carrying no id is given a fresh opaque one.
A Reference already carrying one keeps it, because a held source is named before the record it becomes exists.
Each field is written as its state plus, for a specified field alone, its value.
The new Reference is cited by nothing and credits no Author until one is attached.

## `LReference? LReferenceRead(long id);`

Reads the Reference identified by `id`, or `null` when none exists.

## `IReadOnlyList<LReference> LReferenceAllRead();`

Reads every Reference the workspace holds, by title.
The Sources panel browses the whole shelf, so it asks for it at once.

## `void LReferenceUpdate(LReference reference);`

Rewrites every field of the Reference identified by `reference`'s id, states and values alike.
The id, the Authors credited, and every citation pointing at it are untouched.
So an update never changes where the Reference appears or in what order.
Throws when no Reference carries that id.

## `void LReferenceDelete(long id, bool detach);`

The same delete, with `detach` clearing every citation first.
The detaching, the count, and the delete stand in one session.
Between any two steps the answer to whether anything still cites it can change.
An Example is not deleted with the Reference, so its citation is cleared back to Unspecified.
Only the links between them and this Reference go.
The guard and the delete share one transaction, so nothing can cite the Reference between them.

## `void LReferenceAuthorAttach(long referenceId, long authorId, int position);`

Credits an existing Author on this Reference at `position` in the Reference's own author order.
The Author row itself is untouched and stays available to every other Reference.

## `void LReferenceAuthorDetach(long referenceId, long authorId);`

Removes this Reference's credit for an Author.
The Author and its other credits survive.

## `IReadOnlyDictionary<long, int> LReferenceUsageRead();`

Counts the citations of every Source at once, for the catalog, the ordering, and the delete.
A Source nothing cites is absent from the map rather than present at zero.

The unit is the citing Example, which is the only thing that cites at all.
It is not the number of rows `LReferenceUsageRead(id)` returns.
That list also names each citing card as the way up to the citation.
Counting cards and Examples together would count one citation twice, from two directions.
So the badge showing this number says what it counts, through `Source.Tally`.
The Example rows of the list are what it agrees with.

## `IReadOnlyList<LUsage> LReferenceUsageRead(long id);`

Reads the citing Meanings, Collocations and Examples of one Source, itemized, each with the id the row leads to.
A card names its own id, the Entry it belongs to, and that Entry's headword.
It also names its title with the definition or expression behind it.
An Example names its sentence and its first Gloss, because an Example belongs to no Entry of its own.
The first Gloss is joined by position zero, and an Example without one reads as unspecified.
A card holding two Examples of one Source is listed once.
The row leads to the card rather than the citation.
