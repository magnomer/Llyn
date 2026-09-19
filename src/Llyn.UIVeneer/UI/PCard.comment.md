# PCard.cs

## Inline notes

### `public long PCardId { get; set; }`

Id of the draft card this control shows.
It is negative for a card the engine minted and positive for a stored row.
No control shows it and nothing on screen changes with it.
It is an address into the draft the engine holds, not a value the card owns.
Every request the card raises names it, and every bulletin's render finds the card by it.

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

### `public LStateValue PTitle => _pTitle;`

The title, definition and expression are the engine's values, shown as they stand.
A field the user types into binds the value one way and reports the typing to the editor itself.
So the card holds no copy of what was typed and never resolves a state.

### `internal void PCardTitleShow(LStateValue value)`

Takes the draft's value and redraws the field only when the value differs.
A field already reading what the engine holds is left alone, so the caret survives its own echo.
