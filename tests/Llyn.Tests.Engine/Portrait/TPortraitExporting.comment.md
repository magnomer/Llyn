# TPortraitExporting.cs
Hash: `e1a07bb6c4535443`

## `public sealed class TPortraitExporting`

Covers the portrait exports from a stored entry to a file, one format per test.
The PDF export and the print are checked at the workspace's fake press instead.
The expected strings are read from the page tree, since a writer walks nodes and never the draft.
So a new node field the reader fills turns every format red until its writer carries it.

## `public async Task PortraitExport_Exemplar_HtmlCarriesEveryText()`

The exemplar written as an HTML file carries every text the page tree shows.
A node field the writer forgets turns this red, so no export silently drops a string.
The HTML body follows view mode, so it skips what the view never shows as such.
`unshown` names those texts outright.
The glyph, frequency, form and paradigm bands drop whole, since the view draws them in shapes of its own.
Two joined lines of the reflex drop too, its label line and its text line.
The view splits each of them into cells.

## `public async Task PortraitExport_ExemplarPdf_HandsTheEntrySheetToThePress()`

The fake press records the html and the path it was handed.
The html is the entry sheet, not the portrait sheet, so the PDF follows view mode.

## `public async Task PortraitPrint_StoredEntry_HandsTheEntrySheetToThePress()`

Print opens a press board over the same entry sheet, headword first.
The fake press records that a board was opened, and nothing prints until a window asks.
The fact disposes the relay, so the board never outlives it.

## `public async Task PortraitExport_Exemplar_MarkdownCarriesEveryText()`

The exemplar written as a Markdown file carries every text the page tree shows, after normalizing.
The check is the same as for HTML, so both formats answer to one list of texts.

## `public async Task PortraitExport_Exemplar_DocxCarriesEveryText()`

The package is opened as a zip and only the main document part is read.
That part is matched against the page tree like the other formats.

## `public void PortraitRead_Exemplar_CarriesEveryText()`

The other direction.
Every typed string must reach the page tree, unless `TExemplarHidden` names it.
A hidden string that shows anyway is stale, so the list never outlives the reader.

## `public void PortraitExport_Exemplar_ChildCardNested()`

The child card is found once in the whole tree, and that one sits under its parent.

## `private static void TPortraitExportMatch(LEngine engine, long id, string written, System.Func<string?, string> normalize, IReadOnlyList<string>? unshown = null)`

One page string is named outright, so an empty tree cannot pass by having nothing to check.
A top-level band whose heading `unshown` names is left out with all its texts.
Any other text `unshown` names is skipped alone.
The Markdown and Word checks pass no list, so they still answer for every text.

## `private static IReadOnlyList<LPortraitSection> TPortraitExportFind(IReadOnlyList<LPortraitSection> sections, string heading)`

Collects every section headed `heading` across the tree.
It never descends into a match, so a nested card is found under its parent only once.
