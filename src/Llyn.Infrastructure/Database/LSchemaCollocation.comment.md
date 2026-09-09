# LSchemaCollocation.cs

## `public static class LSchemaCollocation`

Creates the collocations an Entry owns and the single Note it owns.

## `public static void LSchemaCollocationCreate(SqliteConnection connection)`

A collocation is a stable-id row ordered within its entry.
It carries both an expression and the meaning that explains it, and the card has a field for each.
A meaning carries a definition instead.
Reordering rewrites position only.
A collocation's synonym is an interlink, not owned text.
It uses the same discriminated target model as a relation, an Entry XOR a Meaning.
The XOR is enforced by a check constraint.
Each target column is a checked foreign key.
So the referenced row must exist and is never touched by the link.
The note table has no id and no position.
entry_id is its primary key.
That makes at-most-one Note per entry a schema fact.
Everything here cascades when its entry is deleted, and a synonym cascades when its collocation is deleted.
