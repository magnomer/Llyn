# LImprint.cs

## `public sealed record LImprint(`

The edit sheet of one Source: every text and key the imprint fields show, already decided.
The fields write each value into one control and branch on nothing.

**Parameters**

- `LImprintTitle`: the title as written, or empty when never written.
- `LImprintTitleHint`: the resource key of the title field's hint.
- `LImprintYear`: the year as written, or empty when never written.
- `LImprintYearHint`: the resource key of the year field's hint.
- `LImprintUrl`: the address as written, or empty when never written.
- `LImprintUrlHint`: the resource key of the address field's hint.
- `LImprintNote`: the note as written, or empty when never written.
- `LImprintNoteHint`: the resource key of the note field's hint.
- `LImprintKindKey`: the resource key of the kind's name.
- `LImprintKindTag`: the kind's tag, which marks its entry in the kind menu.

## `public static LImprint LImprintCreate(LReference reference)`

Composes the sheet from the Source.
A value recorded unknown shows no text, and its hint turns to the unknown mark instead.
