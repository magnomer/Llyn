# LSchemaExample.cs

## `public static class LSchemaExample`

Every migration step that rebuilds the example table.
SQLite cannot add a foreign key or a column pair to a table that stands.
So both the version 12 constraint and the version 21 translation arrive as a rebuild beside the old table.

## `public static void LSchemaExampleNormalize(SqliteConnection connection)`

Gives example.source_id the foreign key it was declared with only once the source table existed.
Skipped when the constraint is already there, so this costs one pragma read on every later start.

## `public static void LSchemaExampleRebuild(SqliteConnection connection, string carried)`

Rebuilds example around a single translation column, filling it from the expression handed in.
Called only when the old local column is still there.

## `public static string LSchemaCarriedRead(SqliteConnection connection)`

The expression that decides what each Example's single translation becomes.
The hand-written local text wins, because a person wrote it for that sentence.
The first owned row stands in when no local text was written.
An upgraded workspace keeps one translation instead of none.
The table it reads is named example_rendition or example_translation depending on how old the file is.

## Inline notes

### `using (SqliteCommand off = connection.CreateCommand())`

Foreign-key enforcement has to be off across a table rebuild.
It cannot be switched inside a transaction.
Hence the explicit statements rather than a session.
