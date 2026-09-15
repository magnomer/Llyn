# PFanqieItem.cs

## `public sealed class PFanqieItem`

One block of the fanqie box: the placements of one character in one rime book from one source.
They are drawn as [PFanqieLine](PFanqieLine.comment.md) rows in shared columns.
The blocks are built from the stored rows and the pack's book list, with no engine call of their own.

## `public string PFanqieItemCharacter`

The character heading the block, set on the first block of each character when the headword has several.
Empty otherwise, and the column collapses.

## `public string PFanqieItemBook`

The book's name, drawn as the chip beside the lines.
Empty when the block above already carries the same book from another source, so the chip is not repeated.

## `public string PFanqieItemSource`

The site the lines came from, drawn as the chip at the right of the block.

## `public IReadOnlyList<PFanqieLine> PFanqieItemLines`

The character's placements in that book from that source, one line each, in answer order.

## `internal static IReadOnlyList<PFanqieItem> PFanqieItemScan(IReadOnlyList<LFanqieRow> rows, IReadOnlyList<LFanqieBook> books, LHypothesis? hypothesis)`

Groups the rows by character, book and source.
Then walks the characters in headword order and the books in pack order.
A book that placed nothing for a character leaves no block.
Each row becomes a line, its reading resolved through the hypothesis when the pack declares one.
