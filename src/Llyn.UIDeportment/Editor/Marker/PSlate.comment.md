# PSlate.cs

## `public partial class PEditor`

The dropdown of tags the workspace already holds that a typed tag may already name, and the rows it offers.
It is one popup the editor owns rather than one per card, for the reason the situation dropdown is.
Only one card is being typed into at a time.
The popup is retargeted at the caret that opened it.

A row carries the stored Tag's id, so a pick links that Tag rather than a second spelling.
Two cards meaning the same tag should spell it the same way.
Without this the same idea would be filed under two spellings and the taxonomy panel would browse both.

The list opens as the tag is typed rather than only when it is committed.
A user cannot pick from a catalogue they were never shown.

## `private readonly PSlateTemplate _pSlateTemplate`

The offered tag dictionary, held so the editor merges it into its resources.

## `private void PSlateApply(FrameworkElement container, object item, string? _)`

Fills one offered tag row, where bindings and an event attribute stood, and subscribes its press.
The miss Refine is subscribed before the pick Observe, so each hears the press in its role.

## `private void PSlateAttach()`

Hands the slate list its rows and fill, gives the dropdown its placement, and clears its selection as it shuts.

## `private void PSlateMissRefine(object sender, MouseButtonEventArgs e)`

A press that reaches no offered row shuts the dropdown, with no gate involved.

## `private void PSlatePickObserve(object sender, MouseButtonEventArgs e)`

Hands the Tag the pointer chose to the card gate `CCardTagInsert`.
It lands at the caret of the card whose Tag entry has focus, as Enter on a row does.
The press is heard before focus moves, so that entry is still the one the list was opened for.
The list then shuts and the entry empties.
The clerk skips a Tag the card already carries, by id or by text.

## `private void PSlateRefine(PCard card, CSlate slate)`

Paints the tag gate's answer: the text the entry keeps, then the dropdown or its shutting.
The gate already trimmed, filtered, limited and split the rows, so the list shows them as they come.

## `private void PSlateOpenRefine(PCard card, IReadOnlyList<CSlateRow> rows)`

Fills the dropdown with the ready rows and opens it against the card's Tag entry, with no row selected.

## `private void PSlateShutRefine()`

Shuts the dropdown and forgets its rows.

## Inline notes

### `private void PSlateKeyRefine(object sender, KeyEventArgs e)`

The dropdown never takes focus, so the Tag entry's keys drive it while it stands open.
Escape shuts it, and the arrows walk its rows, with no gate involved.
Down from nothing selects the first row and up selects the last.

### `private void PSlateCloseRefine(object? sender, EventArgs e)`

A shut dropdown keeps no selected row, however it was shut.
So Enter reaches a stored tag only while the list stands open.

### `private void PSlateKeyObserve(object sender, KeyEventArgs e)`

Enter on a selected row hands that stored tag to the card gate at the caret.
The list then shuts and the entry empties.
Nothing is selected while the list merely stands open, so Enter still commits the typed tag.
The user reaches the list with the arrows, and only then does Enter take a row.

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
