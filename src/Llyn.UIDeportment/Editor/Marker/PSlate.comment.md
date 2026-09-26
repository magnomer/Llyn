# PSlate.cs

## `public partial class PEditor`

The dropdown of tags the workspace already holds that a typed tag may already name, and the rows it offers.
It is one popup the editor owns rather than one per card, for the reason the situation dropdown is.
Only one card is being typed into at a time.
The popup is retargeted at the caret that opened it.

A tag is its own text, so a row carries no id.
Two cards meaning the same tag should spell it the same way.
Without this the same idea would be filed under two spellings and the taxonomy panel would browse both.

The list opens as the tag is typed rather than only when it is committed.
A user cannot pick from a catalogue they were never shown.

## `internal void PSlateHandle(object sender, MouseButtonEventArgs e)`

Takes the row the pointer chose out of the dropdown and writes that tag onto the card.

## `private readonly PSlateTemplate _pSlateTemplate`

The offered tag dictionary, held so its fill can subscribe the dictionary's forwarders.

## `private void PSlateApply(FrameworkElement container, object item, string? _)`

Fills one offered tag row, where bindings and an event attribute stood, and subscribes its press.

## `private void PSlateAttach()`

Hands the slate list its rows and fill, and gives the dropdown its placement.

## Inline notes

### `private bool PSlateHandle(Key key)`

The dropdown never takes focus, so the caret's key handler drives it.
Nothing is selected while the list merely stands open, so enter still commits the typed tag.
The user reaches the list with the arrows, and only then does enter take a row.
Down from nothing selects the first row and up selects the last.

### `PSlate.PlacementTarget = PSlateFrameFind(box) ?? box ?? (UIElement)PContents;`

The frame inside the caret, because that is the edge the dropdown is read against.
The caret it is drawn for falls back in, then the pane.
The list still opens when the frame is not built yet.
The sheet's minimum width is bound to that target's width, so it follows a resize while open.

### `private static CustomPopupPlacement[] PSlatePlace(Size popup, Size target, Point offset)`

The dropdown is placed against the frame by hand, from the frame's own corner.
Placing it under the frame's edge instead left it short of that edge and over the caret.
The gutter the surface keeps for its shadow is taken back on both counts.
The list reads as the frame's own edge continued.
The second placement puts it above the frame, for a caret near the foot of the screen.

### `private void PSlateShow(PCard card, string text)`

The tag is matched anywhere inside a stored tag, ignoring case, because this is a search rather than a prefix.
So "verb" finds "phrasal verb".
A tag the card already carries is left out, since it cannot be written twice.
The list stays shut when nothing matches, rather than standing empty.

### `private void PSlateSelect(PSlateItem item)`

Asks the engine to link the Tag the user chose, at the caret, and empties the entry.
A Tag the card already carries, by id or by text, is not asked for again.
