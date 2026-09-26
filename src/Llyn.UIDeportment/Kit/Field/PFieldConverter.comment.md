# PFieldConverter.cs

## `internal sealed class PFieldConverter`

Measures the frame a written field is drawn in so every field carries the same frame.
A card writes its fields at the sizes and faces the reading view draws them in.
A frame hugging each of those lines would stand at a different height on every field.
So the frame is measured outward from the line instead, to one height for the whole card.

The frame is a sibling drawn behind the text with a negative margin, never padding.
A field therefore keeps the text where the reading view puts it whatever the frame does.

## Inline notes

### `private const double PFieldConverterHeight = 31;`

One height for every written field, and the height the card's own buttons stand at.

### `var pLine = pFamily.LineSpacing * pSize;`

The height one line of this field's face takes, which is what the text host is given.
A face is asked for its own line spacing rather than assumed, so a change of face still measures true.

### `var pVertical = Math.Max(0, pRoom);`

A field whose line already fills the frame is left hugged rather than pushed inward.
A frame is never drawn inside the text it frames.
