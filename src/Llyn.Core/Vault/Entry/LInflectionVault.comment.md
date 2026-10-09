# LInflectionVault.cs
Hash: `f71ebfe50c0bd8ec`
Hash: `57f900b2f15ba64e`

## `public interface LInflectionVault`

The persistence port for the Inflection rows the engine reads and writes.
It lists exactly what the engine asks of inflection storage, and nothing about how rows are kept.
`LInflectionArchive` in Infrastructure is its adapter over the workspace database.

## `IReadOnlyList<LInflection> LInflectionAppend(long entryId, IReadOnlyList<LInflection> inflections);`

Adds `inflections` after the inflections the entry identified by `entryId` already has, each with its features in list order.
The whole write is one transaction.
The new positions continue the entry's existing numbering.
So adding to an entry that already has inflections extends the set rather than colliding.
The rows come back with their ids and positions.

## `IReadOnlyList<LInflection> LInflectionRead(long entryId);`

Reads the entry's inflections, ordered by position, each carrying its ordered features.

## `IReadOnlyList<LInflection> LInflectionSet(long entryId, IReadOnlyList<LInflection> inflections);`

Replaces the entry's inflections with `inflections` in list order.
Existing inflection rows are cleared, their features cascade, and the new set is written.
So reordering rewrites positions while the entry id stays fixed.
The rows come back with their ids and positions.

## `void LInflectionRegularSave(long inflectionId, bool regular);`

Writes the derived regular flag alone onto one inflection row.
The engine derives it whenever the form or its entry is stored, so a reader never runs the paradigm pattern.
A row rewritten by a move or a delete carries the flag across.
The insert writes what the record holds.

## `void LInflectionAnalysisSave(long inflectionId, string? prediction, IReadOnlyList<LInflectionMark>? marks, string? stamp, bool regular);`

Writes one row's rule-book analysis and its regular flag together.
The four values come from one run of the book, so they are never stored apart.
An uncovered cell has null prediction and marks but retains the book's stamp.
The stamp distinguishes current uncovered analysis from analysis that needs refreshing.
The insert carries the analysis across a move or a delete, as it does the flag.
