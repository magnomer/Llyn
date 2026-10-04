# QAuthorItem.cs
Hash: `44206716aeb026c4`

## `internal sealed class QAuthorItem`

Presentation item for one credit row of the Source edit area, copied from the source editor's row.
The list is spliced on each notice.
A row that keeps its id and place thus keeps its field and caret.

## `internal QAuthorItem(long id, string name, int position, bool earlier, bool later, bool blank)`

Takes each value apart, so the row's changing fields are written by the shell alone.

## `public bool QAuthorItemBlank { get; }`

Whether the row credits nobody yet.
It copies Conduct's `CAuthorRowBlank` mark, so no id value has to mean blank.
An engine row is thus never taken for the blank row, whatever its id.

## `public string QAuthorItemName`

The name the field is filled with, raised on change so a rename elsewhere reaches a kept row.

## `public bool QAuthorItemEarlier`

Whether a row above this one exists, so the move control is offered only where it can act.

## `public bool QAuthorItemLater`

Whether a row below this one exists, so the move control is offered only where it can act.

## `internal static IReadOnlyList<QAuthorItem> QAuthorItemBuild(IReadOnlyList<CAuthorRow> rows)`

One item per credit row the source editor answered, the blank row already placed among them.

## `internal static bool QAuthorItemMatch(QAuthorItem held, QAuthorItem fresh)`

Two rows are the same row when they credit the same Author at the same place, blank or not.
The name is not part of it, so a rename is synced rather than rebuilt.

## `internal static void QAuthorItemSync(QAuthorItem held, QAuthorItem fresh)`

Copies the name and both order flags of the fresh row onto the held row that `QSplice` kept.
The id and place are left, since `QAuthorItemMatch` already found them equal.
Each setter raises only on a change, so an untouched row repaints nothing.

## `internal static QAuthorItem? QAuthorItemFind(IEnumerable<QAuthorItem> rows)`

The blank row among the rows shown, which the caret goes to after an add.

## `internal static void QAuthorItemRefine(object? source)`

Writes the held name back into the field the event came from, dropping what was typed.
Anything but a credit field is left alone, so a handler may pass an event source through unchecked.

## `internal static void QAuthorItemRefine(FrameworkElement container, object item, string? change)`

Fills one credit row: its name, the order handles' enablement, and the four handle icons.
The name is refilled only when the row's name changed, so typing in a kept row is never overwritten.
