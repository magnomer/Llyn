# LSchemaState.cs

## `public static class LSchemaState`

The schema step that gives every card field its state column. A workspace written before card fields said why they stand empty carries the value alone, where a column holding nothing can only be read as nothing recorded.

## `public static void LSchemaStateNormalize(SqliteConnection connection)`

Rebuilds `sense`, `collocation`, `example`, `tag` and `situation` with a state column beside every value the cards write, and carries the rows over: a column holding text becomes specified, an empty one nothing recorded. Nothing becomes unknown — a value that cannot be read back is something only a later reading finds, never something a migration invents.

The rebuild runs with foreign keys off and inside one transaction, because the tables it replaces are pointed at by association rows that must survive the swap. It does nothing when the state columns already stand.
