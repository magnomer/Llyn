# PCardRow.cs
Hash: `6ac3c59901a6adab`

## `internal sealed partial class PCard`

The one diff every list inside a card renders through.
The engine holds the list, and the card only shows it.
So a bulletin brings the whole list back, and the card must change only what differs.
Rows are matched by id, updated in place, added, removed and reordered, never rebuilt.
A row the user is typing in is therefore the same object after the bulletin as before it.

## `internal static void PCardRowShow<PCardItem, PCardDraft>(ObservableCollection<PCardItem> rows, IReadOnlyList<PCardDraft> drafts, Func<PCardItem, long?> key, Func<PCardDraft, long> id, Func<PCardDraft, PCardItem> create, Func<PCardItem, PCardDraft, PCardItem> update)`

Makes `rows` show `drafts`, matching each draft to the row carrying its id.
A row carrying an id the drafts no longer name is removed first.
Then each draft in turn is found and offered `update`, or built with `create` and placed.
A row `key` answers null for is not the engine's and is stepped over.
A new row lands right after the previous draft's row.
A row found out of order is moved back to its place.
`update` may answer a fresh object, and the collection slot is replaced when it does.
So an immutable chip is redrawn by replacement and a mutable row is edited in place.

## `internal static int PCardRowFind<PCardItem>(IReadOnlyList<PCardItem> rows, Func<PCardItem, long?> key, long id)`

The place of the row carrying `id`, or minus one.

## `private static int PCardCaretFind<PCardItem>(IReadOnlyList<PCardItem> rows, PCardItem? anchor)`

The place of the chip a caret is anchored to, or the end when no chip carries it.
That is the position a chip committed at the caret asks the engine for.

## `private static List<PCardItem> PCardCaretRead<PCardItem>(IReadOnlyList<PCardItem> rows, PCardItem? anchor)`

The anchor and every chip after it, read before the rows change.

## `private static PCardItem? PCardCaretResolve<PCardItem>(IReadOnlyList<PCardItem> rows, IReadOnlyList<PCardItem> trail)`

The first chip of the trail that still stands, which becomes the caret's anchor.
Null when none stands, so the caret goes to the end.
The anchor is a chip object, not an id, so no value is derived from the engine's ids.
