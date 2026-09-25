# LSchemaCollocation.cs

## `public static class LSchemaCollocation`

Creates the collocations an Entry owns and the single Note it owns.

## `public const string LSchemaCollocationSequence =`

The next id a new Meaning or Collocation takes, one past the highest either table has ever used.
A draft and every card request name a card by its id alone, across both lists.
So no Collocation may share an id with a Meaning.
The two sequences are read too, so a deleted row's number is never handed out again.
Both inserts give this id explicitly, which also moves the inserting table's own sequence.

## `public static void LSchemaCollocationCreate(SqliteConnection connection)`

A collocation is a stable-id row ordered within its entry.
Its id counts strictly upward and is never given to a later collocation once the row is gone.
It is drawn from the space it shares with meanings, so no Meaning holds the same number.
A revision names a deleted collocation by that id.
A reused number would point the history at a living row.
It carries both an expression and the meaning that explains it, and the card has a field for each.
A meaning carries a definition instead.
Reordering rewrites position only.
The note table has no id and no position.
entry_parent is its primary key.
That makes at-most-one Note per entry a schema fact.
Everything here cascades when its entry is deleted.

## `public static void LSchemaCollocationSettle(SqliteConnection connection, string into)`

Gives every Collocation of an older workspace whose id a Meaning also holds a fresh id above both sequences.
It runs in the migration on the file being built, `into`, with foreign keys off.
Every table whose foreign key names a Collocation follows the new id, found from the schema rather than listed.
The revision rows naming that Collocation follow it too, so the history still points at the same row.
The collocation sequence ends at the last id given, so a later insert starts above it.
A workspace with no shared id is left alone.

## Inline notes

### `private static IReadOnlyList<(string LSchemaTable, string LSchemaColumn)> LSchemaLinkRead(`

Every table and column in `schema` whose foreign key points at the collocation table.

### `private static void LSchemaLinkUpdate(`

Rewrites one id in one column of `table`.
A `kind` limits the rewrite to revision rows of that target type.
