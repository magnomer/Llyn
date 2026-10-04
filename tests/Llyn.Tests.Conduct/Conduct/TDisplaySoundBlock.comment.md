# TDisplaySoundBlock.cs
Hash: `d8922d60db07cfa9`

## `public sealed class TDisplaySoundBlock`

Covers the sound area's fanqie, script and paradigm blocks and its cell clicks on a real workspace.
It builds its wing and entries through `TDisplaySound.TDisplayWingPrepare` and `TDisplaySound.TDisplayKoreanSave`.

## `public void DisplayBlocksRead_NothingShown_AnswersEmptyBlocks()`

With nothing open, the fanqie, script and paradigm blocks are empty, not pending and carry no font.

## `public void DisplayBlocksRead_EnglishEntry_AnswersNoRimeScriptOrParadigmRows()`

An English entry has no rime-book, script or paradigm rows.

## `public void DisplayParadigmRead_MorphologyOff_AnswersTheAbsentTipWithoutALookup()`

With morphology off, saving the entry starts no inflection lookup.
The unanswered plural slot then shows the absent tip.
The source is gated, so a started lookup would show the pending tip instead.

## `public async Task DisplayParadigmRead_LookupGatedThenLost_AnswersThePendingTipThenTheLostTip()`

With morphology on, saving the entry starts a lookup that the gated source holds open.
While it is held, the unanswered plural slot shows the pending tip.
The source then answers unavailable and leaves the slot with no form.
The display, which never holds the draft, then shows the lost tip.

## `public void DisplayDiweiAndStemOpen_ShownEntry_RaiseTheShownLanguage()`

Each click is raised in the shown entry's language, and nothing open raises nothing.
The initial flag picks the cell kind.

## `public void DisplayDiweiOpen_EmptyKey_OpensNothing()`

A blank key of either kind raises nothing and answers false.

## `public void DisplayFanqieSet_ShownEntry_SetsTheRankForTheShownEntry()`

Setting a rank before any entry is shown reaches the gate with nothing.
After entry 7 is shown, setting rank 2 with the raise flag hands the gate `7,3,2,True`.

## Inline notes

### `private static LEntry TDisplayCatSave(LEngine engine)`

Saves a countable English noun whose paradigm asks for a plural nothing stores.
