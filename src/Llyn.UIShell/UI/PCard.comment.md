# PCard.cs

## Inline notes

### `public string PCardId { get; set; } = string.Empty;`

Id of the stored row this card was loaded from, empty for a card typed into an empty form.
No control shows it and nothing on screen changes with it.
It travels with the card so a saved form can say which stored row each card is.
That is what lets an update change the row a card came from instead of writing it again.

### `public int PCardPosition`

The number this card is shown by, counted from one.
It is stored rather than derived, so the card carries the same number the draft was saved with.
Changing it retitles the card, because the header reads the prefix and this number.

### `public string PCardPositionText`

The number as the badge shows it, and as a writer types over it.
It follows the position on every renumber, so the badge never lags the card.
Typing changes this alone, because a half-typed number is not an order.

### `public bool PCardPositionActive`

Whether the badge is open for writing.
The number is read-only otherwise, so a click on it drags the card as the header does.

### `internal void PCardPositionHide()`

Closes the badge and puts the stored number back into it.
A number typed and then abandoned leaves nothing behind.
