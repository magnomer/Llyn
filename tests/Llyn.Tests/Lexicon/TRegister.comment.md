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

## `public void RegisterDelete_LanguagePackRow_KeepsIt()`

A row a language pack ships survives a delete, because the pack owns it and seeding would bring it back.
