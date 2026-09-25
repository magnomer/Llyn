# LIndex.cs

## `public sealed class LIndex`

The entry list of the Library panel and of each duplex wing.
It holds the rows, the empty notice and the click that reads an entry back.
The veneer hands over the controls once, and the deportment fills and steers them from then on.

## `private readonly ItemsControl _lIndexView;`

The control the rows are shown in, whose source is the held list.
Only the index touches it, so no caller reads a control's state.

## `private readonly FrameworkElement _lIndexEmpty;`

The notice shown in place of rows when a request found nothing.

## `public static IReadOnlyList<LCatalogOrder> LIndexOrder { get; } =`

The orderings an entry list offers, in the order its dropdown lists them.
The Library panel and both wings share it, so no veneer names an engine ordering.

## `public bool LIndexShown { get; private set; }`

Whether the list was last shown, held here rather than read back from the control.
A list that starts collapsed starts hidden.

## `public void LIndexShownSet(bool shown)`

Shows or collapses the list and remembers which, so the keys ask the index and not the control.

## `public void LIndexShow(IReadOnlyList<LVistaRow> rows, bool asked)`

Splices the fresh rows into the held list, so unchanged rows keep their containers and the scroll position.
The empty notice shows only for an answered request, so a list not yet asked stays blank.

## `public void LIndexClear()`

Empties the list without touching the empty notice.

## `public long? LIndexChosenRead()`

The entry of the row the engine marked chosen, or null when no listed row is.
A choice made under an earlier query may name an entry the rows no longer hold, and reads null.

## `public long? LIndexNeighbourFind(bool down)`

The neighbour of the chosen row among the held rows.

## `internal static long? LIndexNeighbourFind(IReadOnlyList<LIndexItem> items, bool down)`

The entry one row below or above the chosen one, stopping at either end.
No chosen row counts as the place before the first, so Down lands on the first row.
An empty list answers null, so the keys do nothing.
It takes the rows as an argument, so a test can pin it without a control.

## `public void LIndexEntryScroll(long id)`

Brings the row of `id` into view, when its container is built.

## `public bool LIndexHoldCheck(Visual target)`

Whether `target` sits inside the list, so a focus move into a row is not a leave.

## `public static long? LIndexEntryRead(object sender)`

The entry id of the row the sender stands for, read from its data context.
A sender without a row answers null, which leaves the panel on no entry.
