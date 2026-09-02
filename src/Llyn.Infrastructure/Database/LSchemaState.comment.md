# LSchemaState.cs

## `public static class LSchemaState`

The schema step that gives every card field its state column.
A workspace written before card fields said why they stand empty carries the value alone.
There a column holding nothing can only be read as nothing recorded.

## `public static void LSchemaStateNormalize(SqliteConnection connection)`

Rebuilds `sense`, `collocation`, `example` and `situation` with a state column beside every card value.
It carries the rows over.
A column holding text becomes specified, and an empty one nothing recorded.
Nothing becomes unknown.
A value that cannot be read back is something only a later reading finds.
It is never something a migration invents.

The rebuild runs with foreign keys off and inside one transaction.
The tables it replaces are pointed at by association rows that must survive the swap.
It does nothing when the state columns already stand.
