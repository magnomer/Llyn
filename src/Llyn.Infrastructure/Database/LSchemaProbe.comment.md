# LSchemaProbe.cs

## `public static class LSchemaProbe`

Asks an open database what shape it already has.
A migration step decides whether it has anything to do by reading sqlite_master or pragma_table_info.
The two reads are shared, so they live apart from the steps that make them.

## `public static bool LSchemaTableFind(SqliteConnection connection, string table)`

Whether a table of that name stands in the database.

## `public static bool LSchemaColumnFind(SqliteConnection connection, string table, string column)`

Whether a table carries a column of that name.
`ALTER TABLE` has no `IF NOT EXISTS`, so this read is the guard before every column change.
