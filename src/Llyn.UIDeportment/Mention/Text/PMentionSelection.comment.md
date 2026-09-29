# PMentionSelection.cs

## `internal static class PMentionSelection`

Places a popup for what the user has selected in a sentence field.
Both hosts of the gesture place it the same way, so the placing lives apart from either.

## `internal static Rect PMentionSelectionPlace(TextBox box)`

Where a popup opened for the selection should stand, in the field's own coordinates.
It is the rectangle of the selection's first character.
A field not yet laid out answers an empty rectangle, so the popup falls to the field's foot instead.
