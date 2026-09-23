# PAnchorItem.cs

## `internal sealed class PAnchorItem`

One row of the anchor dropdown: a fanqie row of the character, its summary, its tick and its estimate.
Every value is copied from the row the engine already marked.

## `internal static IReadOnlyList<PAnchorItem> PAnchorItemScan(IReadOnlyList<LAnchorRow> rows)`

One item per row the window's anchor scan marks.
