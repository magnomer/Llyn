# LFrequencyArchive.cs

## `public sealed class LFrequencyArchive`

Reads and writes the frequency rows one entry carries, one row per pack source that answered.

## `public IReadOnlyList<LFrequency> LFrequencyRead(long entryId)`

Reads the entry's rows in the order they were written, which is the pack's source order.
The word interval is not stored, so the rows come back without it for the engine to derive.

## `public void LFrequencySet(long entryId, IReadOnlyList<LFrequency> rows)`

Replaces the entry's rows with the given set in one transaction.
Every row is stamped with the same fetch time.

## `public void LFrequencyClear(long entryId)`

Drops the entry's rows, so a renamed entry is fetched afresh.

## `public void LFrequencyBandSet(long entryId, string source, string? band)`

Stores the band of one row after the engine resolved it, which a migrated row needs once.

## `private static void LFrequencyDelete(SqliteConnection connection, long entryId)`

Deletes the entry's rows inside the caller's session.
