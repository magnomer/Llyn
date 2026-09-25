# LEtymologyVault.cs

## `public interface LEtymologyVault`

The persistence port for the etymology of an entry, in both of its shapes.
It lists what the engine asks of etymology storage, and nothing about how rows are kept.
`LEtymologyArchive` in Infrastructure is its adapter over the workspace database.

## `LEtymology? LEtymologyRead(long entryId);`

Reads the entry's narrative, or null when it keeps none.
The spans come back in offset order, as every Mention read does.

## `IReadOnlyList<LEtymon> LEtymologyEtymonRead(long entryId);`

Reads the entry's direct links in position order, empty when it has none.

## `LEtymology? LEtymologySave(long entryId, LEtymology? etymology);`

Makes `etymology` the whole narrative of the entry, spans and all.
A null or blank narrative deletes the stored one and returns null.
The stored spans are written fresh each time, so their ids do not survive the save.
It throws when a span falls outside the text, overlaps another, or names no Entry.
Returns the stored narrative with its ids filled in.

## `IReadOnlyList<LEtymon> LEtymologyEtymonSet(long entryId, IReadOnlyList<long> targetIds);`

Makes `targetIds` the whole list of the entry's direct links, in the order given.
A repeated target, a missing id and the entry itself are dropped rather than refused.
Returns the stored links with their ids and positions filled in.
