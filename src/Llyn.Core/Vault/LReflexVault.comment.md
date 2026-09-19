# LReflexVault.cs

## `public interface LReflexVault`

The persistence port for the Reflex rows the engine reads and writes.
It lists exactly what the engine asks of reflex storage, and nothing about how rows are kept.
`LReflexArchive` in Infrastructure is its adapter over the workspace database.

## `IReadOnlyList<LReflex> LReflexRead(long entryId);`

Reads the entry's reflexes in position order, empty when it has none.
The eight anatomy columns come back as one `LAnatomy` on each row, and the anchors as their sorted ids.

## `IReadOnlyDictionary<long, IReadOnlyList<LReflex>> LReflexAnchorScan(long diweiId);`

The reflex rows anchored to each fanqie row of one Diwei category, keyed by the fanqie id.
A fanqie row nothing is anchored to has no key, so a tally over it counts nothing.
Each row carries its own entry id and its anchors, as a read would give it.

## `IReadOnlyList<LReflex> LReflexSet(long entryId, IReadOnlyList<LReflex> reflexes);`

Makes `reflexes` the whole list of the entry, in the order given.
A row with a positive id is rewritten in place under that id, so the id survives the save.
A row with no id is inserted and given one.
A stored row the list no longer names is deleted.
Returns the stored rows with their ids and positions filled in.
The anchors of each row are rewritten after the row, under the id the row now has.
The whole write is one transaction.
It throws when a positive id names no row of this entry.
No column is unique, so no row needs shelving before the write.
