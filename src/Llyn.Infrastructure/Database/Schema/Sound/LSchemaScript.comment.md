# LSchemaScript.cs

## `public static class LSchemaScript`

Creates the script store, one row per glyph picture of one character in one style.

## `public static void LSchemaScriptCreate(SqliteConnection connection)`

The row is keyed by language, character, style and position, so a refetch replaces a character's set cleanly.
It hangs off no entry, because a character is shared by every entry written with it.
The picture bytes sit in the row as the original the source drew.
