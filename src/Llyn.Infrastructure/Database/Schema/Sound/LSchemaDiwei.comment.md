# LSchemaDiwei.cs

## `public static class LSchemaDiwei`

Creates the diwei store: the 音韻地位 categories and the links from fanqie rows to them.

## `public static void LSchemaDiweiCreate(SqliteConnection connection)`

A category is keyed by language, kind and key, so 來 as an initial is one row for every character.
A link belongs to a fanqie row and points to a category, one link per part the row carries.
A refetch deletes the character's fanqie rows, and the links go with them by cascade.
A category deleted takes its links too, so a rebuild can start from an empty language.
