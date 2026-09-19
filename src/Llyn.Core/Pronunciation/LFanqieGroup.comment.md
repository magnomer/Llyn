# LFanqieGroup.cs

## `public sealed record LFanqieGroup(`

One block of the fanqie box: the rows of one character from one book and source.
The engine groups the rows, so the shell only draws the blocks it is handed.

**Parameters**

- `LFanqieGroupHeading` — The character, printed once at its first block when the entry has several.
- `LFanqieGroupLabel` — The book name, printed once and blank while the book stays the same.
- `LFanqieGroupSource` — The source the rows came from.
- `LFanqieGroupRows` — The rows of the block, as the archive stores them.

## `public static IReadOnlyList<LFanqieGroup> LFanqieGroupScan(`

Groups the rows by character and then by book, in the order the pack lists the books.
