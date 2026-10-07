# LLiverySound.cs
Hash: `d67e88c612f83b6c`

## `internal static class LLiverySound`

Writes the reading and sound part of a Joplin entry body for `LLiverySheet.LLiveryFormat`.
It reads only `LLiveryPage` and the `lookup` it is handed.

## `private const string LLiveryAnchorSeparator = " · ";`

The separator between old sounds in one cell, a middle dot with a space on each side.

## `private const char LLiveryToneJoiner = '-';`

The joiner kept inside a tone run when a digit follows it, as in a sandhi contour.

## `public static void LLiverySoundAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup, LTheme theme)`

Splits the draft's written reflexes into shown and folded rows in draft order.
A reflex's guise decides whether it folds, and `LLiveryPageFolded` decides when no guise stands at its index.
The shown rows form one table and the folded rows a second one.
The second table sits inside one closed `<details>` whose summary is the `Reflex.More` text.
The old sounds read every stored fanqie row of `LLiveryPageFanqie`.
Then one two-column table holds the accent rows, the other transcriptions and the glyph row.
An accent row is the sheet's opener, text and closer, with tone digits marked when the contour is toned.
Its label is the variety's flag from `LLiveryPageEnsign` through `LLiveryHeader.LLiveryFlagFormat`.
The primary row pairs with the draft's first pronunciation, and any other row with the pronunciation of its id.
That pronunciation's recording adds a player through `LLiveryPlayerFormat` at the row's end.
The play button stands there in view mode, right after the accent text.
A row under the primary row holds the contour charts from `LLiveryContourFormat` when the contour has syllables.
The label of that row stays empty, as view mode draws the charts under the accent.
The flag stands in for the variety name, and the name prints when no flag reads.
The other transcriptions come from `LGlyph.LGlyphOtherRead`, labeled by scheme.
The glyph row joins `LLiveryPageCell` under the glyph's name.
Each table is left out when it has no row.

## `private static void LLiveryReadingAppend(StringBuilder sheet, List<(LReflexDraft, LReflexGuise?)> readings, IReadOnlyList<LFanqieRow> rows, string headword)`

Writes one five-column table of language, kind, reading, note and old sound, with an empty header row.
The language is named on the first row of each run of the same language.
A respelled guise shows the respelling when one is stored.
A phonemic guise wraps the reading in slashes and marks its tone digits.
A main reflex's reading carries `llyn-main`.
The note cell holds `LReflexDraftNote` in a `llyn-note` span, and stays empty when no note is stored.
The old sound is `LAnchor.LAnchorTextFormat` over the reflex's anchors and `rows`.

## `private static void LLiveryRowAppend(StringBuilder sheet, List<(string, string)> lines)`

Writes one two-column table of ready label markup and ready cell markup, with an empty header row.

## `private static string LLiveryPlayerFormat(LPronunciationDraft? recorded)`

Writes a space and an `<audio>` player on the stored recording path of `recorded`.
It starts with `LLiverySheet.LLiveryAudioHead`, so the sheet finds the player and swaps it for a resource link.
No pronunciation or an empty audio path writes nothing.

## `private static string LLiveryContourFormat(IReadOnlyList<LContour> contour, LTheme theme)`

Writes one `llyn-contour` span holding one chart image per syllable, in order.
Joplin's sanitizer drops every inline `<svg>` and keeps its stray children as bare text.
So each chart is a standalone SVG inside an `<img>` data address.
The sheet then lifts each image into an `image/svg+xml` parcel, like a flag.
An image sees no note CSS, so every part carries its colour from `theme`.
Each chart has a grid line for every level of `LContour.LContourScale`, top level first.
Only the first chart prints the level numbers, as view mode shows them once at the left.
The syllable's levels, clamped to the scale, form one polyline with a dot at each point.
A single level draws a flat line across the chart.
Each level takes its role from `LContour.LContourRoleRead`, the pick view mode draws from.
`LLiveryRoleFormat` turns the role into its theme colour key, read through `theme`.
Each dot paints the colour of its level.
A line whose points share one colour strokes that colour.
A line through several colours strokes a `linearGradient` whose stops carry those colours.
The gradient spans the line in user space, so the colours fade like the view's line.
Each image is its own document, so the gradient id needs no index.
The grid takes `line`, the level numbers `muted` and the syllable text `ink`.
The syllable text is XML-escaped only, as Markdown never reads inside the image.

## `private static string LLiveryRoleFormat(LContourRole role)`

Maps a contour colour role to its theme colour key by name.
An unknown role throws, so a new Core role cannot pass uncoloured.

## `private static string LLiveryToneFormat(string text, bool toned)`

Escapes `text` through `LLiveryHeader.LLiveryTextFormat`.
When `toned` holds, each run of digits sits inside a `llyn-tone` span.
