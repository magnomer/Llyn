# LLiverySound.cs
Hash: `288e1a48cfec5345`

## `internal static class LLiverySound`

Writes the reading and sound part of a Joplin entry body for `LLiverySheet.LLiveryFormat`.
It reads only `LLiveryPage` and the `lookup` it is handed.

## `public static void LLiverySoundAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup, LTheme theme)`

Splits the draft's written reflexes into shown and folded rows through `LSheetSound.LSheetReflexDivide`.
The HTML export splits them through the same helper, so both fold the same rows.
The shown rows form one table and the folded rows a second one.
The second table sits inside one closed `<details>` whose summary is the `Reflex.More` text.
The old sounds read every stored fanqie row of `LLiveryPageFanqie`.
Then one two-column table holds the accent rows, the other transcriptions and the glyph row.
An accent row with empty text is skipped.
An accent row is the sheet's opener, text and closer, with tone digits marked when the contour is toned.
Its label is the variety's flag from `LLiveryPageEnsign` through `LLiveryHeader.LLiveryFlagFormat`.
The primary row pairs with the draft's first pronunciation, and any other row with the pronunciation of its id.
That pronunciation's recording adds a player through `LLiveryPlayerFormat` at the row's end.
The play button stands there in view mode, right after the accent text.
A row under the primary row holds the contour charts when the contour has syllables.
They come from `LSheetSound.LSheetContourFormat`, escaped with `WebUtility.HtmlEncode`.
The label of that row stays empty, as view mode draws the charts under the accent.
The flag stands in for the variety name, and the name prints when no flag reads.
The other transcriptions come from `LGlyph.LGlyphOtherRead`, labeled by scheme.
The glyph row joins `LLiveryPageCell` under the glyph's name.
Each table is left out when it has no row.

## `internal static void LLiveryReadingAppend(StringBuilder sheet, IReadOnlyList<(LReflexDraft, LReflexGuise?)> readings, IReadOnlyList<LFanqieRow> rows, string headword)`

Writes one seven-column table, with an empty header row.
The columns are language, kind, reading, romanization, gloss, note and old sound, as the app's reflex rows.
The language cell comes from `LSheetSound.LSheetLabelScan`, so it is named on the first row of each run only.
The reading is the guise's `LReflexGuiseShown`, which the engine filled from the stored text and respelling.
A row without a guise prints its stored text.
A phonemic guise wraps the reading through `LSheetSound.LSheetSlashFormat` and marks its tone digits.
A main reflex's reading carries `llyn-main`.
The romanization and gloss cells hold the stored `LReflexDraftRomanization` and `LReflexDraftMeaning`, empty when blank.
The note cell holds `LReflexDraftNote` in a `llyn-note` span, and stays empty when no note is stored.
The old sound is `LAnchor.LAnchorTextFormat` over the reflex's anchors and `rows`.
Its separator is `LSheetSound.LSheetAnchorSeparator`, shared with the HTML export.
No readings write nothing.
`LLiveryXiesheng` writes each series member's reflex rows through it, with no fanqie rows, so the old sound stays empty there.

## `private static void LLiveryRowAppend(StringBuilder sheet, List<(string, string)> lines)`

Writes one two-column table of ready label markup and ready cell markup, with an empty header row.

## `private static string LLiveryPlayerFormat(LPronunciationDraft? recorded)`

Writes a space and an `<audio>` player on the stored recording path of `recorded`.
It starts with `LLiverySheet.LLiveryAudioHead`, so the sheet finds the player and swaps it for a resource link.
No pronunciation or an empty audio path writes nothing.

## `private static string LLiveryToneFormat(string text, bool toned)`

Marks tone digits through `LSheetSound.LSheetToneFormat`, escaping through `LLiveryHeader.LLiveryTextFormat`.
The digit rule lives there once, shared with the HTML export.
