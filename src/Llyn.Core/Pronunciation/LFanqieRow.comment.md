# LFanqieRow.cs

## `public sealed record LFanqieRow(`

One placement of one character in one rime book, as fetched and as stored.
The row belongs to the character, not to the entry, so every entry holding that character shares it.
The text is one line, such as `疑 模[模] 一等平`: initial, rime with its rime heading, then division and tone.
The parts the line was built from are kept beside it.
The reading the hypothesis derives from them is stored beside them too, so the view prints it as stored.

**Parameters**

- `LFanqieRowCharacter` — The single Han character the row places.
- `LFanqieRowBook` — The book name of the pack row the placement came from.
- `LFanqieRowPosition` — The row's place among the book's rows, in table order from zero.
- `LFanqieRowText` — The line the table cell was read into, tags dropped and spaces collapsed.
- `LFanqieRowInitial` — The initial the table row is headed by, such as 疑, or empty when the book gives none.
- `LFanqieRowRime` — The rime group the cell names, such as 模 or 真B, or empty.
- `LFanqieRowHeading` — The rime heading printed after the rime, such as 模 or 桓, or empty.
- `LFanqieRowDivision` — The division the column is headed by, such as 一 or 三, or empty.
- `LFanqieRowTone` — The tone the column is headed by, such as 平 or 入, or empty.
- `LFanqieRowRounded` — Whether the source marks the placement as rounded, the 合口 medial.
- `LFanqieRowSource` — The site the placement was read from, shown beside its line, the book name when unset.
- `LFanqieRowSpelling` — The 反切 spelling the source prints for the placement, such as 五乎, or empty.
- `LFanqieRowReading` — The reading the language's hypothesis derived when the row was placed, or empty.
- `LFanqieRowClass` — The tone class the hypothesis put the reading in, or empty.
