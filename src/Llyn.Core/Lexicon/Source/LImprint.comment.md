# LImprint.cs
Hash: `d4089ba314089697`

## `public sealed record LImprint(string LImprintTitle, string LImprintTitleHint, string LImprintYear, string LImprintYearHint, string LImprintUrl, string LImprintUrlHint, string LImprintNote, string LImprintNoteHint, string LImprintKindKey, string LImprintKindTag)`

The edit sheet of one Source: every text and key the imprint fields show, already decided.
The fields write each value into one control and branch on nothing.

**Parameters**

- `LImprintTitle` — The title as written, or empty when never written.
- `LImprintTitleHint` — The resource key of the title field's hint.
- `LImprintYear` — The year as written, or empty when never written.
- `LImprintYearHint` — The resource key of the year field's hint.
- `LImprintUrl` — The address as written, or empty when never written.
- `LImprintUrlHint` — The resource key of the address field's hint.
- `LImprintNote` — The note as written, or empty when never written.
- `LImprintNoteHint` — The resource key of the note field's hint.
- `LImprintKindKey` — The resource key of the kind's name.
- `LImprintKindTag` — The kind's tag, which marks its entry in the kind menu.

## `public static LImprint LImprintCreate(LReference reference)`

Composes the sheet from the Source.
A value recorded unknown shows no text, and its hint turns to the unknown mark instead.
