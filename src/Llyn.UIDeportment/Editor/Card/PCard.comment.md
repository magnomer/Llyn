# PCard.cs
Hash: `6cce150cad8e6944`

## `internal sealed partial class PCard : INotifyPropertyChanged`

One meaning or collocation card of the editor, the item the card list draws.
Each field it shows lives in its own partial file beside this one.
It holds the engine's values as they stand, so it never resolves a state.

## `internal PCard(`

The catalog, particle, dependence and language lists are the editor's own, shared by every card.
Each Example row reads them, so one write reaches every card at once.
The Situation, Register, Translation and Tag fields set their caret hints here.
So an empty card shows its placeholders before any draft arrives.

## `internal static void PCardRowApply(FrameworkElement container, PCard card, string? changed)`

Writes a card's own parts from the card, where bindings and data triggers stood.
The badge takes the accent ring and opens for typing while the position is open.
The title, expression and meaning show their ready text and the placeholder Conduct chose.
Each field's text is rewritten only on a full fill or when its own property changed.
The badge's text is rewritten when the position moves or the badge opens or shuts.
So a change to one property leaves the caret in another field alone.
The placeholders and the badge's look follow every change.
The three icons are set here, since an icon is drawn by code.

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

The number as the badge shows it, read from the position alone.
Typed text never enters it, because a half-typed number is not an order.
The box keeps that text until the badge shuts or the card is renumbered.

### `public bool PCardPositionActive`

Whether the badge is open for writing.
The number is read-only otherwise, so a click on it drags the card as the header does.

### `internal void PCardPositionHide()`

Closes the badge, and the row fill puts the stored number back into the box.
So a number typed and then abandoned or refused leaves nothing behind.

### `public CStateWording PTitle => _pTitle;`

The title, definition and expression are the engine's values, shown as they stand.
A field the user types into binds the value one way and reports the typing to the editor itself.
So the card holds no copy of what was typed and never resolves a state.
Each arrives worded by Conduct, which also names the meaning field's hint for the card's kind.

### `internal void PCardTitleShow(CStateWording value)`

Takes the draft's value and redraws the field only when the value differs.
A field already reading what the engine holds is left alone, so the caret survives its own echo.
