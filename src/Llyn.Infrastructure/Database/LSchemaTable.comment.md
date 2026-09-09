# LSchemaTable.cs

## `public static class LSchemaTable`

The steps that create a table an older database never had.
Nothing is copied and nothing is rebuilt, because no older row said anything the new table holds.
LSchema's own CREATE statements usually get there first on startup.
But a step here is what makes each arrival explicit at the version that introduced it.

## `public static void LSchemaAudioNormalize(SqliteConnection connection)`

Creates the pronunciation_audio table on a database that predates it.
The rows already stored are untouched.
A pronunciation with no recording simply has no row.

## `public static void LSchemaFavoriteCreate(SqliteConnection connection)`

Creates the favorite table and the index over its mark time.
No older row carries a mark, so nothing is copied into it.

## `public static void LSchemaRegisterCreate(SqliteConnection connection)`

Creates the register table and the two link tables a Meaning and a Collocation reach it by.
Both links carry a position, so a card holds its registers in order.
