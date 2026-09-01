# LExampleArchive.cs

## `public sealed class LExampleArchive`

Persists Examples — the first entity no Entry, Meaning, or Collocation owns. An Example is created once with an opaque id, then *referenced* by any number of referrers through the association tables, each carrying the position the Example takes for that referrer alone. Attaching and detaching therefore only ever write association rows: detaching leaves the Example and its other references untouched, and `LExampleDelete` refuses to run while any reference remains. Translations are owned text, rewritten wholesale on update and removed with the Example; the single Source an Example cites is a reference only and is never created, updated, or deleted from here.

The association tables live in `LExampleLink`, which owns attaching, detaching, and reading an Example set by referrer; this file owns the Example itself.

## `public LExampleArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LExample LExampleCreate(LExample example)`

Inserts `example` and its translations with fresh opaque ids and returns the stored Example with those ids filled in. The new Example is referenced by nothing until it is attached to a referrer.

## `public LExample? LExampleRead(string id)`

Reads the Example identified by `id` with its translations in order, or `null` when no such Example exists.

## `public void LExampleUpdate(LExample example)`

Rewrites the Example identified by `example`'s id: its language, text, local rendering, and Source reference, plus its translations, which are replaced wholesale. A translation that arrives with an id keeps it, so an id a caller is holding stays valid; only a translation without one is given a fresh id. The Example's own id and every reference pointing at it are untouched, so an update never changes where the Example appears or in what order. Throws when no Example carries that id.

## `public void LExampleSourceUpdate(string exampleId, string? sourceId)`

Sets or clears the single Source the Example identified by `exampleId` cites — pass `null` for `sourceId` to clear it. Only the reference moves: the Source row itself is never created, changed, or removed here. Throws when no Example carries that id.

## `public int LExampleReferenceRead(string id)`

Counts the references that still point at the Example identified by `id` — the number `LExampleDelete` refuses a delete over. A caller that has just detached one reference reads this to learn whether the row it detached from was the last one, without a store of its own having to know which association tables exist.

## `public void LExampleDelete(string id)`

Deletes the Example identified by `id` together with its translations. Guarded: while any Entry, Meaning, or Collocation still references the Example, nothing is deleted and an `InvalidOperationException` is thrown — detach every reference first. A Source the Example cited is left standing; only the reference to it disappears with the row. The guard and the delete share one transaction, so nothing can attach the Example between them.

## `internal static LExample? LExampleSingleRead(SqliteConnection connection, string id)`

Reads one Example with its translations on a connection the caller already holds. Shared with `LExampleLink`, which resolves a whole referrer's set at once.

## `internal static IReadOnlyDictionary<string, IReadOnlyList<LTranslation>> LExampleTranslationRead(`

Reads the translations of every Example a referrer names, in one query, grouped by Example id. Reading them one Example at a time is what turns a referrer's set into a round-trip per row.

## Inline notes

### `private static int LExampleReferenceRead(SqliteConnection connection, string id)`

The same count on a connection the caller already holds, so a guard and the delete it guards run in one transaction and nothing can attach the row between them.
