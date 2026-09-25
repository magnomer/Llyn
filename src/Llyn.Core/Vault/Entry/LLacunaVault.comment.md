# LLacunaVault.cs

## `public interface LLacunaVault`

The persistence port for the Lacuna rows the engine reads and writes.
It lists exactly what the engine asks of lacuna storage, and nothing about how rows are kept.
`LLacunaArchive` in Infrastructure is its adapter over the workspace database.

## `IReadOnlyList<LLacuna> LLacunaRead(long entryId);`

Reads the entry's rows in the order they were written.
A row without a morphology value says no source was reached for the entry.

## `void LLacunaSave(long entryId, IReadOnlyList<long?> morphologyIds);`

Replaces the entry's rows with one per given morphology id in one transaction.
A `null` id writes the row that marks the entry lost, and only the first of several is written.
The key treats every null as distinct, so the replace clause alone would let lost rows pile up.
An empty list leaves the entry with no rows at all.
Every row is stamped with the same fetch time.

## `void LLacunaDelete(long entryId);`

Drops the entry's rows, so a hand-edited entry is asked afresh.
