# PMentionSelection.cs

## `internal static class PMentionSelection`

Reads what the user has selected in a sentence field as a span a Mention request can carry.
Both hosts of the gesture read the field the same way, so the reading lives apart from either.

## `internal static (int PMentionSelectionOffset, int PMentionSelectionLength) PMentionSelectionRead(TextBox box, LWindow window)`

The selection in code points, which is what every Mention is measured in.
The field counts in UTF-16 units, so the engine converts the span before anything reaches a request.
The engine also drops whitespace at either end, because a double click selects the space after a word too.

## `internal static LMentionDraft? PMentionSelectionFind(IReadOnlyList<LMentionDraft> mentions, int offset, int length)`

The Mention the selection lies inside, or nothing.
A caret with no length counts as inside when it stands anywhere on the span, its ends included.
So a right click on a linked word finds it without the word being selected first.

## `internal static Rect PMentionSelectionPlace(TextBox box)`

Where a popup opened for the selection should stand, in the field's own coordinates.
It is the rectangle of the selection's first character.
A field not yet laid out answers an empty rectangle, so the popup falls to the field's foot instead.
