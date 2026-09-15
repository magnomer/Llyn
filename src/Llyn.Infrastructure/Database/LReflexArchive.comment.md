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

## `public IReadOnlyList<LReflex> LReflexSet(long entryId, IReadOnlyList<LReflex> reflexes)`

Makes `reflexes` the whole list of the entry, in the order given.
A row with a positive id is rewritten in place under that id, so the id survives the save.
A row with no id is inserted and given one.
A stored row the list no longer names is deleted.
Returns the stored rows with their ids and positions filled in.
The whole write is one transaction.
It throws when a positive id names no row of this entry.
No column is unique, so no row needs shelving before the write.

## `private static void LReflexParameterApply(SqliteCommand command, LReflex row)`

Adds the column values of one row to a write, shared by the update and the insert.
