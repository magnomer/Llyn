# LSheetEntry.cs
Hash: `450e69ebb2151aca`

## `public static class LSheetEntry`

The sheet writer of an entry's HTML export, its PDF and its print.
It reads the Livery page beside the portrait page, so the body follows view mode.
Catalog pages and the Markdown and Word exports keep `LSheet` alone.

## `public static string LSheetEntryFormat(LLiveryPage livery, LPortraitPage portrait, LPortraitLabel label, LTheme theme, Func<string, string> lookup)`

Writes the document head as `LSheet.LSheetFormat` does, with `LSheetCascade` in place of the catalog style.
It adds one style rule per flag the Livery page shows.
Each flag is embedded once there, so the body names it by class instead of repeating its bytes.
The body is one `<main class="entry">` in the order of `PDisplay.xaml`.
The header, the sound block, the speech line and the frequency pill come from the Livery page.
The paradigm box follows through `LSheetParadigm.LSheetParadigmAppend`, where the view places `PDisplayParadigm`.
The lower body follows in the order of `PDisplay.xaml`.
It runs the fanqie box, the Meaning deck, the Other band, incoming, etymology, the note, then the script box.
Etymology and the note are two calls, each writing its own band.
Their content still comes from the portrait sections, so only the frame differs from `LSheet`.
The Meaning deck is the one deck of numbered cards, through `LSheetDeck.LSheetDeckAppend`.
The Other band holds the collocations with no home among the Meanings, under `LPortraitLabelOther`.
It is written through `LSheetSection.LSheetSectionAppend`, so each row matches a homed collocation's row.
The portrait page picks them, so a linked collocation shows once under its home.
The glyph and frequency bands are left out, since the sound block and the pill already show them.
The form and paradigm bands are left out too, since the paradigm box replaces them.
The stamp footer closes the body.

## `private static void LSheetSpeechAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)`

Writes the unit and freedom line and the speech chips, as `PDisplaySpeechSection` shows them.
Nothing prints unless the draft is marked, the same test the view applies.
Unit and freedom are worded through `LUnitKey` and `LFreedomKey`, and each prints only when set.

## `private static void LSheetFrequencyAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)`

Writes one pill from `LFrequencyGauge.LFrequencyGaugeResolve`, with no heading.
The pill shows the tier name and carries the gauge's sources as its `title`.
The band prints `LFrequencyGaugeBand` lit pips and `LFrequencyGaugeSpare` dim ones.
An unranked gauge shows its tier name alone, as the view hides the band.
The tier class lets the style colour each tier as the view does.

## `private static void LSheetStampAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)`

Writes the rule and the Created and Updated grid that close the view.
The times are the Livery page's ready strings, never formatted here.
