# LInflectionArchive.cs

## `public sealed class LInflectionArchive`

Persists an entry's inflected forms and the ordered grammatical features each carries.
Inflections are written as ordered child rows under an entry.
So reordering rewrites `position` only and never touches the entry id.
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

## `public void LInflectionRegularSave(long inflectionId, bool regular)`

Writes the derived regular flag alone onto one inflection row.
The engine derives it whenever the form or its entry is stored, so a reader never runs the paradigm pattern.
A row rewritten by a move or a delete carries the flag across.
The insert writes what the record holds.

## `public IReadOnlyList<LInflection> LInflectionSet(long entryId, IReadOnlyList<LInflection> inflections)`

Replaces the entry's inflections with `inflections` in list order.
Existing inflection rows are cleared, their features cascade, and the new set is written.
So reordering rewrites positions while the entry id stays fixed.
The rows come back with their ids and positions.

## Inline notes

### `private static IReadOnlyList<LInflection> LInflectionSetRead(SqliteConnection connection, long entryId)`

The entry's inflections in order on a connection the caller already holds.
One query reads the inflections and one reads every feature of all of them.
There is no round-trip per inflection.

### `private static int LInflectionCountRead(SqliteConnection connection, long entryId)`

How many inflections the entry already has, so an append continues its numbering.

### `private static IReadOnlyDictionary<long, IReadOnlyList<long>> LInflectionMorphologyRead(`

Every morphology link of every inflection the entry has, in one query, grouped by inflection id.
