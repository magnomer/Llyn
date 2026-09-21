# LFanqieGroup.cs

## `public sealed record LFanqieGroup(`

One block of the fanqie box: the rows of one character from one book and source.
The engine groups the rows, so the shell only draws the blocks it is handed.

**Parameters**

- `LFanqieGroupHeading` — The character, printed once at its first block when the entry has several.
- `LFanqieGroupLabel` — The book name, printed once and blank while the book stays the same.
- `LFanqieGroupSource` — The source the rows came from.
- `LFanqieGroupRows` — The rows of the block, as the archive stores them.
- `LFanqieGroupStems` — The character's phonetic series as separate keys, carried only by its first block.

## `public static IReadOnlyList<LFanqieGroup> LFanqieGroupScan(`

Groups the rows by character and then by book, in the order the pack lists the books.
The series of a character is put on its first block alone, so the line is printed once.
The stored series text is cut on the separator, so each series stands as its own chip.
Handing no series over leaves every block without one, as a pack declaring none does.
