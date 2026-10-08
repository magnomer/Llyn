# TReflexRow.cs
Hash: `f65e1ea1223e53ac`

## `public sealed class TReflexRow`

Covers the reflex row rules, namely the label keys, the lead, the hidden row and the typed copy.

## `public void ReflexLanguageKey_NamedOrBlank_PrefixesTheReflexKey()`

A language or kind is labelled under `Reflex.` plus its name.
A blank name keys `Reflex.` alone.

## `public void ReflexLeadRead_ThreeRuns_LeadsEachRunOnce()`

The first row of each run of one language leads, and a later run of the same language leads again.
Languages compare as stored, so a trailing space opens a run of its own.

## `public void ReflexHiddenCheck_FoldAndOpening_HidesAFoldedRowWhileClosed()`

Only a folded row under a closed fold is hidden.

## `public void ReflexTypedApply_TypedCell_ChangesThatCellAlone()`

A typed cell changes only that cell, and the label key follows a typed language.
The held row stays as it was, and the lead is kept.

## `private static CReflex TReflexRowCreate(string language, string kind, bool folded)`

Builds one ready reflex row in a language and kind, folded or not.
