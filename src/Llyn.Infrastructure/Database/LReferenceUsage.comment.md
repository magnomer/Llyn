# LReferenceUsage.cs

## `public sealed class LReferenceUsage`

Reads the citing side of a Source: which cards and Examples cite it, and how many.
It lives beside `LReferenceArchive` rather than inside it.
Counting the citations of a Source is a different responsibility from storing one.
Nothing here creates, changes, or deletes a Source.

A Situation is not counted, because a Situation is written rather than quoted and carries no Source column.
Only an Example cites a Source, through its own `reference_ref` column.
A Meaning or a Collocation is reported as citing because it holds such an Example, never because a row says so.
So a card arm is a join back up the chain and detaching is clearing two columns on the Example.

## `public LReferenceUsage(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyDictionary<string, int> LReferenceUsageRead()`

Counts the citations of every Source at once, for the catalog, the ordering, and the delete.
A Source nothing cites is absent from the map rather than present at zero.

The unit is the citing Example, which is the only thing that cites at all.
It is not the number of rows `LReferenceUsageRead(id)` returns, because that list also names each citing card as the way up to the citation.
Counting cards and Examples together would count one citation twice, from two directions.
So the badge showing this number says what it counts, through `Source.Tally`, and the Example rows of the list are what it agrees with.

## `public IReadOnlyList<LUsage> LReferenceUsageRead(long id)`

Reads the citing Meanings, Collocations and Examples of one Source, itemized, each with the id the row leads to.
A card names its own id, the Entry it belongs to, that Entry's headword, and its title with the definition or expression behind it.
An Example names its sentence and its translation, because an Example belongs to no Entry of its own.
A card holding two Examples of one Source is listed once, because the row leads to the card rather than the citation.

## `internal static int LReferenceUsageRead(SqliteConnection connection, long id)`

Counts the citations of one Source inside a session the caller already holds.
`LReferenceDelete` reads it after detaching, so the count and the delete stand in one session.

## `internal static void LReferenceUsageClear(SqliteConnection connection, long id)`

Drops every citation of one Source.
An Example is not deleted with the Source, so its citation is cleared back to Unspecified.
The cards citing through those Examples lose the Source with them, because they never held it themselves.
