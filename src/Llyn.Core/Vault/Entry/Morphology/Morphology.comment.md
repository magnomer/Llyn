# Morphology

This folder holds the persistence ports for inflected forms and the morphology vocabulary they link.

## `LInflectionVault`

The persistence port for the Inflection rows of an entry.

## `LInflectionAppend`

Adds inflections after those the entry already has, each with its features in list order.
The whole write is one transaction.
The new positions continue the entry's existing numbering.
So adding to an entry that already has inflections extends the set rather than colliding.
The rows come back with their ids and positions.

## `LInflectionRead`

Reads the entry's inflections, ordered by position, each carrying its ordered features.

## `LInflectionSet`

Replaces the entry's inflections in list order.
Existing inflection rows are cleared, their features cascade, and the new set is written.
So reordering rewrites positions while the entry id stays fixed.
The rows come back with their ids and positions.

## `LInflectionRegularSave`

Writes the derived regular flag alone onto one inflection row.
The engine derives it whenever the form or its entry is stored, so a reader never runs the paradigm pattern.
A row rewritten by a move or a delete carries the flag across.
The insert writes what the record holds.

## `LInflectionAnalysisSave`

Writes one row's rule-book analysis and its regular flag together.
The four values come from one run of the book, so they are never stored apart.
An uncovered cell has null prediction and marks but retains the book's stamp.
The stamp distinguishes current uncovered analysis from analysis that needs refreshing.
The insert carries the analysis across a move or a delete, as it does the flag.

## `LMorphologyVault`

The persistence port for the Morphology rows, the features and values a part of speech takes.

## `LFeatureCreate`

Adds or replaces one feature, keyed by the part of speech and the pack code.
It returns the feature with its row id.
A pack that renames a feature keeps the row id.

## `LMorphologyCreate`

Adds or replaces one value, keyed by the feature and the pack code, and returns it with its row id.
A pack that renames a value keeps the row id, so every inflection that links it follows the rename.

## `LFeatureRead`

Reads every feature the part of speech takes, in display order.

## `LMorphologyRead`

Reads one value by row id, or null when no row has it.

## `LMorphologyFind`

Resolves the value a name names under the feature, or null when none does.
Matching is a trimmed, case-insensitive comparison of the display name.

## `LMorphologyCodeFind`

Resolves the value a language pack names by code under its declared feature and part.
It returns null when the language holds none.
A value code is only unique within its feature, so the feature and part codes bind the row exactly.
The join runs from the value through its feature to the part of speech.
The pack code is matched at each step.

## Stamp

Commit: `c2f335c331fe`
- `LInflectionVault.cs` `f71ebfe50c0bd8ec`
- `LMorphologyVault.cs` `461d0762aab7fa0b`
