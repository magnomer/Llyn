# PRegisterCaret.cs

## `internal sealed class PRegisterCaret`

The open caret at the end of a card's Register field.
It holds the text being typed before it becomes a Register chip.
It also holds the hint shown while the field is empty.
It is an item of the same collection the committed chips sit in.
So the caret wraps onto the next line with them, and the field grows instead of scrolling.

The hint is carried here rather than fixed in the template, because it belongs to the field's state.
A card already marked with a Register has nothing left to prompt for.

## `internal long PRegisterCaretId { get; set; }`

The id the engine minted for the text standing in the caret, or zero before any save named it.
The chip the caret closes into takes this id, so the item keeps its identity from typing to chip.
Clearing the caret drops the id, because the item it named is gone.

## `internal long PRegisterCaretId { get; set; }`

The id the engine minted for the text standing in the caret, or zero before any save named it.
The chip the caret closes into takes this id, so the item keeps its identity from typing to chip.
Clearing the caret drops the id, because the item it named is gone.
