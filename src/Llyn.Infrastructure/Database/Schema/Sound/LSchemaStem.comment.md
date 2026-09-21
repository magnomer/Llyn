# LSchemaStem.cs

## `public static class LSchemaStem`

The tables the phonetic series are browsed by.
A series row is shared by every character the same series names.

## `public static void LSchemaStemCreate(SqliteConnection connection)`

Creates the `stem` table and the `shengfu_stem` link table.
A series is unique per language and key, so one key never stands twice in a language.
The link cascades from both sides, so a dropped character or series leaves no link behind.
