# Speech

This folder holds the store and the pack loader of a language's part-of-speech vocabulary.

## `LSpeechArchive`

Persists and resolves a language's part-of-speech vocabulary.
An entry's POS rows link a speech_value row by id.
The display name lives here and is read by that id.
So a name is never copied onto an entry's rows.

## `LSpeechValueCreate`

Adds or replaces one vocabulary row, keyed by (language, pack_code), and returns it with its row id.
A pack that renames a value keeps the row id, so every entry that links it follows the rename.
A value with code 0 is user-added and is given a negative code no pack can collide with.
`LSpeechCodeCreate` counts down from -1 for the next free code.

## `LSpeechValueRead`

Reads one value by row id, or null when no row has it.
The language overload reads every part of speech the language declares, in stored order, which carries no meaning.
`LVocabularyClerk.LSpeechRead` orders them by the pack's declared rule before any view sees them.
Those are the presets the part-of-speech field offers.
A language with no stored rows reads as an empty list rather than a failure.

## `LSpeechValueFind`

Resolves the part of speech a name names in a language back to its row, or null when none matches.
Matching is a trimmed, case-insensitive comparison of the display name.
The name arrives as the user typed it, so picking "Verb" and typing "verb" mean the same thing.
Only text that names no value at all is stored as typed.

## `LSpeechLoad`

The parts of speech and features the language pack declares, read through `LSpeechLoader`.

## `LSpeechRetirementApply`

Moves every link off one retired part and deletes its row, in one session.
A target the workspace does not hold drops the links as a row without a target does.
The unit goes on first, while the links still say which entries held the part.
An entry that already links the target loses its retired link instead of holding the target twice.
A dropped link leaves an inflection without a part rather than failing on the foreign key.
The touched entries are renumbered from zero, since a removed link leaves a gap.
`LSpeechPositionReset` passes through negative positions first, so no step collides with the primary key.

## `LSpeechLoader`

Loads a language's display vocabulary from languages/`<Name>`/vocabulary.json under the application's base directory.
That is the same way `LLanguageLoader` resolves a pronunciation pack.
The parts of speech and their morphology are language-specific facts.
So they are data in the pack, never names compiled into the store.
Adding a language needs a folder, not a recompile.

A missing or malformed file yields an empty vocabulary rather than throwing.
A language may legitimately declare no morphology at all, as an isolating language does.
So an empty result is an answer, not a failure.
One unknown pack never stops the others from loading.

## `LSpeechLoaderLoad`

Reads the vocabulary a language declares, or an empty one when the pack declares none.
A name `LLanguageNameValidate` refuses reads nothing.

## `LSpeechPackRead`

Builds the pack as records with row id 0 and parent links holding codes.
The file states order by listing rather than numbering.
Each part takes its display order from its place in the list.
Each feature and value take theirs from their place among their siblings.
A part may name the part it specialises, and a paradigm names its part, both by code.
A paradigm row is skipped when its part or any of its values is not a positive integer.
A row may state values, cells or both, and a row with neither is skipped.
Each value becomes a cell of one value, listed before the stated cells.
A present but malformed values or cells skips the row.
A paradigm may except parts by code and may carry regular-form rules as pattern and replacement pairs.

## `LSpeechNumberRead`

Reads one declared positive integer, or null when absent, not a number, or not positive.
A code must be positive because user-added values take the negative range.

## `LSpeechNumbersRead`

Reads one declared list of positive integers, or null when absent, not an array, or holding anything else.
A paradigm naming one value nothing can resolve is no paradigm at all.
The inflection loader reads its value lists with it too.

## `LSpeechCellsRead`

Reads the declared cells, each a non-empty list of positive integers, or null when anything else is found.
The values keep the order the pack lists them, since that order is the slot's display name.
The inflection loader reads its columns and line cells with it too.

## `LSpeechRetirementScan`

Reads the retired rows, each with its code, its into target or 0, and its unit key.
A row without a positive id is skipped.

## `LSpeechRuleScan`

Reads the declared rules of a paradigm, each a two-string array, or none when the property is absent.
A row that is not such a pair or has an empty pattern is skipped.
So is one that does not parse as a regular expression.

## `LSpeechTextRead`

Reads one declared string, or null when absent, not a string, or blank.
A row with a blank name would be a vocabulary entry nothing can show.

## `LSpeechRowRead`

Reads the rows of one list, or none when the property is absent or not an array.

## Inline notes

### LSpeechArchive.cs: command.CommandText =

The name is compared in the store, so the whole vocabulary is not read to answer one lookup.
SQLite's NOCASE collation folds ASCII only, and the display names are the pack's.
So a name the pack declares outside that range still matches when it is typed as declared.

## Stamp

Commit: `c2f335c331fe`
- `LSpeechArchive.cs` `c9c0bb44eab4767c`
- `LSpeechLoader.cs` `46d335516437555c`
