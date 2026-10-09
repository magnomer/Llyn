# LLacunaVault.cs
Hash: `bc31ece851bb9230`
Hash: `b482473c7970af96`

## `public interface LLacunaVault`

The persistence port for the Lacuna rows the engine reads and writes.
It lists exactly what the engine asks of lacuna storage, and nothing about how rows are kept.
`LLacunaArchive` in Infrastructure is its adapter over the workspace database.

## `IReadOnlyList<LLacuna> LLacunaRead(long entryId);`

Reads the entry's rows in the order they were written.
A cell key identifies a missed slot even without a morphology id.
A row with neither identifier names no slot.

## `void LLacunaSave(long entryId, IReadOnlyList<LLacuna> lacunae);`

Replaces the entry's rows with one per given lacuna in one transaction.
Each row keeps its morphology id and its cell key.
An empty list leaves the entry with no rows at all.
Every row is stamped with the same fetch time.

## `void LLacunaDelete(long entryId);`

Drops the entry's rows, so a hand-edited entry is asked afresh.
