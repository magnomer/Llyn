# LSchemaIndex.cs

## `public static class LSchemaIndex`

Creates the lookup indexes the schema needs but does not get for free.
SQLite indexes a primary key and a unique constraint, and nothing else.
In particular it never indexes the *child* side of a foreign key.
With `PRAGMA foreign_keys = ON` every parent delete has to prove no child row points at it.
So an unindexed child column turns one delete into a full scan of that table.
The cascade of an Entry delete reaches a dozen of them.

Only the columns no existing key already covers are listed here.
A composite primary key indexes its leading column.
So `form (entry_id, position)` and `entry_example (entry_id, example_id)` need nothing for their first column.
It is the second column that needs an index of its own.
That is the one an association is searched by from the other direction.

## `public static void LSchemaIndexCreate(SqliteConnection connection)`

Creates every index that does not yet exist.
Called by `LSchema.LSchemaCreate` on each startup.
Existing indexes are left as they are.

## Inline notes

### `command.CommandText =`

Lexical rows owned by an Entry or a Meaning whose owning column carries no key of its own.

### `command.CommandText =`

The target side of every interlink: the column a delete of the referenced row must scan.

### `command.CommandText =`

The member side of every association table, and the shared entities those members name.

### `command.CommandText =`

Operational rows pointing at lexical rows and at history.

### `command.CommandText =`

The ordered sets that were left without a unique position.
They are a relation within its Meaning and a collocation within its Entry.
They are also a synonym within its Collocation and a translation within its Example.
Every other ordered set already has one.
Without it a duplicate position is silently possible and the read order becomes arbitrary.
Any duplicate an older database still holds is renumbered by the migration that precedes this call.
