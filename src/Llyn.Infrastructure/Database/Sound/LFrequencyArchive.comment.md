# LFrequencyArchive.cs
Hash: `6a2ae58bf5ab16a1`

## `public sealed class LFrequencyArchive : LFrequencyVault`

Reads and writes the frequency rows one entry carries, one row per pack source that answered.

## `public IReadOnlyList<LFrequency> LFrequencyRead(long entryId)`

Reads the entry's rows by source name, only so the result is stable.
A refetch rewrites the rows, so write order carries no meaning.
The engine reorders them by the pack's declared sources.
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
