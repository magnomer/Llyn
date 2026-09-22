# LReflexArchive.cs

## `public sealed class LReflexArchive`

Persists the ordered reflexes an entry owns, one row per reading.
A reflex's id is assigned here on insertion and survives every later save.
So a draft and a shell may hold that id across saves.
The whole list of an entry is written at once.
The rows are few, and their order is one fact.

## `public LReflexArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyList<LReflex> LReflexRead(long entryId)`

Reads the entry's reflexes in position order, empty when it has none.
The eight anatomy columns come back as one `LAnatomy` on each row, and the anchors as their sorted ids.

## `public IReadOnlyDictionary<long, IReadOnlyList<LReflex>> LReflexAnchorScan(long diweiId)`

The reflex rows anchored to each fanqie row of one Diwei category, keyed by the fanqie id.
A fanqie row nothing is anchored to has no key, so a tally over it counts nothing.
Each row carries its own entry id and its anchors, as a read would give it.

## `private static LReflex LReflexRowRead(SqliteDataReader reader, long entryId)`

One reflex row from the twenty columns every reflex select lists in the same order.

## `private static IReadOnlyList<LReflex> LReflexAnchorLoad(SqliteConnection connection, List<LReflex> rows)`

The rows with their anchors read, one small query per row.

## `private static IReadOnlyList<long> LReflexAnchorRead(SqliteConnection connection, long reflexId)`

The fanqie ids one reflex row is anchored to, ascending.

## `private static void LReflexAnchorSave(SqliteConnection connection, LReflex row)`

Rewrites the anchors of one stored row: every old pair is dropped and each id of the row is written.
An id no fanqie row carries is skipped, so a stale draft never breaks the save.

## `private static LAnatomy LReflexAnatomyRead(SqliteDataReader reader)`

The anatomy of one row, from the eight columns after the region in the select order.

## `public IReadOnlyList<LReflex> LReflexSet(long entryId, IReadOnlyList<LReflex> reflexes)`

Makes `reflexes` the whole list of the entry, in the order given.
A row with a positive id is rewritten in place under that id, so the id survives the save.
A row with no id is inserted and given one.
A stored row the list no longer names is deleted.
Returns the stored rows with their ids and positions filled in.
The anchors of each row are rewritten after the row, under the id the row now has.
The whole write is one transaction.
It throws when a positive id names no row of this entry.
No column is unique, so no row needs shelving before the write.

## `private static void LReflexLeftoverDelete(SqliteConnection connection, long entryId, HashSet<long> kept)`

Deletes each stored row of the entry whose id the replacement list did not keep.

## `private static LReflex LReflexRowSave(SqliteConnection connection, LReflex row)`

Updates one existing row and refuses an id that belongs to no row of the entry.

## `private static LReflex LReflexInsert(SqliteConnection connection, LReflex row)`

Inserts one new row and returns it with its minted id.

## `private static void LReflexParameterApply(SqliteCommand command, LReflex row)`

Adds the column values of one row to a write, shared by the update and the insert.

## `private static void LReflexAnatomyApply(SqliteCommand command, LAnatomy anatomy)`

Adds the eight anatomy values of one row to a write, after the other columns.
