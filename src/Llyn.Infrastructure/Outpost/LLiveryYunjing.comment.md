# LLiveryYunjing.cs
Hash: `fdb8c41af05a7c10`

## `internal static class LLiveryYunjing`

Writes the body of a rime-table category note for `LLiverySheet.LLiveryFormat`.
It reads only `LLiveryDiwei`, the `note` map and the `lookup` it is handed.

## `public static void LLiveryYunjingAppend(StringBuilder sheet, LLiveryDiwei diwei, Func<long, string> note, Func<string, string> lookup)`

Writes the category key as the heading first.
A tone key goes through `Display.FanqieTone`, as `LLiveryRime` prints tone labels.
A page without sections writes `Yunjing.DiweiEmpty` as a `llyn-vacant` span.
Each section follows in page order through `LLiverySectionAppend`.
`LLiveryXiesheng.LLiveryEntryAppend` writes the entries of the category last.

## `private static void LLiverySectionAppend(StringBuilder sheet, LDiweiSection section, IReadOnlyList<LEntry> entries, Func<long, string> note)`

Writes one section as a four-column Markdown table inside a `llyn-diwei` div.
A non-empty label stands above the table as a `llyn-heading` chip.
The header row is empty, so the table shows no column titles.
The lines come first in page order, then the tally rows of the section.

## `private static string LLiveryLineFormat(LDiweiLine line, IReadOnlyList<LEntry> entries, Func<long, string> note)`

One table row holding the reading, the label, the rounded mark and the characters.
The rounded mark is the same fixed glyph the rime table shows, and stays empty for an open line.
Each character goes through `LLiveryXiesheng.LLiveryCharacterFormat`, so a stored character links to its entry.

## `private static string LLiveryTallyFormat(LTallyRow tally)`

One table row holding the tally's language and kind chips, an empty cell and the marks.
Each mark is a `llyn-mark` chip with its text and its character count.
The empty cell keeps the marks in the column the characters use.
