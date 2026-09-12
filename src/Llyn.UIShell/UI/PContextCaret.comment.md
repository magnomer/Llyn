# PContextCaret.cs

## `internal sealed class PContextCaret`

The open caret at the end of a card's Situation field.
It holds the text being typed before it becomes a Situation.
It also holds the hint shown while the field is empty.
It is an item of the same collection the committed Situations sit in.
So the caret wraps onto the next line with them.
The field grows in height instead of scrolling.

The hint is carried here rather than fixed in the template.
It belongs to the field's state.
A card already carrying a Situation has nothing left to prompt for.
