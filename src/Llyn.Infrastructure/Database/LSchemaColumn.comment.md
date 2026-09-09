# LSchemaColumn.cs

## `public static class LSchemaColumn`

The steps that add or drop a column on a table that stands.
None of them needs a rebuild, so existing rows keep everything they have.
Each one is guarded by a read of the current shape, because `ALTER TABLE` has no `IF NOT EXISTS`.

## `public static void LSchemaTitleNormalize(SqliteConnection connection, string table)`

Gives a card table the title column its template has always had a field for.
Adding a column needs no table rebuild.
So existing rows keep everything they have and read back a NULL title, which is what they had.
Skipped when the column is already there.

## `public static void LSchemaCollocationNormalize(SqliteConnection connection)`

Gives the collocation table the meaning column its card has always had a field for.
Adding a column needs no table rebuild.
So the existing rows, and their expressions, are untouched.
The meaning of a collocation written before this version reads back as NULL, which is what it was.

## `public static void LSchemaVideoNormalize(SqliteConnection connection)`

Gives the video table the span of it worth watching, as a state column and a text column.
A database already carrying the span column is left alone.
The columns are added rather than the table rebuilt, since nothing already stored has to move.

## `public static void LSchemaSituationNormalize(SqliteConnection connection)`

Drops the two columns a Situation used to hold a Source in.
A Situation carries no Source field, so neither column names anything.
Each column is dropped only when it is there, because the step may have run before.
