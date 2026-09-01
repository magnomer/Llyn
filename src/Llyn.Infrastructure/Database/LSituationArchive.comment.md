# LSituationArchive.cs

## `public sealed class LSituationArchive`

Persists Situations — independent data no Entry, Meaning, or Collocation owns. A Situation is created once with an opaque id, then *referenced* by any number of Meanings and Collocations through the association tables, each carrying the position the Situation takes for that referrer alone. Attaching and detaching therefore only ever write association rows: detaching leaves the Situation and its other references untouched, updating rewrites the visible title, description, and kind and never the id, and `LSituationDelete` refuses to run while any reference remains.

A referrer's order is a unique index, so attaching and detaching renumber that referrer's whole set through `LDatabaseOrder`: a caller names the index it wants and never has to find a free position or leave a gap behind.

## `public LSituationArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LSituation LSituationCreate(LSituation situation)`

Inserts `situation` with a fresh opaque id and returns the stored Situation with that id filled in. The new Situation is referenced by nothing until it is attached to a referrer.

## `public LSituation? LSituationRead(string id)`

Reads the Situation identified by `id`, or `null` when no such Situation exists.

## `public IReadOnlyList<LSituation> LSituationSenseRead(string senseId)`

Reads the Situations a Meaning references, in the order that Meaning gives them.

## `public IReadOnlyList<LSituation> LSituationCollocationRead(string collocationId)`

Reads the Situations a Collocation references, in the order that Collocation gives them.

## `public void LSituationUpdate(LSituation situation)`

Rewrites the visible title, description, and kind of the Situation identified by `situation`'s id. The id and every reference pointing at it are untouched, so an update never changes where the Situation appears or in what order. Throws when no Situation carries that id.

## `public int LSituationReferenceRead(string id)`

Counts the references that still point at the Situation identified by `id` — the number `LSituationDelete` refuses a delete over. A caller that has just detached one reference reads this to learn whether the row it detached from was the last one, without a store of its own having to know which association tables exist.

## `public void LSituationDelete(string id)`

Deletes the Situation identified by `id`. Guarded: while any Meaning or Collocation still references the Situation, nothing is deleted and an `InvalidOperationException` is thrown — detach every reference first. Deleting a Situation never deletes the rows that referenced it. The guard and the delete share one transaction, so nothing can attach the Situation between them.

## `public void LSituationSenseAttach(string senseId, string situationId, int position)`

References an existing Situation from a Meaning at `position` in that Meaning's order.

## `public void LSituationCollocationAttach(string collocationId, string situationId, int position)`

References an existing Situation from a Collocation at `position` in that Collocation's order.

## `public void LSituationSenseDetach(string senseId, string situationId)`

Removes a Meaning's reference to a Situation. The Situation and its other references survive.

## `public void LSituationCollocationDetach(string collocationId, string situationId)`

Removes a Collocation's reference to a Situation. The Situation and its other references survive.

## Inline notes

### `private static int LSituationReferenceRead(SqliteConnection connection, string id)`

The same count on a connection the caller already holds, so a guard and the delete it guards run in one transaction and nothing can attach the row between them.

### `private void LSituationReferenceAttach(`

The two association tables differ only in their name and their referrer column, so the reference operations share one implementation each. Both identifiers are store-owned literals chosen by the methods above, never caller input, so composing them into the statement text opens no injection seam; every value still travels as a parameter.

The row goes in beyond the end of the set and the whole set is then renumbered around it, so the requested index is honoured and an occupied position is no longer a unique-index failure.
