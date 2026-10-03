# TDisplaySound.cs
Hash: `35038c898c7b175c`

## `public sealed class TDisplaySound`

Covers the reading view's sound area, its reads and gates alike, on a real workspace.
The wing opens the entry, so each case stands on its display's sound area.
The playback gates live in `TDisplaySoundPlayback`.
The fanqie, script and paradigm blocks and their clicks live in `TDisplaySoundBlock`.
Both build their wing and entries through this class's helpers.

## `public void DisplayFontRead_ShownEntryOrNothingShown_ReadsThePackFontOfTheShownLanguage()`

The font read answers the shown entry's pack typography for each role.
Nothing shown answers the blank font, so the view keeps its theme.

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

Resonating reloads the stored entry, so its rows replace the shown draft's empty list.

## `public void DisplayReflexToggle_Opened_SetsTheSharedFoldAndRaisesTheChange()`

The gate opens the fold the editor shares and raises the change once.

## `internal static CWing TDisplayWingPrepare(CAtelier atelier, List<string> asked)`

A left wing whose envoy notes every question and failure in `asked`.
The other sound classes build their wing through it.

## `internal static LEntryDraft TDisplayDraftCreate(string headword, string language, IReadOnlyList<LReflexDraft> reflexes)`

One draft with a single meaning, carrying `reflexes`.

## `internal static LEntry TDisplayEnglishSave(LEngine engine)`

Stores the English entry water with no reflexes.

## `internal static LEntry TDisplayKoreanSave(LEngine engine)`

Stores a Korean entry with a plain row, a Hanja row and an empty Yale row.
