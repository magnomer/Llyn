# TRegister.cs

## `public sealed class TRegister`

Covers the engine's Register seam, which mirrors the Situation one on a card.
It covers the two kinds of row that share the shelf.
Those are the ones a language pack ships and the ones the user writes.

## `public void RegisterSave_CardCarryingPresetNames_StoresBuiltinRows()`

A card marked with names a language pack ships must reference that pack's rows rather than write new ones.
The ids are fixed by the pack, so the same name in two workspaces means the same Register.
The card's order is kept, because a mark is ordered on the reference and not on the row.

## `public void RegisterSave_TwoPacksNamingOneName_ShareOneRow()`

Two language packs shipping the same name must share one built-in row.
Both entries are then found through that one row.

## `public void RegisterSeed_NameTheUserWroteFirst_BecomesBuiltin()`

A name the user wrote before any pack shipped it keeps its row when a pack later ships it.
The row turns built-in in place, so every mark made on it survives.

## `public void RegisterSave_CardCarryingWrittenName_StoresWrittenRow()`

A name no pack ships is stored as a written Register, belonging to no language.

## `public void RegisterSave_TwoEntriesSharingName_ReferenceOneRow()`

Two cards typing the same wording must reference one row.
Otherwise the shelf would grow a duplicate every time a name was typed instead of chosen.

## `public void RegisterSave_TwoEntriesWritingOneName_ReferenceOneRow()`

Two cards typing the same wording without ever looking it up must still reference one row.
The engine folds case and edge spaces when it looks the wording up.
The panel therefore needs no matching rule of its own.
A client that never consults the shelf, such as an import, gets the same shelf as the form.

## `public void RegisterFind_WorkspaceShelf_ReturnsMarkCounts()`

The panel browsing the shelf must see a Register nothing is marked with, which no card read can reach.
It must also see the count on every row without asking once per row.
Under the usage ordering the most-marked row leads, which is the whole reason that ordering exists.

## `public void RegisterFind_ByLanguageName_AnswersNothing()`

A query naming a language finds no row, because registers are found by their own wording.
A language name is no register, so the shelf answers nothing for it.

## `public void EntryFind_ByRegister_ReturnsTheEntriesMarkedWithIt()`

The middle column of the panel: the Entries whose cards carry the chosen Register.
A Register carrying no id stands for the whole workspace, which is what the panel shows before one is chosen.

## `public void RegisterCreate_WordingNoCardCarries_ListsItAsWritten()`

The tenor panel's New makes a written Register no card marks yet, trimmed, on the language it was given.
It is listed with no marks counted, so the panel can select it at once.

## `public void RegisterCreate_WordingAlreadyOnShelf_ReturnsTheStoredRow()`

A wording the shelf already holds, whatever its case, answers the stored row rather than a second one.
