# LSchemaCollocation.cs

## `public static class LSchemaCollocation`

Creates the collocations an Entry owns and the single Note it owns.

## `public static void LSchemaCollocationCreate(SqliteConnection connection)`

A collocation is a stable-id row ordered within its entry.
Its id counts strictly upward and is never given to a later collocation once the row is gone.
A revision names a deleted collocation by that id, so a reused number would point the history at a living row.
It carries both an expression and the meaning that explains it, and the card has a field for each.
A meaning carries a definition instead.
Reordering rewrites position only.
The note table has no id and no position.
entry_parent is its primary key.
That makes at-most-one Note per entry a schema fact.
Everything here cascades when its entry is deleted.
