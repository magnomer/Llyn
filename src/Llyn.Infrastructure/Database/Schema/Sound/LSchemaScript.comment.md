# LSchemaScript.cs

## `public static class LSchemaScript`

Creates the script store, one row per glyph picture of one character in one style.

## `public static void LSchemaScriptCreate(SqliteConnection connection)`

The row is keyed by language, character, style and position, so a refetch replaces a character's set cleanly.
It hangs off no entry, because a character is shared by every entry written with it.
The picture bytes sit in the row as the original the source drew.
The caption is stored without the chronology the source printed at its head, and `epoch` holds that age's code.
The age is matched once, when the picture is fetched, so nothing is matched or fetched again to print it.

## `public static void LSchemaScriptSettle(SqliteConnection connection, string schema, long stored)`

Dates, in `schema`, every picture carried from a workspace older than the note, whose caption still holds its age.
Such rows were fetched before an age had a code of its own, so their captions are read again here.
Each row is cut by the pack labels of the style it names.
Only a row that was cut is written.
No picture is fetched again, because the caption a row already holds is all the cutting needs.
A caption no label matches is left exactly as it was.
A later pack cuts it once it lists that age.
