# PCandidate.cs

## `public partial class PEditor`

The dropdown of stored Situations a typed wording may already name, and the rows it offers.
It is one popup the editor owns rather than one per card, for the reason the translation dropdown is.
Only one card is being typed into at a time.
The popup is retargeted at the caret that opened it.

A Situation is a workspace entity, not a word on a card.
It carries a description, a kind and a usage count.
Two cards naming the same wording should name the same Situation.
Without this the wording would be written twice and the workspace would hold two Situations that read alike.

The list opens as the wording is typed rather than only when it is committed.
A user cannot pick from a catalogue they were never shown.

## `internal void PCandidateHandle(object sender, MouseButtonEventArgs e)`

Takes the row the pointer chose out of the dropdown and attaches that Situation.

## Inline notes

### `private bool PCandidateHandle(Key key)`

The dropdown never takes focus, so the caret's key handler drives it.
Nothing is selected while the list merely stands open, so enter still commits the typed wording.
The user reaches the list with the arrows, and only then does enter take a row.
Down from nothing selects the first row and up selects the last.

### `PCandidate.PlacementTarget = PCandidateFrameFind(box) ?? box ?? (UIElement)PContents;`

The frame inside the caret, because that is the edge the dropdown is read against.
The caret it is drawn for falls back in, then the pane.
The list still opens when the frame is not built yet.

### `private static CustomPopupPlacement[] PCandidatePlace(Size popup, Size target, Point offset)`

The dropdown is placed against the frame by hand, from the frame's own corner.
Placing it under the frame's edge instead left it short of that edge and over the caret.
The gutter the surface keeps for its shadow is taken back on both counts.
The list reads as the frame's own edge continued.
The second placement puts it above the frame, for a caret near the foot of the screen.

### `private void PCandidateShow(PCard card, string text)`

The wording is matched anywhere inside a stored title, ignoring case, because this is a search rather than a prefix.
So "the situation" finds "This is the situation".
Rows are ordered by how many cards already use them, so the settled wordings stand first.
A Situation the card already carries is left out, since it cannot be attached twice.
The list stays shut when nothing matches, rather than standing empty.
