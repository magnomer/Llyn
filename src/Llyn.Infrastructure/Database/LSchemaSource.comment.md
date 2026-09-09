# LSchemaSource.cs

## `public static class LSchemaSource`

Version 31.
Retires the Reference program and channel columns and puts a kind and a note in their place.
Program name and channel name were one medium's metadata standing as columns every other medium left empty.

## `public static void LSchemaSourceNormalize(SqliteConnection connection)`

Rebuilds the source table around title, year, kind, note and url.
A source row already carrying a kind column is the new shape, so the step returns untouched.
The retired program and channel names are joined into the note, because a memo is where such text now belongs.
A row that stated neither keeps an unspecified note rather than an empty one.
Every kind starts unspecified, because no stored column ever said what the material was.

## Inline notes

### `using (SqliteCommand off = connection.CreateCommand())`

Foreign-key enforcement has to be off across a table rebuild.
It cannot be switched inside a transaction.
Hence the explicit statements rather than a session.
