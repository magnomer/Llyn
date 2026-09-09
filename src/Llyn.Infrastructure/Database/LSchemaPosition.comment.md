# LSchemaPosition.cs

## `public static class LSchemaPosition`

Version 12.
Duplicate positions in the ordered sets that had no unique index yet.
The index cannot be created over a set that holds one, so the set is renumbered first.

## `public static void LSchemaPositionNormalize(SqliteConnection connection, string table, string ownerColumn)`

Renumbers an ordered set to 0…n-1 per owner so the unique position index can be created over it.
The new positions are computed into a temporary table first.
An UPDATE that read the very column it writes would depend on the order rows were visited.
