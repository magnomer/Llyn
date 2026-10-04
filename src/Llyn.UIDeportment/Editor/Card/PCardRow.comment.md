# PCardRow.cs
Hash: `cff92a97c742cac2`

## `internal sealed partial class PCard`

The one diff every list inside a card renders through.
The engine holds the list and owns its order, and the card only copies it.
So a bulletin brings the whole list back, and the card must change only what differs.
Rows are paired by id and only removed by identity or appended.
The driver may never insert or move a row, because the engine alone owns the order.
The longest head of the new order already standing keeps its objects and controls.
Every engine row after the first mismatch is re-added, so its object survives but its control is rebuilt.

## `internal static void PCardRowShow<PCardItem, PCardDraft>(ObservableCollection<PCardItem> rows, IReadOnlyList<PCardDraft> drafts, Func<PCardItem, long?> key, Func<PCardDraft, long> id, Func<PCardDraft, PCardItem> create, Func<PCardItem, PCardDraft, PCardItem> update)`

Makes `rows` show `drafts` in their order, duplicates included, around any row it does not own.
Each draft claims the first unclaimed row carrying its id, so pairing is one to one.
A draft with no such row is built with `create`.
`update` may answer a fresh object, which then takes the paired row's place.
Only removal by identity and appending are used, because the engine alone owns the order.
The rows kept untouched are the longest head of the new order already standing in that sequence.
They are compared by object identity, and each keeps its object and its control.
Every engine row after the first mismatch is removed and re-added.
Its object survives, but its control is rebuilt and loses focus and caret.
This includes rows the change never passed.
Moving B to the front of A, B, C, D rebuilds A, C and D.
An insert in the middle rebuilds every later row.
Moving a row to the end rebuilds only that row.
That cost is the price of never inserting or moving.
A fresh object from `update` counts as a mismatch, so it and every later row are re-added.
Conduct does not promise unique ids, so repeated ids show faithfully and never throw.
Removing repeats is behaviour, owned by Conduct or a layer below it, never by the driver.
A row `key` answers null for is not the engine's, so it is never removed or re-added.
It keeps its place among the rows, and the drafts show in order around it.
No caller holds such a row today, because every key answers an id.

## `private static int PCardCaretFind<PCardItem>(IReadOnlyList<PCardItem> rows, PCardItem? anchor)`

The place of the chip a caret is anchored to, or the end when no chip carries it.
That is the position a chip committed at the caret asks the engine for.

## `private static List<PCardItem> PCardCaretRead<PCardItem>(IReadOnlyList<PCardItem> rows, PCardItem? anchor)`

The anchor and every chip after it, read before the rows change.

## `private static PCardItem? PCardCaretResolve<PCardItem>(IReadOnlyList<PCardItem> rows, IReadOnlyList<PCardItem> trail)`

The first chip of the trail that still stands, which becomes the caret's anchor.
Null when none stands, so the caret goes to the end.
The anchor is a chip object, not an id, so no value is derived from the engine's ids.
