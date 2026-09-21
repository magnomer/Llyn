# LStem.cs

## `public sealed record LStem(`

One phonetic series of a language as a category of its own.
The Shengfu rows of the characters that belong to it link to this row.
It is derived from the series text rather than from placements, so it needs no Hypothesis.

**Parameters**

- `LStemId` — The database identity of the series row.
- `LStemLanguage` — The language the series belongs to.
- `LStemKey` — The series as the source printed it, such as 工.
- `LStemCount` — The number of entries whose headword carries one of its characters.
- `LStemChosen` — True while the column shows this series as the chosen one.

## `public bool LStemMatch(long id)`

True while the row is the one that identity names.

## `public static LStem? LStemFind(IReadOnlyList<LStem> rows, long id)`

The row of that identity among the rows, or null when none carries it.

## `public static IReadOnlyList<string> LStemKeyScan(string text, string separator)`

The series keys a stored series text holds, cut on the separator the language rule prints them with.
An empty separator leaves the text whole.
Blank pieces and repeats are dropped, so one character never joins the same series twice.
