# LShengfu.cs

## `public sealed record LShengfu(`

The phonetic series one Han character belongs to, as one stored row per language and character.
A series names the character whose sound the graph was borrowed for, such as 工 for 江.
It hangs off no entry, because a character is shared by every entry written with it.

**Parameters**

- `LShengfuCharacter` — The character the series was fetched for.
- `LShengfuText` — The series as printed, with several joined by the rule's separator.
- `LShengfuSource` — The source the series came from, printed nowhere and kept for provenance.

## `public bool LShengfuWritten`

True while the row carries a series to print.

## `public static string LShengfuTextFind(IReadOnlyList<LShengfu> rows, string character)`

The series of that character among the rows, or empty when none was stored.
