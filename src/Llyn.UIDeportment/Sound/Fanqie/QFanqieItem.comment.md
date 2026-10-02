# QFanqieItem.cs

## `internal sealed class QFanqieItem`

One block of the fanqie box: the placements of one character in one rime book from one source.
They are drawn as [QFanqieLine](QFanqieLine.comment.md) rows in shared columns.
The blocks are built from the stored rows and the pack's book list, with no engine call of their own.

## `public string QFanqieItemCharacter`

The character heading the block, set on the first block of each character when the headword has several.
Empty otherwise, and the column collapses.

## `public string QFanqieItemBook`

The book's name, drawn as the chip beside the lines.
Empty when the block above already carries the same book from another source, so the chip is not repeated.

## `public string QFanqieItemSource`

The site the lines came from, drawn as the chip at the right of the block.

## `public IReadOnlyList<string> QFanqieItemStems`

The character's phonetic series as separate keys, each drawn as a chip beside the book.
Empty on every block but the character's first, so the chips are drawn once.

## `public IReadOnlyList<QFanqieLine> QFanqieItemLines`

The character's placements in that book from that source, one line each, in answer order.

## `internal static void QFanqieItemRefine(FrameworkElement container, object item, string? _)`

Fills a block of `Theme.Fanqie.Row`: the stem line, character, book chip, source and lines.
The stem line shows only while the block has stems, and the book chip keeps its room when blank.
The line list is attached to `QFanqieLine.QFanqieLineRefine`.

## `internal static IReadOnlyList<QFanqieItem> QFanqieItemScan(IReadOnlyList<CFanqieGroup> groups)`

One block per group the engine handed over, each line copied from its row.

