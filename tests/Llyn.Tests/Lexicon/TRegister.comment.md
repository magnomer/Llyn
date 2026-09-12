# TRegister.cs

## `public sealed class TRegister`

Covers the engine's Register seam, which mirrors the Situation one on a card.
It covers the two kinds of row that share the shelf: the ones a language pack ships and the ones the user writes.

## `public void RegisterSave_CardCarryingPresetNames_StoresLanguagePackRows()`

A card marked with names a language pack ships must reference that pack's rows rather than write new ones.
The ids are fixed by the pack, so the same name in two workspaces means the same Register.
The card's order is kept, because a mark is ordered on the reference and not on the row.

## `public void RegisterSave_CardCarryingWrittenName_StoresWrittenRow()`

A name no pack ships is stored as a written Register, belonging to no language.

## `public void RegisterSave_TwoEntriesSharingName_ReferenceOneRow()`

Two cards typing the same wording must reference one row.
Otherwise the shelf would grow a duplicate every time a name was typed instead of chosen.

## `public void RegisterSave_TwoEntriesWritingOneName_ReferenceOneRow()`

Two cards typing the same wording without ever looking it up must still reference one row.
The engine folds case and edge spaces when it looks the wording up, so the panel needs no matching rule of its own.
A client that never consults the shelf, such as an import, gets the same shelf as the form.

## `public void RegisterFind_WorkspaceShelf_ReturnsMarkCounts()`

The panel browsing the shelf must see a Register nothing is marked with, which no card read can reach.
It must also see the count on every row without asking once per row.
Under the usage ordering the most-marked row leads, which is the whole reason that ordering exists.

## `public void RegisterFind_ByLanguage_AnswersThePackName()`

A shelf holding several packs is narrowed by naming a language, not only by naming a register.
A row the user wrote belongs to no language, so it answers no such query.

## `public void EntryFind_ByRegister_ReturnsTheEntriesMarkedWithIt()`

The middle column of the panel: the Entries whose cards carry the chosen Register.
A Register carrying no id stands for the whole workspace, which is what the panel shows before one is chosen.

## `public void RegisterChange_WrittenRow_RenamesItOnEveryCard()`

A written Register is renamed once and every card marked with it follows, because a mark is a reference.

## `public void RegisterDelete_LanguagePackRow_KeepsIt()`

A row a language pack ships survives a delete, because the pack owns it and seeding would bring it back.

## `public void RegisterDelete_LanguagePackRowOnCards_KeepsEveryMark()`

A delete aimed at a shipped row must stop before it drops anything.
Refusing only the final statement would leave the row standing with its mark stripped off every card.
