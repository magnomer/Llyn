# LFrequencyVault.cs

## `public interface LFrequencyVault`

The persistence port for the Frequency rows the engine reads and writes.
It lists exactly what the engine asks of frequency storage, and nothing about how rows are kept.
`LFrequencyArchive` in Infrastructure is its adapter over the workspace database.

## `IReadOnlyList<LFrequency> LFrequencyRead(long entryId);`

Reads the entry's rows in the order they were written, which is the pack's source order.
The word interval is not stored, so the rows come back without it for the engine to derive.

## `void LFrequencySet(long entryId, IReadOnlyList<LFrequency> rows);`

Replaces the entry's rows with the given set in one transaction.
Every row is stamped with the same fetch time.

## `void LFrequencyClear(long entryId);`

Drops the entry's rows, so a renamed entry is fetched afresh.

## `void LFrequencyBandSet(long entryId, string source, string? band);`

Stores the band of one row after the engine resolved it, which a migrated row needs once.
