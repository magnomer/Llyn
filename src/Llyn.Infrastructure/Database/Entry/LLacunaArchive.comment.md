# LLacunaArchive.cs
Hash: `cf9f9fe9156d40f5`

## `public sealed class LLacunaArchive : LLacunaVault`

Reads and writes the lacuna rows one entry carries, one row per paradigm slot the web could not fill.

## `public IReadOnlyList<LLacuna> LLacunaRead(long entryId)`

Reads the entry's rows in the order they were written.
An older workspace may hold a row without a morphology value.
The read returns it as `null` and leaves the ignoring to the reader.

## `public void LLacunaSave(long entryId, IReadOnlyList<long> morphologyIds)`

Replaces the entry's rows with one per given morphology id in one transaction.
An empty list leaves the entry with no rows at all.
Every row is stamped with the same fetch time.

## `public void LLacunaDelete(long entryId)`

Drops the entry's rows, so a hand-edited entry is asked afresh.

## `private static void LLacunaClear(SqliteConnection connection, long entryId)`

Empties the entry's rows inside the caller's session.
