# LSchemaMeaning.cs

## `public static class LSchemaMeaning`

Creates the Meaning tree an Entry owns.

## `public static void LSchemaMeaningCreate(SqliteConnection connection)`

Each meaning is a stable-id node that may nest under another meaning in the same entry.
It carries a single inline definition field.
A meaning cascades when its parent meaning is deleted and when its Entry is deleted.
Sibling ordering is unique within a parent.
The ifnull() expression index treats root meanings (null parent) as one sibling group.
