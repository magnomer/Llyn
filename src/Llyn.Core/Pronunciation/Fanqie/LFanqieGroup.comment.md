# LFanqieGroup.cs
Hash: `70925acd222e5431`

## `public sealed record LFanqieGroup(string LFanqieGroupHeading, string LFanqieGroupLabel, string LFanqieGroupSource, IReadOnlyList<LFanqieRow> LFanqieGroupRows, IReadOnlyList<string>? LFanqieGroupStems = null)`

One block of the fanqie box: the rows of one character from one book and source.
The engine groups the rows, so the shell only draws the blocks it is handed.

**Parameters**

- `LFanqieGroupHeading` — The character, printed once at its first block when the entry has several.
- `LFanqieGroupLabel` — The book name, printed once and blank while the book stays the same.
- `LFanqieGroupSource` — The source the rows came from.
- `LFanqieGroupRows` — The rows of the block, in the order `LFanqieRow.LFanqieRowSort` declares.
- `LFanqieGroupStems` — The character's phonetic series as separate keys, carried only by its first block.

## `public static IReadOnlyList<LFanqieGroup> LFanqieGroupScan(IReadOnlyList<LFanqieRow> rows, IReadOnlyList<LFanqieBook> books, IReadOnlyList<LShengfu>? shengfu = null, string separator = "")`

Groups the rows by character and then by book, with characters in the order the rows bring them.
The rows are sorted by `LFanqieRow.LFanqieRowSort` first, whatever order the caller hands them in.
A book's block stands where its first sorted row stands.
So a book holding a ranked row comes first, and unranked books keep the pack's order.
The series of a character is put on its first block alone, so the line is printed once.
The stored series text is cut on the separator, so each series stands as its own chip.
Handing no series over leaves every block without one, as a pack declaring none does.

## `public static string LFanqieReadingFormat(IReadOnlyList<LFanqieGroup> groups, string headword)`

The headword's representative readings, drawn under the headword itself, such as `/bhiaeng, bhien 'an/`.
Each character gives its marked readings through `LFanqieMarkedScan`, in the order the headword writes the characters.
The line itself is laid out by the overload below, so the headword and a series member print alike.

## `public static string LFanqieReadingFormat(IReadOnlyList<IReadOnlyList<string>> characters)`

The representative line of characters whose marked readings are already scanned, one list per character.
Each character's readings are separated by commas, the characters by a space, and the whole stands between slashes.
A character with no marked reading is left out.
Nothing ranked at all gives an empty line, and the view then shows none.
A series member hands its one list here, so the series page prints the line the entry page prints.

## `public static IReadOnlyList<string> LFanqieMarkedScan(IReadOnlyList<LFanqieRow> rows, string character)`

The readings of the character's marked rows in rank order, rows of equal rank kept in the order given.
A row the user never marked as representative, or that stored no reading, is left out.
The mark is the user's per-row choice, stored as the `representative` rank of the row.

