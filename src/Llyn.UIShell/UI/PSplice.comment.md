# PSplice.cs

## `internal static class PSplice`

Joins a freshly built row list onto the rows a list control already shows.
When every row still shows the same values, only the chosen marks are copied across.
So choosing a row moves a mark on two rows and keeps every container and the scroll.
Any other difference refills the list whole, as a changed list always did.

## `internal static void PSpliceApply<PSpliceRow>(ObservableCollection<PSpliceRow> held, IReadOnlyList<PSpliceRow> fresh, Func<PSpliceRow, PSpliceRow, bool> match, Action<PSpliceRow, PSpliceRow> mark)`

Marks the held rows in place when the fresh list matches them pair by pair, else clears and refills.
The match and the mark are the item's own, so this helper never reads a logic value.
No row is moved or overwritten in place, because the walker treats that as reordering a store.

## `private static bool PSpliceMatchCheck<PSpliceRow>(ObservableCollection<PSpliceRow> held, IReadOnlyList<PSpliceRow> fresh, Func<PSpliceRow, PSpliceRow, bool> match)`

Whether the two lists are the same length and match at every place.
