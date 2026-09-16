# LSchemaFrequency.cs

## `public static class LSchemaFrequency`

Creates the frequency store, one row per source that answered for one entry.

## `public static void LSchemaFrequencyCreate(SqliteConnection connection)`

The row is keyed by entry and source, so a refetch replaces an entry's rows cleanly.
The raw figure is the ground truth exactly as the source returned it.
The band is the label the pack mapped the figure to at fetch time, so no view resolves it again.
The rows go with their entry when it is deleted.

## `public static void LSchemaFrequencySettle(SqliteConnection connection, string into, string from)`

Carries the old `entry.frequency` column of an older workspace into the frequency table during migration.
The old column held `source|raw`, and the settle splits it on the first bar.
The band is left empty, and the engine resolves and stores it the first time the entry is read.
A workspace whose entry table no longer has the column is left alone.
