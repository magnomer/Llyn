# LTagArchive.cs

## `public sealed class LTagArchive`

Persists Tags — independent data no Entry, Meaning, or Collocation owns. A Tag is created once with an opaque id, then *referenced* by any number of Meanings and Collocations through the association tables, each carrying the position the Tag takes for that referrer alone. Attaching and detaching therefore only ever write association rows: detaching leaves the Tag and its other references untouched, updating rewrites the visible text and never the id, and `LTagDelete` refuses to run while any reference remains.

A referrer's order is a unique index, so attaching and detaching renumber that referrer's whole set through `LDatabaseOrder`: a caller names the index it wants and never has to find a free position or leave a gap behind.

## `public LTagArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LTag LTagCreate(LTag tag)`

Inserts `tag` with a fresh opaque id and returns the stored Tag with that id filled in. The new Tag is referenced by nothing until it is attached to a referrer.

## `public LTag? LTagRead(string id)`

Reads the Tag identified by `id`, or `null` when no such Tag exists.

## `public IReadOnlyList<LTag> LTagSenseRead(string senseId)`

Reads the Tags a Meaning references, in the order that Meaning gives them.

## `public IReadOnlyList<LTag> LTagCollocationRead(string collocationId)`

Reads the Tags a Collocation references, in the order that Collocation gives them.

## `public void LTagUpdate(LTag tag)`

Rewrites the visible text of the Tag identified by `tag`'s id. The id and every reference pointing at it are untouched, so an update never changes where the Tag appears or in what order. Throws when no Tag carries that id.

## `public int LTagReferenceRead(string id)`

Counts the references that still point at the Tag identified by `id` — the number `LTagDelete` refuses a delete over. A caller that has just detached one reference reads this to learn whether the row it detached from was the last one, without a store of its own having to know which association tables exist.

## `public void LTagDelete(string id)`

Deletes the Tag identified by `id`. Guarded: while any Meaning or Collocation still references the Tag, nothing is deleted and an `InvalidOperationException` is thrown — detach every reference first. Deleting a Tag never deletes the rows that referenced it. The guard and the delete share one transaction, so nothing can attach the Tag between them.

## `public void LTagSenseAttach(string senseId, string tagId, int position)`

References an existing Tag from a Meaning at `position` in that Meaning's order.

## `public void LTagCollocationAttach(string collocationId, string tagId, int position)`

References an existing Tag from a Collocation at `position` in that Collocation's order.

## `public void LTagSenseDetach(string senseId, string tagId)`

Removes a Meaning's reference to a Tag. The Tag and its other references survive.

## `public void LTagCollocationDetach(string collocationId, string tagId)`

Removes a Collocation's reference to a Tag. The Tag and its other references survive.

## Inline notes

### `private static int LTagReferenceRead(SqliteConnection connection, string id)`

The same count on a connection the caller already holds, so a guard and the delete it guards run in one transaction and nothing can attach the row between them.

### `private void LTagReferenceAttach(string table, string column, string referrerId, string tagId, int position)`

The two association tables differ only in their name and their referrer column, so the reference operations share one implementation each. Both identifiers are store-owned literals chosen by the methods above, never caller input, so composing them into the statement text opens no injection seam; every value still travels as a parameter.

The row goes in beyond the end of the set and the whole set is then renumbered around it, so the requested index is honoured and an occupied position is no longer a unique-index failure.
