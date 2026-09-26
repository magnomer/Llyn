# TPortraitExporting.cs

## `public sealed class TPortraitExporting`

Covers the portrait exports from a stored entry to a file, one format per test.
The expected strings are read from the page tree, since a writer walks nodes and never the draft.
So a new node field the reader fills turns every format red until its writer carries it.

## Inline notes

### `private static void TPortraitExportMatch(LEngine engine, long id, string written)`

One page string is named outright, so an empty tree cannot pass by having nothing to check.

### `public async Task PortraitExport_Exemplar_DocxCarriesEveryText()`

The package is opened as a zip and only the main document part is read.

### `public void PortraitRead_Exemplar_CarriesEveryText()`

The other direction: every typed string must reach the page tree, unless `TExemplarHidden` names it.
A hidden string that shows anyway is stale, so the list never outlives the reader.

### `public void PortraitExport_Exemplar_ChildCardNested()`

The child card is found once in the whole tree, and that one sits under its parent.
