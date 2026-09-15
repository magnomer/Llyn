# LSchemaFanqie.cs

## `public static class LSchemaFanqie`

Creates the fanqie store, one row per placement of one character in one rime book.

## `public static void LSchemaFanqieCreate(SqliteConnection connection)`

The row is keyed by language, character, book, source and position, so a refetch replaces a character's set cleanly.
Two sources of one book each count their positions from zero, so the source is part of the key.
It hangs off no entry, because a character is shared by every entry written with it.
The text is the line the reading view prints as it stands.
The parts it was built from sit beside it, so a hypothesis can derive a reading without a refetch.
The source names the site the row came from, since one book may be read from several.
The spelling is the 反切 the site printed, or empty.
