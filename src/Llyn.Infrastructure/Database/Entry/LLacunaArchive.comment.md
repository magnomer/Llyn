# LLacunaArchive.cs
Hash: `a55a2ef1f0c5bbbf`
Hash: `cf9f9fe9156d40f5`

## `public sealed class LLacunaArchive : LLacunaVault`

Reads and writes the lacuna rows one entry carries, one row per paradigm slot the web could not fill.

## `public IReadOnlyList<LLacuna> LLacunaRead(long entryId)`

Reads the entry's rows in the order they were written.
An older workspace may hold a row without a morphology value.
The returned lacuna preserves its nullable id and cell key for the reader to resolve.
A row stored before cells existed carries an empty cell.

## `public void LLacunaSave(long entryId, IReadOnlyList<LLacuna> lacunae)`

Replaces the entry's rows with one per given lacuna in one transaction.
Each row keeps its morphology id and its cell key.
An empty list leaves the entry with no rows at all.
Every row is stamped with the same fetch time.

## `public void LLacunaDelete(long entryId)`

Drops the entry's rows, so a hand-edited entry is asked afresh.

## `private static void LLacunaClear(SqliteConnection connection, long entryId)`

Empties the entry's rows inside the caller's session.
