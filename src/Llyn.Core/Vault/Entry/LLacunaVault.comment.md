# LLacunaVault.cs
Hash: `b482473c7970af96`

## `public interface LLacunaVault`

The persistence port for the Lacuna rows the engine reads and writes.
It lists exactly what the engine asks of lacuna storage, and nothing about how rows are kept.
`LLacunaArchive` in Infrastructure is its adapter over the workspace database.

## `IReadOnlyList<LLacuna> LLacunaRead(long entryId);`

Reads the entry's rows in the order they were written.
An older workspace may hold a row without a morphology value.
A reader ignores it.

## `void LLacunaSave(long entryId, IReadOnlyList<long> morphologyIds);`

Replaces the entry's rows with one per given morphology id in one transaction.
An empty list leaves the entry with no rows at all.
Every row is stamped with the same fetch time.

## `void LLacunaDelete(long entryId);`

Drops the entry's rows, so a hand-edited entry is asked afresh.
