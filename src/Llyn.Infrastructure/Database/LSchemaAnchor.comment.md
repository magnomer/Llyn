# LSchemaAnchor.cs

## `public static class LSchemaAnchor`

Creates the anchor store: the ties from reflex rows to the fanqie rows they answer.

## `public static void LSchemaAnchorCreate(SqliteConnection connection)`

An anchor belongs to a reflex row and points to a fanqie row, one row per pair.
A reflex row deleted takes its anchors by cascade.
A fanqie row deleted takes them too, so a placement the refetch no longer finds unties itself.
The fanqie save upserts on the natural key, so a refetch keeps the ids and the anchors stand.
