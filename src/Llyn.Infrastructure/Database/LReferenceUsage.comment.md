# LReferenceUsage.cs

## `public sealed class LReferenceUsage`

Reads the citing side of a Source: which Entries and Examples cite it, and how many.
It lives beside `LReferenceArchive` rather than inside it.
Counting the citations of a Source is a different responsibility from storing one.
Nothing here creates, changes, or deletes a Source.

A Situation is not counted, because a Situation is written rather than quoted and carries no Source column.
The two citing sides are shaped differently in the schema.
An Entry cites through the `entry_source` table and an Example through its own `source_id` column.
So detaching means deleting a row on one side and clearing two columns on the other.

## `public LReferenceUsage(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyDictionary<string, int> LReferenceUsageRead()`

Counts the citations of every Source at once, for the catalog, the ordering, and the delete.
A Source nothing cites is absent from the map rather than present at zero.

## `public IReadOnlyList<LUsage> LReferenceUsageRead(string id)`

Reads the citing Entries and Examples of one Source, itemized, each with the id the row leads to.
An Entry names its headword and its first sense, and an Example names its sentence and its translation.

## `internal static int LReferenceUsageRead(SqliteConnection connection, string id)`

Counts the citations of one Source inside a session the caller already holds.
`LReferenceDelete` reads it after detaching, so the count and the delete stand in one session.

## `internal static void LReferenceUsageClear(SqliteConnection connection, string id)`

Drops every citation of one Source, on both sides at once.
An Example is not deleted with the Source, so its citation is cleared back to Unspecified.
