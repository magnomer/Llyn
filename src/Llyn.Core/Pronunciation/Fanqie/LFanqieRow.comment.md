# LFanqieRow.cs
Hash: `98d612b9e92f73e1`

## `public sealed record LFanqieRow(string LFanqieRowCharacter, string LFanqieRowBook, int LFanqieRowPosition, string LFanqieRowText, string LFanqieRowInitial = "", string LFanqieRowRime = "", string LFanqieRowHeading = "", string LFanqieRowDivision = "", string LFanqieRowTone = "", bool LFanqieRowRounded = false, string? LFanqieRowSource = null, string LFanqieRowSpelling = "", string LFanqieRowReading = "", string LFanqieRowClass = "", long LFanqieRowId = 0, string LFanqieRowLabel = "", string LFanqieRowSummary = "", int LFanqieRowRepresentative = 0)`

One placement of one character in one rime book, as fetched and as stored.
The row belongs to the character, not to the entry, so every entry holding that character shares it.
The text is one line, such as `疑 模[模] 一等平`.
It reads initial, rime with its rime heading, then division and tone.
The parts the line was built from are kept beside it.
The reading the hypothesis derives from them is stored beside them too, so the view prints it as stored.

**Parameters**

- `LFanqieRowCharacter` — The single Han character the row places.
- `LFanqieRowBook` — The book name of the pack row the placement came from.
- `LFanqieRowPosition` — The row's place among the book's rows, in table order from zero.
  It is web hit order, so it keys the upsert but never the order a view shows.
- `LFanqieRowText` — The line the table cell was read into, tags dropped and spaces collapsed.
- `LFanqieRowInitial` — The initial the table row is headed by, such as 疑, or empty when the book gives none.
- `LFanqieRowRime` — The rime group the cell names, such as 模 or 真B, or empty.
  The trailing capital is the 重紐 letter, kept as fetched and read out of the key the view links to.
- `LFanqieRowHeading` — The rime heading printed after the rime, such as 模 or 桓, or empty.
- `LFanqieRowDivision` — The division the column is headed by, such as 一 or 三, or empty.
- `LFanqieRowTone` — The tone the column is headed by, such as 平 or 入, or empty.
- `LFanqieRowRounded` — Whether the source marks the placement as rounded, the 合口 medial.
- `LFanqieRowSource` — The site the placement was read from, shown beside its line, the book name when unset.
- `LFanqieRowSpelling` — The 反切 spelling the source prints for the placement, such as 五乎, or empty.
- `LFanqieRowReading` — The reading the language's hypothesis derived when the row was placed, or empty.
- `LFanqieRowClass` — The tone class the hypothesis put the reading in, or empty.
- `LFanqieRowId` — The stored id of the row, zero before the archive has kept it.
  The save upserts on the natural key, so the id survives a refetch and an Anchor on it stands.
- `LFanqieRowLabel` — The tone class as the view prints it, filled by the format pass, or empty.
- `LFanqieRowSummary` — The one line the view shows when the row is named elsewhere, filled by the format pass.
- `LFanqieRowRepresentative` — The row's place among the character's representative readings, one first, zero when unmarked.
  The ranks of one character run from one without a gap, so the number is the order itself.

## `public const int LFanqieRankLast = int.MaxValue;`

The rank that asks for a row to be placed last among its character's representative readings.
The archive clamps any rank past the end, so the largest value always means last.

## `public bool LFanqieRowMarked`

Whether the row is a representative reading at all.

## `public bool LFanqieRowPrimary`

Whether the row is the first representative reading, the one the character is read by.

## `public bool LFanqieRowParted`

Whether the line was read into parts, by an initial or a rime.
An unparted row prints its raw text and shows no medial.

## `public string LFanqieRowMedial`

開 or 合 as the source marked the placement, or empty when the line was not read into parts.

## `public string LFanqieRowCell`

The key of the rime category the row belongs to, as `LDiwei.LDiweiRimeFormat` writes it.
The view links the cell to that category by this key.

## `public string LFanqieRowRemainder`

The raw line when nothing was read into parts, else empty.
So a book without a head pattern still shows what it answered.

## `public static int LFanqieRankResolve(int rank, bool raise)`

The rank a mark request asks the archive for, given the row's current rank.
Raising moves the row up one place, and raising the first wraps it to last.
Without raising, the request toggles the mark.
An unmarked row is marked last and a marked one is unmarked.

## `public bool LFanqieRowMatch(string character, LFanqieBook book)`

Whether the row places `character` in `book` as read from that book's own site.
Two sites may serve one book, so the source is part of the match.

## `public static IReadOnlyList<string> LFanqieCharacterScan(IReadOnlyList<LFanqieRow> rows)`

The distinct characters the rows place, in first-seen order.
The grouping follows that order, so the blocks stand as the headword spells them.

## `public static IReadOnlyList<LFanqieRow>? LFanqieRowScan(IReadOnlyList<LFanqieRow> rows, string character, LFanqieBook book)`

The rows placing `character` in `book`, or null when there are none.
Null rather than empty lets the grouper skip a missing block in one pattern test.

## `public static IReadOnlyList<LFanqieRow> LFanqieRowSort(IReadOnlyList<LFanqieRow> rows, IReadOnlyList<LFanqieBook> books)`

The one declared order of fanqie rows, which every view reaches through the clerk or the grouper.
A ranked row comes before an unranked one, and lower ranks come first.
Then the rows follow the order the pack lists `books`, and a book the pack no longer lists goes last.
Then the reading sorts by ordinal, and the stored id breaks the last tie only to keep the result stable.
Fetches store rows in arrival order, so the stored order never decides what the user sees.

## `public LFanqieRow LFanqieRowFormat(string pattern)`

The row with its label and summary filled for the view.
`pattern` is the localized format around the tone class, and an empty one prints the class bare.
The summary prints the rime key, or the raw line when no key could be made.
