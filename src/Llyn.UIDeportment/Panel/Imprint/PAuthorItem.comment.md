# PAuthorItem.cs

## `internal sealed class PAuthorItem`

Presentation item for one credit row of the Source edit area, copied from the engine's row.
The list is spliced on each notice.
A row that keeps its id and place thus keeps its field and caret.

## `internal PAuthorItem(long id, string name, int position, bool earlier, bool later)`

Takes each value apart, so the row's changing fields are written by the shell alone.

## `public bool PAuthorItemBlank => PAuthorItemId == 0;`

Whether the row credits nobody yet.
A stored Author has a positive id and a minted one a negative id.
So zero is free to mean neither.

## `public string PAuthorItemName`

The name the field is filled with, raised on change so a rename elsewhere reaches a kept row.

## `public bool PAuthorItemEarlier`

Whether a row above this one exists, so the move control is offered only where it can act.

## `internal static IReadOnlyList<PAuthorItem> PAuthorItemBuild(IReadOnlyList<LAuthorRow> rows, int blankAt)`

One item per engine row, with the blank row spliced in at the index the deportment names.
The rows are counted here as they are copied, so no engine count enters the compare.
The blank row takes the position of the credit it precedes, so an addition lands there.
A negative index adds no blank row, and one equal to the count adds it last.

## `internal static bool PAuthorItemMatch(PAuthorItem held, PAuthorItem fresh)`

Two rows are the same row when they credit the same Author at the same place, blank or not.
The name is not part of it, so a rename is synced rather than rebuilt.

## `internal static PAuthorItem? PAuthorItemFind(IEnumerable<PAuthorItem> rows)`

The blank row among the rows shown, which the caret goes to after an add.

## `internal static void PAuthorItemRestore(object? source)`

Writes the held name back into the field the event came from, dropping what was typed.
Anything but a credit field is left alone, so a handler may pass an event source through unchecked.

## `internal static void PAuthorItemApply(FrameworkElement container, object item, string? change)`

Fills one credit row: its name, the order handles' enablement, and the four handle icons.
The name is refilled only when the row's name changed, so typing in a kept row is never overwritten.
