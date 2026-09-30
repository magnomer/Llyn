# TDisplaySound.cs

## `public sealed class TDisplaySound`

Covers the reading view's sound area, its reads and gates alike, on a real workspace.
The wing opens the entry, so each case stands on its display's sound area.
A fake port stands in where a case must see what the gate hands the engine.

## `public void DisplayGlyphRead_HanjaRow_AnswersTheRowReadyToShow()`

A Korean entry with a Hanja row answers the scheme's key and name.
It also answers its linked cells and the pack's glyph font.

## `public void DisplayGlyphRead_LanguageWithoutGlyphOrNothingShown_AnswersTheHiddenRow()`

With nothing open, or for a language without a glyph section, the row is hidden and empty.

## `public void DisplayGlyphOpen_LinkedCell_RaisesTheCharacterEntryForTheLibrary()`

A character raises the entry the engine resolves for it, for the library tab.
A blank cell is refused, shows `Glyph.OpenFailed` and raises nothing.

## `public void DisplayTranscriptionRead_GlyphAndEmptyRows_ListsOnlyOtherFilledRows()`

The Hanja row and an empty row are left out, and nothing open lists nothing.

## `public void DisplayReflexRead_StoredEntry_AnswersWrittenRowsWithTheirAnchors()`

Only the written row stands, marked as the lead of its language, with an anchor text and no anchoring offered.

## `public void DisplayReflexResonate_ShownDraftWithoutRows_ReadsTheStoredRows()`

The notice reloads the stored entry, so its rows replace the shown draft's empty list.

## `public void DisplayReflexToggle_Opened_SetsTheSharedFoldAndRaisesTheChange()`

The gate opens the fold the editor shares and raises the change once.

## `public void DisplayPlaybackRead_AccentWithAudioOnly_ShowsTheTrayButNotTheButton()`

An accent row with audio shows the tray, while the missing own recording hides the button.
Nothing open hides both.

## `public void DisplayPlaybackStart_AccentRow_PlaysAtTheLevelAndTheCancelStopsThatPlay()`

The row plays its file and the button plays the shown draft, each at the level handed in.
Nothing open plays nothing, and the cancel stops the latest play's ticket.

## `public void DisplayPlaybackCancel_NothingPlaying_KeepsTheShownEntry()`

Stopping play with nothing playing leaves the shown entry standing.

## `public void DisplayBlocksRead_NothingShown_AnswersEmptyBlocks()`

With nothing open, the fanqie, script and paradigm blocks are empty, not pending and carry no font.

## `public void DisplayBlocksRead_EnglishEntry_AnswersNoRimeOrScriptAndTheMorphologySetting()`

An English entry has no rime-book or script rows, and its paradigm block follows the morphology setting.

## `public void DisplayDiweiAndStemOpen_ShownEntry_RaiseTheShownLanguage()`

Each click is raised in the shown entry's language, and nothing open raises nothing.
The engine names the cell kind from the initial flag.

## `public void DisplayDiweiOpen_EmptyKey_OpensNothing()`

A blank key of either kind raises nothing and answers false.

## `public void DisplayFanqieSet_ShownEntry_SetsTheRankForTheShownEntry()`

The gate hands the held rank and the raise flag for the shown entry, and nothing open sets nothing.
