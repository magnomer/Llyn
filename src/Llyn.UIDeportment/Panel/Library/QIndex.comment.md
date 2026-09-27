# QIndex.cs

## `internal sealed class QIndex`

The entry list of the Library panel and of each duplex wing.
It holds the rows, the empty notice and the click that reads an entry back.
The panel hands over the veneer's controls once, and fills and steers them through it from then on.
It is medium, so it lives beside the views and never in a controller.

## `private readonly ItemsControl _qIndexView;`

The control the rows are shown in, whose source is the held list.
Only the index touches it, so no caller reads a control's state.

## `private readonly FrameworkElement _qIndexEmpty;`

The notice shown in place of rows when a request found nothing.

## `internal static IReadOnlyList<CCatalogOrder> QIndexOrder { get; } =`

The orderings an entry list offers, in the order its dropdown lists them.
The Library panel and both wings share it, so no veneer names an engine ordering.

## `internal bool QIndexShown { get; private set; }`

Whether the list was last shown, held here rather than read back from the control.
A list that starts collapsed starts hidden.

## `internal void QIndexShownSet(bool shown)`

Shows or collapses the list and remembers which, so the keys ask the index and not the control.

## `internal void QIndexShow(IReadOnlyList<CVistaRow> rows, bool asked)`

Splices the fresh rows into the held list, so unchanged rows keep their containers and the scroll position.
The empty notice shows only for an answered request, so a list not yet asked stays blank.

## `internal void QIndexClear()`

Empties the list without touching the empty notice.

## `internal long? QIndexChosenRead()`

The entry of the row the engine marked chosen, or null when no listed row is.
A choice made under an earlier query may name an entry the rows no longer hold, and reads null.

## `internal long? QIndexNeighbourFind(bool down)`

The neighbour of the chosen row among the held rows.

## `internal static long? QIndexNeighbourFind(IReadOnlyList<QIndexItem> items, bool down)`

The entry one row below or above the chosen one, stopping at either end.
No chosen row counts as the place before the first, so Down lands on the first row.
An empty list answers null, so the keys do nothing.
It takes the rows as an argument, so a test can pin it without a control.

## `internal void QIndexEntryScroll(long id)`

Brings the row of `id` into view, when its container is built.

## `internal bool QIndexHoldCheck(Visual target)`

Whether `target` sits inside the list, so a focus move into a row is not a leave.
