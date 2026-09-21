# LSchemaShengfu.cs

## `public static class LSchemaShengfu`

Creates the phonetic-series store, one row per character of one language.

## `public static void LSchemaShengfuCreate(SqliteConnection connection)`

The row is keyed by language and character, because a character has one series per language.
It hangs off no entry, because a character is shared by every entry written with it.
The text is the series as the reading view prints it, with several already joined.
The source names the site the series came from, since one language may be read from several.
