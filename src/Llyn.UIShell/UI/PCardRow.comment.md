# PCardRow.cs

## `internal sealed partial class PCard`

The one diff every list inside a card renders through.
The engine holds the list, and the card only shows it.
So a bulletin brings the whole list back, and the card must change only what differs.
Rows are matched by id, updated in place, added, removed and reordered, never rebuilt.
A row the user is typing in is therefore the same object after the bulletin as before it.

## `internal static void PCardRowShow<PCardItem, PCardDraft>(`

Makes `rows` show `drafts`, matching each draft to the row carrying its id.
A row carrying an id the drafts no longer name is removed first.
Then each draft in turn is found and offered `update`, or built with `create` and placed.
A row `key` answers null for, such as the caret, is not the engine's and is stepped over.
A new row lands right after the previous draft's row, so it sits before the caret the user typed into.
A row found out of order is moved back to its place.
`update` may answer a fresh object, and the collection slot is replaced when it does.
So an immutable chip is redrawn by replacement and a mutable row is edited in place.

## `internal static int PCardRowFind<PCardItem>(IReadOnlyList<PCardItem> rows, Func<PCardItem, long?> key, long id)`

The place of the row carrying `id`, or minus one.

## `internal static int PCardRowResolve<PCardItem>(IReadOnlyList<PCardItem> rows, Func<PCardItem, long?> key, int limit)`

How many of the engine's rows stand before `limit`.
That is the position a chip committed at the caret asks the engine for.
