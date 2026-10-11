# LSheetSound.cs
Hash: `3f14e81fa423e9f4`

## `public static class LSheetSound`

Writes the reading and sound block of an exported entry body.
It also owns the reading decisions that the Joplin writer `LLiverySound` shares.
Those that write markup take an escape function, so HTML and Markdown escape their own way.

## `internal const string LSheetAnchorSeparator = " · ";`

The separator between old sounds in one cell, a middle dot with a space on each side.

## `private const char LSheetToneJoiner = '-';`

The joiner kept inside a tone run when a digit follows it, as in a sandhi contour.

## `public static void LSheetSoundAppend(StringBuilder sheet, LLiveryPage page, IReadOnlyList<string> ensigns, LTheme theme, Func<string, string> lookup)`

Writes the block in the order of `PDisplay.xaml`.
The shown reflex rows come first, then the folded ones in a closed `<details>`.
The summary of that fold is the `Reflex.More` text.
Then one table holds the accent rows, the other transcriptions and the glyph cells.
The primary accent row comes first, with its contour charts on the row under it.
An accent row with empty text is skipped.
The sheet's opener and closer, when set, wrap each accent text as brackets.
Accent text marks its tone digits when `LContour.LContourToneCheck` holds for the contour.
Each accent row shows its variety's ensign class, else the variety name.
The class comes from `LSheetEnsign.LSheetEnsignRead` over the path in `LLiveryPageEnsign`.
The flag keeps the name as its `title`, so the name still reads on hover.
The other transcriptions come from `LGlyph.LGlyphOtherRead`, labeled by scheme.
The glyph row prints each cell of `LLiveryPageCell` under the glyph's name.
Recording players stay out, as audio controls are not part of the body.
A page with nothing to show writes no block.

## `internal static (IReadOnlyList<(LReflexDraft, LReflexGuise?)>, IReadOnlyList<(LReflexDraft, LReflexGuise?)>) LSheetReflexDivide(LLiveryPage page)`

Splits the draft's written reflexes into shown and folded rows, in draft order.
A reflex's guise decides whether it folds.
`LLiveryPageFolded` decides when no guise stands at its index.

## `internal static IReadOnlyList<string> LSheetLabelScan(IReadOnlyList<(LReflexDraft, LReflexGuise?)> readings)`

Returns the language cell of each reading, in the order of `readings`.
The language is named on the first row of each run of one language, as the view leads a run.
Every later row of the run gets an empty cell.
This export and the Joplin writer `LLiverySound` both read it, so the rule has one owner.

## `internal static string LSheetSlashFormat(string text, bool phonemic)`

Wraps a phonemic reading in slashes, unless it already starts with one.

## `internal static string LSheetContourFormat(IReadOnlyList<LContour> contour, LTheme theme, Func<string, string> escape)`

Writes one `llyn-contour` span holding one chart image per syllable, in order.
Joplin's sanitizer drops every inline `<svg>`, so each chart is a standalone SVG in an `<img>` data address.
An image sees no page CSS, so every part carries its colour from `theme`.
Each chart has a grid line for every level of `LContour.LContourScale`, top level first.
Only the first chart prints the level numbers, as view mode shows them once at the left.
The syllable's levels, clamped to the scale, form one polyline with a dot at each point.
A single level draws a flat line across the chart.
Each level takes its colour role from `LContour.LContourRoleRead`, the pick view mode draws from.
A line through several colours strokes a `linearGradient`, so the colours fade like the view's line.
Each image is its own document, so the gradient id needs no index.
The face name and the syllable text pass through `escape`.

## `internal static string LSheetToneFormat(string text, bool toned, Func<string, string> escape)`

Escapes `text` through `escape`.
When `toned` holds, each run of digits sits inside a `llyn-tone` span.
A joiner between two digits stays inside the run.

## `private static void LSheetReflexAppend(StringBuilder sheet, IReadOnlyList<(LReflexDraft, LReflexGuise?)> readings, IReadOnlyList<LFanqieRow> rows, string headword)`

Writes one reflex table with the columns of the view's reflex row.
They are language, kind, reading, romanization, gloss, note and old sound.
An empty `readings` list writes no table.
Each row's language cell is its label from `LSheetLabelScan`.
The reading is the guise's shown text, or the stored text when no guise stands.
A phonemic guise slashes the reading and marks its tone digits.
A main reflex's row carries the `main` class.
The old sound is `LAnchor.LAnchorTextFormat` over the reflex's anchors and `rows`.

## `private static string LSheetRoleFormat(LContourRole role)`

Maps a contour colour role to its theme colour key by name.
An unknown role throws, so a new Core role cannot pass uncoloured.
