# LSituationArchive.cs

## `public sealed partial class LSituationArchive`

Persists Situations — independent data no Entry, Meaning, or Collocation owns.
A Situation is created once with an opaque id.
It is then *referenced* by any number of Meanings and Collocations through the association tables.
Each association carries the position the Situation takes for that referrer alone.
Attaching and detaching therefore only ever write association rows.
Detaching leaves the Situation and its other references untouched.
Updating rewrites the visible title, description, and kind, and never the id.
`LSituationDelete` refuses to run while any reference remains.

A Situation shows Images and Videos the way a Meaning does, through `situation_image` and `situation_video`.
Every read fills the two lists, and create and update write them back.
The Image and Video rows themselves go through `LImageArchive` and `LVideoArchive`.
This store only settles which rows the Situation references and in what order.

A referrer's order is a unique index.
So attaching and detaching renumber that referrer's whole set through `LDatabaseOrder`.
A caller names the index it wants.
It never has to find a free position or leave a gap behind.

## `public LSituationArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LSituation LSituationCreate(LSituation situation)`

Inserts `situation` with a fresh opaque id and returns the stored Situation with that id filled in.
The new Situation is referenced by nothing until it is attached to a referrer.
Its Images and Videos are not written here.
The engine settles them after the row exists, as it does for a card.
The stored rows come back with their ids.

## `public LSituation? LSituationRead(long id)`

Reads the Situation identified by `id`, or `null` when no such Situation exists.

## `public IReadOnlyList<LSituation> LSituationMeaningRead(long meaningId)`

Reads the Situations a Meaning references, in the order that Meaning gives them.

## `public IReadOnlyList<LSituation> LSituationCollocationRead(long collocationId)`

Reads the Situations a Collocation references, in the order that Collocation gives them.

## `public void LSituationUpdate(LSituation situation)`

Rewrites the visible title, description, and kind of the Situation identified by `situation`'s id.
The Images and Videos are the engine's to settle, through the attach and detach the media stores offer.
The id and every reference pointing at it are untouched.
So an update never changes where the Situation appears or in what order.
Throws when no Situation carries that id.

## `public int LSituationReferenceRead(long id)`

Counts the references that still point at the Situation identified by `id` — the number `LSituationDelete` refuses a delete over.
A caller that has just detached one reference reads this.
It learns whether the row it detached from was the last one.
No store of its own has to know which association tables exist.

## Inline notes

### `private static int LSituationReferenceRead(SqliteConnection connection, long id)`

The same count on a connection the caller already holds.
So a guard and the delete it guards run in one transaction.
Nothing can attach the row between them.

## `public IReadOnlyList<LSituation> LSituationRead()`

Every Situation the workspace holds, in the order they were written.
It includes one nothing references, which is reachable nowhere else.

## `public IReadOnlyDictionary<string, int> LSituationReferenceRead()`

How many places reference each Situation, the whole shelf in one statement.
A panel listing the catalog needs the figure on every row.
One query per row is a query per row.
A Situation nothing references is absent rather than present as zero.

## `public IReadOnlyList<LUsage> LSituationUsageRead(long id)`

The referring sides of one Situation, named rather than counted.
A Meaning is named by its title and, standing without one, by its definition.
A Collocation is named by its title and, standing without one, by its expression.
The Entry each side belongs to is read with it, so a row is legible without a second query.

## `public void LSituationDelete(long id, bool detach)`

Deletes the Situation, first dropping every reference to it when `detach` is asked for.
Detaching, counting and deleting share one session.
Between any two of them the answer to whether something still references the row can change.
Without `detach` the count still refuses the delete, which is the guard a card edit relies on.
