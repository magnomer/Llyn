# LSchemaFavorite.cs

## `public static class LSchemaFavorite`

Creates the favorite marks, one row per marked Entry.

## `public static void LSchemaFavoriteCreate(SqliteConnection connection)`

The mark is no lexical object, so it holds nothing but the entry it stands on and its stamp.
The cascade drops the mark with the entry it marks.
The stamp is indexed because the favorites panel orders by it.
