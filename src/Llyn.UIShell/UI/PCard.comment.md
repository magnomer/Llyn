# PCard.cs

## Inline notes

### `public long PCardId { get; set; }`

Id of the draft card this control shows.
It is negative for a card the engine minted and positive for a stored row.
No control shows it and nothing on screen changes with it.
It is an address into the draft the engine holds, not a value the card owns.
Every request the card raises names it, and every bulletin's render finds the card by it.

### `internal void PCardIdentityApply(LCardDraft stored)`

Takes the ids the engine minted when it saved the chips and writes them into the chips and rows.
Nothing is redrawn, so the caret and the focus stay where the user left them.
Each field walks its own rows in the order the read handed them over, skipping what the read skipped.

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
