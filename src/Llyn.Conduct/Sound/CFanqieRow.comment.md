# CFanqieRow.cs
Hash: `fb5891e650e64f48`

## `public sealed record CFanqieRow(long CFanqieRowId, int CFanqieRowRepresentative, bool CFanqieRowMarked, bool CFanqieRowPrimary, string CFanqieRowOrder, bool CFanqieRowClosed, string CFanqieRowSlashed, string CFanqieRowLabel, string CFanqieRowInitial, string CFanqieRowCell, string CFanqieRowBracketed, string CFanqieRowKnotted, string CFanqieRowMedial, string CFanqieRowGraded, string CFanqieRowTone, string CFanqieRowSpelling, string CFanqieRowRemainder)`

One fanqie reading of a character, as the fanqie table writes its line.
The controller copies every derived cell, so the rime tables stay in the engine.

**Parameters**

- `CFanqieRowId`: the stored reading, which the representative buttons name.
- `CFanqieRowRepresentative`: the rank the user gave the reading, zero when unranked.
- `CFanqieRowMarked`: whether the reading carries any rank.
- `CFanqieRowPrimary`: whether the reading holds the first rank.
- `CFanqieRowOrder`: the rank written as the line shows it.
- `CFanqieRowClosed`: whether the rime is rounded.
- `CFanqieRowSlashed`: the spelling with its slash, as the reading cell shows it.
- `CFanqieRowLabel`: the book's label for the reading.
- `CFanqieRowInitial`: the initial of the reading.
- `CFanqieRowCell`: the rime cell of the rime tables.
- `CFanqieRowBracketed`: the heading written in brackets.
- `CFanqieRowKnotted`: the chongniu mark, empty when the rime has none.
- `CFanqieRowMedial`: the medial of the reading.
- `CFanqieRowGraded`: the division written as the line shows it.
- `CFanqieRowTone`: the tone of the reading.
- `CFanqieRowSpelling`: the two fanqie characters.
- `CFanqieRowRemainder`: the rest of the book's text after the spelling.
