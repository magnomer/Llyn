# PContextCaret.cs

## `internal sealed class PContextCaret`

The open caret at the end of a card's Situation field.
It holds the text being typed before it becomes a Situation.
It also holds the hint shown while the field is empty.
It is not an item of the collection the committed chips sit in.
The field's `QBerth` seats its entry right before the chip the caret is anchored to.
So the caret wraps onto the next line with them.
The field grows in height instead of scrolling.

The hint is carried here rather than fixed in the template.
It belongs to the field's state.
A card already carrying a Situation has nothing left to prompt for.

## `public PContext? PContextCaretAnchor`

The chip the caret stands right before, or null when the caret stands at the end.
Only a caret step writes it, so the chips' order is never touched.
A chip the engine adds at the caret lands before the anchor, so the caret stays after it.
When the anchor chip leaves the field, the caret falls back to the end.
