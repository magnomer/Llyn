# LSchemaLacuna.cs
Hash: `1ff0cd476f3a833c`
Hash: `91d309bcaa047fae`

## `public static class LSchemaLacuna`

Creates the lacuna store, one row per paradigm slot the web could not fill for one entry.

## `public static void LSchemaLacunaCreate(SqliteConnection connection)`

The row is keyed by entry, morphology value and cell, so a refetch replaces an entry's rows cleanly.
The cell is the key of a missed multi-value cell, and empty for a one-value slot.
Cells sharing a first value would collide without it.
A row carried from an older version takes the empty cell, which is the one-value row it was.
Nullable morphology links preserve older rows and permit identification through a cell key alone.
The fetch time is kept so a later build may age the answer out.
The rows go with their entry when it is deleted, and with their morphology value when a pack drops it.
Existing rows can take the empty cell default when the shared-column migration adds cell keys.
