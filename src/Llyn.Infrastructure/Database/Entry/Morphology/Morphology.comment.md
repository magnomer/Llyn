# Morphology

This folder holds the stores of an entry's inflected forms and the morphology vocabulary they link.

## `LInflectionArchive`

It persists an entry's inflected forms and the ordered grammatical features each carries.
Inflections are ordered child rows under an entry, each with its own row id.
A feature links it by that id.
Deleting an inflection removes its features through the cascade, and deleting the entry removes both.
Only row links are stored here, and names are read from the morphology vocabulary (`LMorphologyArchive`).

Removing or moving an inflection rewrites the entry's whole set.
That keeps positions at `0 … n-1` and carries every feature along.
A rewrite mints new inflection ids, so callers take the returned rows rather than keeping old ones.

`LInflectionAppend` adds after the existing ones, continuing the entry's numbering, so adding extends the set rather than colliding.
The whole write is one transaction, and rows come back with ids and positions.

`LInflectionRead` gives inflections by position, each with ordered features and its stored rule-book analysis.
The marks are parsed by `LInflectionMarkParse`.

`LInflectionSet` clears the existing rows, lets their features cascade, and writes the new set in list order.

`LInflectionRegularSave` writes the derived regular flag alone onto one row.
The engine derives it whenever the form or its entry is stored, so a reader never runs the paradigm pattern.
A row rewritten by a move or a delete carries the flag across.

`LInflectionAnalysisSave` writes the analysis and the regular flag in one statement.
The marks are stored as text through `LInflectionMarkFormat`, and an empty list stores an empty string.
Null analysis values stay SQL NULL, which differs from empty marks on a covered form.
Uncovered forms can retain the stamp of the book that examined them.
The insert writes the analysis the record holds, so a rewritten row carries it across.

`LInflectionSetRead` reads on a connection the caller holds, with one query for inflections and one for every feature.
There is no round trip per inflection.
`LInflectionInsert` writes each form with its regular flag and analysis from a first position on.
So a move or a delete keeps the analysis without running the book again.
`LInflectionCountRead` lets an append continue the numbering.
`LInflectionMorphologyRead` reads every morphology link in one query, grouped by inflection id.

## `LMorphologyArchive`

It persists and resolves a language's morphology vocabulary.
A feature belongs to a part of speech and owns its values.
Inflections link a value row by id, and the value knows its feature.
Names live here and are never copied onto an entry's lexical rows.

`LFeatureCreate` adds or replaces a feature keyed by `(speech_value_parent, pack_code)`.
`LMorphologyCreate` does the same for a value keyed by `(morphology_feature_parent, pack_code)`.
A pack that renames either keeps the row id, so every inflection linking a value follows the rename.

`LFeatureRead` lists the features a part of speech takes in display order.
`LMorphologyRead` answers `null` when no row has the id.
`LMorphologyFind` matches a trimmed, case-insensitive display name under a feature.
`LMorphologyCodeFind` resolves a value a language pack names by code, or `null` when the language holds none.
A value code is only unique within its feature, so the feature and part codes bind the row exactly.
The join runs from the value through its feature to the part of speech.
It matches the pack code at each step.

## Stamp

Commit: `c2f335c331fe`
- `LInflectionArchive.cs` `4a9f05b271636c8b`
- `LMorphologyArchive.cs` `4439bbd38a6fd68f`
