# LExampleVault.cs

## `public interface LExampleVault`

The persistence port for the Example rows the engine reads and writes.
It lists exactly what the engine asks of example storage, and nothing about how rows are kept.
`LExampleArchive` in Infrastructure is its adapter over the workspace database.

## `LExample LExampleCreate(LExample example);`

Inserts `example` with a fresh opaque id and returns the stored Example with that id filled in.
The new Example is referenced by nothing until it is attached to a referrer.
An Example's language is optional and an empty one is stored as written.
A sentence may be written in two languages, so no one language can be demanded of it.

## `LExample? LExampleRead(long id);`

Reads the Example identified by `id`, or `null` when no such Example exists.

## `IReadOnlyList<LExample> LExampleRead();`

Every stored Example, in insertion order.
One statement answers for the whole table, rather than one per Example.

## `void LExampleUpdate(LExample example);`

Rewrites the Example identified by `example`'s id.
It rewrites its language, text, translation, Source reference, and Mention list.
The Example's own id and every reference pointing at it are untouched.
So an update never changes where the Example appears or in what order.
Throws when no Example carries that id.

## `void LExampleTextUpdate(long exampleId, LStateValue text);`

Rewrites only the text of the Example identified by `exampleId`.
A Mention whose span no longer fits the new text is dropped, and the rest are kept as they stand.
Shifting a span to follow an edit is the engine's work, not the store's.
Throws when no Example carries that id.

## `void LExampleSourceUpdate(long exampleId, LStateAnchor source);`

Sets or clears the single Source the Example identified by `exampleId` cites — pass `null` for `sourceId` to clear it.
Only the reference moves: the Source row itself is never created, changed, or removed here.
Throws when no Example carries that id.

## `int LExampleReferenceRead(long id);`

Counts the references that still point at the Example identified by `id` — the number `LExampleDelete` refuses a delete over.
A caller that has just detached one reference reads this.
It learns whether the row it detached from was the last one.
No store of its own has to know which association tables exist.

## `IReadOnlyDictionary<long, int> LExampleReferenceRead();`

How many rows reference each Example, counted across the three association tables at once.
The browsing panel needs the figure for every row it lists, so one statement answers for all of them.

## `void LExampleDelete(long id);`

Deletes the Example identified by `id`.
Guarded: while any Meaning or Collocation still references the Example, nothing is deleted.
An `InvalidOperationException` is thrown instead.
Detach every reference first.
A Source the Example cited is left standing.
Only the reference to it disappears with the row.
The guard and the delete share one transaction, so nothing can attach the Example between them.

## `void LExampleDelete(long id, bool detach);`

Deletes the Example, clearing every reference to it first when `detach` is set.
The clearing, the count and the delete share one session.
So the row is judged against the references as they stand at that moment.
The Source it cited is left standing, because a citation is a pointer and never ownership.

## `IReadOnlyList<LUsage> LExampleUsageRead(long id);`

Every card quoting one Example, itemized rather than counted.
A Meaning or Collocation row names the Entry it belongs to.
A row carries the entry id it is followed through.
It stays followable after the text it shows is edited.
A quoting card with no wording of its own falls back to the definition or expression beneath it.
