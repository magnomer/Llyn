# LSituationArchive.cs

## `public sealed class LSituationArchive`

Persists Situations — independent data no Entry, Meaning, or Collocation owns.
A Situation is created once with an opaque id.
It is then *referenced* by any number of Meanings and Collocations through the association tables.
Each association carries the position the Situation takes for that referrer alone.
Attaching and detaching therefore only ever write association rows.
Detaching leaves the Situation and its other references untouched.
Updating rewrites the visible title, description, and kind, and never the id.
`LSituationDelete` refuses to run while any reference remains.

A referrer's order is a unique index.
So attaching and detaching renumber that referrer's whole set through `LDatabaseOrder`.
A caller names the index it wants.
It never has to find a free position or leave a gap behind.

## `public LSituationArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LSituation LSituationCreate(LSituation situation)`

Inserts `situation` with a fresh opaque id and returns the stored Situation with that id filled in.
The new Situation is referenced by nothing until it is attached to a referrer.

## `public LSituation? LSituationRead(string id)`

Reads the Situation identified by `id`, or `null` when no such Situation exists.

## `public IReadOnlyList<LSituation> LSituationSenseRead(string senseId)`

Reads the Situations a Meaning references, in the order that Meaning gives them.

## `public IReadOnlyList<LSituation> LSituationCollocationRead(string collocationId)`

Reads the Situations a Collocation references, in the order that Collocation gives them.

## `public void LSituationUpdate(LSituation situation)`

Rewrites the visible title, description, and kind of the Situation identified by `situation`'s id.
The id and every reference pointing at it are untouched.
So an update never changes where the Situation appears or in what order.
Throws when no Situation carries that id.

## `public int LSituationReferenceRead(string id)`

Counts the references that still point at the Situation identified by `id` — the number `LSituationDelete` refuses a delete over.
A caller that has just detached one reference reads this.
It learns whether the row it detached from was the last one.
No store of its own has to know which association tables exist.

## `public void LSituationDelete(string id)`

Deletes the Situation identified by `id`.
Guarded: while any Meaning or Collocation still references the Situation, nothing is deleted.
An `InvalidOperationException` is thrown instead.
Detach every reference first.
Deleting a Situation never deletes the rows that referenced it.
The guard and the delete share one transaction, so nothing can attach the Situation between them.

## `public void LSituationSenseAttach(string senseId, string situationId, int position)`

References an existing Situation from a Meaning at `position` in that Meaning's order.

## `public void LSituationCollocationAttach(string collocationId, string situationId, int position)`

References an existing Situation from a Collocation at `position` in that Collocation's order.

## `public void LSituationSenseDetach(string senseId, string situationId)`

Removes a Meaning's reference to a Situation.
The Situation and its other references survive.

## `public void LSituationCollocationDetach(string collocationId, string situationId)`

Removes a Collocation's reference to a Situation.
The Situation and its other references survive.

## Inline notes

### `private static int LSituationReferenceRead(SqliteConnection connection, string id)`

The same count on a connection the caller already holds.
So a guard and the delete it guards run in one transaction.
Nothing can attach the row between them.

### `private void LSituationReferenceAttach(`

The two association tables differ only in their name and their referrer column.
So the reference operations share one implementation each.
Both identifiers are store-owned literals chosen by the methods above, never caller input.
So composing them into the statement text opens no injection seam.
Every value still travels as a parameter.

The row goes in beyond the end of the set and the whole set is then renumbered around it.
So the requested index is honoured.
An occupied position is no longer a unique-index failure.

## `public IReadOnlyList<LSituation> LSituationRead()`

Every Situation the workspace holds, in the order they were written.
It includes one nothing references, which is reachable nowhere else.

## `public IReadOnlyDictionary<string, int> LSituationReferenceRead()`

How many places reference each Situation, the whole shelf in one statement.
A panel listing the catalog needs the figure on every row, and one query per row is a query per row.
A Situation nothing references is absent rather than present as zero.

## `public IReadOnlyList<LUsage> LSituationUsageRead(string id)`

The referring sides of one Situation, named rather than counted.
A Meaning is named by its title and, standing without one, by its gloss.
A Collocation is named by its title and, standing without one, by its expression.
The Entry each side belongs to is read with it, so a row is legible without a second query.

## `public void LSituationDelete(string id, bool detach)`

Deletes the Situation, first dropping every reference to it when `detach` is asked for.
Detaching, counting and deleting share one session.
Between any two of them the answer to whether something still references the row can change.
Without `detach` the count still refuses the delete, which is the guard a card edit relies on.

## Inline notes

### `private static void LSituationLinkDelete(SqliteConnection connection, string table, string situationId)`

Drops one association table's references to a Situation and renumbers what each referrer has left.
The referrers are read before the delete because afterwards there is nothing left to name them.
A gap in a referrer's positions is a unique-index failure waiting for its next attach.
