# QIndex.cs
Hash: `00fd6b5e2e1d5fd1`

## `internal sealed class QIndex`

The entry list of the Library panel and of each duplex wing.
It holds the rows and the empty notice.
The panel hands over the veneer's controls once, and fills and steers them through it from then on.
It is medium, so it lives beside the views and never in a controller.

## `private readonly ItemsControl _qIndexView;`

The control the rows are shown in, whose source is the held list.
Only the index touches it, so no caller reads a control's state.

## `private readonly FrameworkElement _qIndexEmpty;`

The notice shown in place of rows when a request found nothing.

## `internal QIndex(ItemsControl view, FrameworkElement empty)`

Sets the held list as the view's source once.
Every later fill splices that list, so the view's source never changes and its rows keep their containers.

## `internal bool QIndexShown { get; private set; }`

Whether the list was last shown, held here rather than read back from the control.
A list that starts collapsed starts hidden.

## `internal void QIndexShownRefine(bool shown)`

Shows or collapses the list and remembers which, so the keys ask the index and not the control.

## `internal void QIndexRefine(IReadOnlyList<CVistaRow> rows, bool empty)`

Splices the fresh rows into the held list, so unchanged rows keep their containers and the scroll position.
The empty notice shows as `empty` says, a verdict the panel's Conduct answers.

## `internal void QIndexClearRefine()`

Empties the list without touching the empty notice.

## `internal long? QIndexChosenRead()`

The entry of the row the engine marked chosen, or null when no listed row is.
A choice made under an earlier query may name an entry the rows no longer hold, and reads null.

## `internal void QIndexScrollRefine(long? id)`

Brings the row of `id` into view, when its container is built.
A null `id` scrolls nothing, since the gate answered no row.

## `internal bool QIndexHoldCheck(Visual target)`

Whether `target` sits inside the list, so a focus move into a row is not a leave.
