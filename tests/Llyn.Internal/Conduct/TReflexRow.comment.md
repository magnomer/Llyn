# TReflexRow.cs

## `public sealed class TReflexRow`

Covers the reflex row rules both panes share: the label keys, the lead and the hidden row.
It also covers the editor's lead read while a language is typed.

## `public void ReflexLanguageKey_NamedOrBlank_PrefixesTheReflexKey()`

A language or kind is labelled under `Reflex.` plus its name.
A blank name keys `Reflex.` alone, which no catalog holds.

## `public void ReflexLeadRead_ThreeRuns_LeadsEachRunOnce()`

The first row of each run of one language leads, and a later run of the same language leads again.
Languages compare as stored, so a trailing space opens a run of its own.

## `public void EditorLeadRead_TypedLanguage_LeadsByTheOverlaidRows()`

No held draft leads no row.
A language typed into the middle row stands in for its stored one, and the leads follow it.

## `public void ReflexHiddenCheck_FoldAndOpening_HidesAFoldedRowWhileClosed()`

Only a folded row under a closed fold is hidden.

## `private static CReflex TReflexRowCreate(string language, string kind, bool folded)`

Builds one ready reflex row in a language and kind, folded or not.
