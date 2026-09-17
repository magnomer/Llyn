# LSchemaLacuna.cs

## `public static class LSchemaLacuna`

Creates the lacuna store, one row per paradigm slot the web could not fill for one entry.

## `public static void LSchemaLacunaCreate(SqliteConnection connection)`

The row is keyed by entry and morphology value, so a refetch replaces an entry's rows cleanly.
A row with no morphology value says no source was reached for the entry at all.
The fetch time is kept so a later build may age the answer out.
The rows go with their entry when it is deleted, and with their morphology value when a pack drops it.
No settle step exists, since no earlier build kept these answers anywhere.
