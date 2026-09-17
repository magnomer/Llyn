# LLacunaArchive.cs

## `public sealed class LLacunaArchive`

Reads and writes the lacuna rows one entry carries, one row per paradigm slot the web could not fill.

## `public IReadOnlyList<LLacuna> LLacunaRead(long entryId)`

Reads the entry's rows in the order they were written.
A row without a morphology value says no source was reached for the entry.

## `public void LLacunaSave(long entryId, IReadOnlyList<long?> morphologyIds)`

Replaces the entry's rows with one per given morphology id in one transaction.
A `null` id writes the row that marks the entry lost, and only the first of several is written.
The key treats every null as distinct, so the replace clause alone would let lost rows pile up.
An empty list leaves the entry with no rows at all.
Every row is stamped with the same fetch time.

## `public void LLacunaDelete(long entryId)`

Drops the entry's rows, so a hand-edited entry is asked afresh.

## `private static void LLacunaClear(SqliteConnection connection, long entryId)`

Empties the entry's rows inside the caller's session.
