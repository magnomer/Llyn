# LInflectionArchive.cs
Hash: `4a9f05b271636c8b`
Hash: `2751885466558c60`

## `public sealed class LInflectionArchive : LInflectionVault`

Persists an entry's inflected forms and the ordered grammatical features each carries.
Inflections are written as ordered child rows under an entry.
Reordering preserves the entry id but replaces the inflection rows and their ids.
Each inflection has its own row id, and a feature links it by that id.
Deleting an inflection removes its features through the foreign-key cascade, and deleting the entry removes both.
Only row links are stored here.
Feature and value names are read from the morphology vocabulary (`LMorphologyArchive`).

Removing or moving an inflection rewrites the entry's whole set.
That keeps positions at `0 … n-1` and carries every feature along with its inflection.
A rewrite mints new inflection ids, so callers take the returned rows rather than keeping old ones.

## `public LInflectionArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyList<LInflection> LInflectionAppend(long entryId, IReadOnlyList<LInflection> inflections)`

Adds `inflections` after the inflections the entry identified by `entryId` already has, each with its features in list order.
The whole write is one transaction.
The new positions continue the entry's existing numbering.
So adding to an entry that already has inflections extends the set rather than colliding.
The rows come back with their ids and positions.

## `public IReadOnlyList<LInflection> LInflectionRead(long entryId)`

Reads the entry's inflections, ordered by position, each carrying its ordered features.
Each also carries its stored rule-book analysis, with the marks parsed by `LInflectionMarkParse`.

## `public void LInflectionRegularSave(long inflectionId, bool regular)`

Writes the derived regular flag alone onto one inflection row.
The engine derives it whenever the form or its entry is stored, so a reader never runs the paradigm pattern.
A row rewritten by a move or a delete carries the flag across.
The insert writes what the record holds.

## `public void LInflectionAnalysisSave(long inflectionId, string? prediction, IReadOnlyList<LInflectionMark>? marks, string? stamp, bool regular)`

Writes the rule-book analysis and the regular flag onto one inflection row in one statement.
The marks are stored as text through `LInflectionMarkFormat`, and an empty list stores an empty string.
Null analysis values preserve SQL NULL, distinct from empty marks on a covered form.
Uncovered forms can retain the stamp of the book that examined them.
The insert writes the analysis the record holds, so a rewritten row carries it across.

## `public IReadOnlyList<LInflection> LInflectionSet(long entryId, IReadOnlyList<LInflection> inflections)`

Replaces the entry's inflections with `inflections` in list order.
Existing inflection rows are cleared, their features cascade, and the new set is written.
So reordering rewrites positions while the entry id stays fixed.
The rows come back with their ids and positions.

## `private static IReadOnlyList<LInflection> LInflectionSetRead(SqliteConnection connection, long entryId)`

The entry's inflections in order on a connection the caller already holds.
One query reads the inflections and one reads every feature of all of them.
There is no round-trip per inflection.

## `private static IReadOnlyList<LInflection> LInflectionInsert(SqliteConnection connection, long entryId, IReadOnlyList<LInflection> inflections, int first)`

Writes each form with its regular flag and its rule-book analysis from `first` on.
So a move or a delete keeps the analysis without running the book again.

## `private static int LInflectionCountRead(SqliteConnection connection, long entryId)`

How many inflections the entry already has, so an append continues its numbering.

## `private static IReadOnlyDictionary<long, IReadOnlyList<long>> LInflectionMorphologyRead(SqliteConnection connection, long entryId)`

Every morphology link of every inflection the entry has, in one query, grouped by inflection id.
