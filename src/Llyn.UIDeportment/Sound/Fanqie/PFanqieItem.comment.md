# PFanqieItem.cs

## `internal sealed class PFanqieItem`

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

## `public IReadOnlyList<string> PFanqieItemStems`

The character's phonetic series as separate keys, each drawn as a chip beside the book.
Empty on every block but the character's first, so the chips are drawn once.

## `public IReadOnlyList<PFanqieLine> PFanqieItemLines`

The character's placements in that book from that source, one line each, in answer order.

## `internal static void PFanqieItemApply(FrameworkElement container, object item, string? _)`

Fills a block of `Theme.Fanqie.Row`: the stem line, character, book chip, source and lines.
The stem line shows only while the block has stems, and the book chip keeps its room when blank.
The line list is attached to `PFanqieLine.PFanqieRowApply`.

## `internal static IReadOnlyList<PFanqieItem> PFanqieItemScan(IReadOnlyList<LFanqieGroup> groups)`

One block per group the engine handed over, each line copied from its row.

