# LSchemaSpeech.cs

## `public static class LSchemaSpeech`

Version 16.
The part-of-speech field is editable, so a row may now carry text no preset names.

## `public static void LSchemaSpeechNormalize(SqliteConnection connection)`

part_of_speech rebuilt to hold a custom part of speech beside a declared one.
Nullability and a CHECK are table-shape facts.
So neither ADD COLUMN nor anything short of SQLite's documented rebuild can deliver them.
The table is one an earlier build created.
The assignments already stored are all preset ids and copy across untouched.

## Inline notes

### `using (SqliteCommand off = connection.CreateCommand())`

Foreign-key enforcement has to be off across a table rebuild.
It cannot be switched inside a transaction.
Hence the explicit statements rather than a session.
