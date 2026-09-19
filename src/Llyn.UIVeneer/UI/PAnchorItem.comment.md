# PAnchorItem.cs

## `internal sealed class PAnchorItem`

One row of the anchor dropdown: a fanqie row of the character, its summary, its tick and its estimate.
Every value is copied from the row the engine already marked.

## `internal static IReadOnlyList<PAnchorItem> PAnchorItemScan(`

One item per stored fanqie row, as `LAnchor.LAnchorScan` marks them.
A row the archive has not kept yet has no id and is left out below.
