# Marker

Marker holds the part-of-speech row of the editor.
It has the chips already taken and the field the next one is typed into.
The unit and freedom droppers sit at its head.
The tag dropdown `QSlate` lives here too, since it shares the dropdown shape.

## `PMarkerChip`

This is one part-of-speech chip under the field.
It is immutable, because chips are added and removed rather than edited.
Conduct keeps the value each chip stands for, so the chip carries only the name shown.

## `PSlateItem`

This is one stored tag offered in the dropdown, with its id and its text in three pieces.
Choosing a row links that Tag by id, so a rename in the workspace follows the card.
It copies the ready row the tag gate answered and computes nothing.
The tag clerk split the text, so the dropdown and a console mark the same piece.
The row draws the middle piece in weight, so a reader sees why the row is offered.

## `QFreedom`

This class drives the freedom dropper that follows the unit dropper.
The dropper names the held freedom, or the field's title while none is held.
Every name is a key Conduct chose, so the driver only looks it up.
`QMarker` calls its refine on every draft change.
Picking the held freedom clears it, a rule the tenure keeps.

## `QFreedomItem`

This is one row of the freedom menu, immutable because the menu is rebuilt rather than edited.
Its key both draws the row and goes back to the gate.
It also carries whether the draft holds the freedom, which draws a check.

## `QMarker`

This class paints and hears the chips and the pending text, which are Conduct's `CCardSpeech`.
The presets the field offers live in `QCategory`.
The field is a text box rather than a chooser on purpose.
A pack cannot have thought of everything a user will want to write down.
So the box takes anything, whether it was picked or typed.
The chip list is held to the width of its row, where a binding followed that width.

### `QMarkerIntroduce`

Each draft change hands the unit and freedom drivers their rows read with the field.
So the row is read once.

### `QMarkerCloseRefine`

Escape closes the menu and leaves the typing where it stands.
A user shutting the offer away has not asked to lose what they wrote.

### `QMarkerCommitObserve`

Enter is what turns typing into a chip, and nothing else in the box commits.
The box is cleared after the gate either way, so a repeated name does not sit there looking unread.

### `QMarkerDropperRefine`

An empty box or nothing matching closes the menu, because a menu with nothing to offer is in the way.

### `QMarkerFieldRefine`

The category menu calls it too, so a picked name leaves no typed text behind.

### `QMarkerRefine`

The menu is painted after the chips, because what is marked may have changed.

## `QSlate`

This is the dropdown of stored tags a typed tag may already name.
It is one popup the editor owns rather than one per card.
Only one card is typed into at a time.
The popup is retargeted at the caret that opened it.
A row carries the stored Tag's id, so a pick links that Tag rather than a second spelling.
Without this the same idea would be filed under two spellings and the taxonomy panel would browse both.
The list opens as the tag is typed, because a user cannot pick from a catalogue they were never shown.

### `QSlateApply`

The miss Refine is subscribed before the pick Observe, so each hears the press in its role.

### `QSlatePickObserve`

The Tag lands at the caret of the card whose Tag entry has focus, as Enter on a row does.
The caret gate places it from its own trail, so no position is read here.
The press is heard before focus moves, so that entry is still the one the list was opened for.
The clerk skips a Tag the card already carries, by id or by text.

### `QSlateRefine`

The gate already trimmed, filtered, limited and split the rows, so the list shows them as they come.

### Inline notes

### QSlate.cs: `internal void QSlateKeyRefine(object sender, KeyEventArgs e)`

The dropdown never takes focus, so the Tag entry's keys drive it.
The arrows ask `CLanternMove` which row to light.
A shut dropdown holds no rows, so the gate answers null and the arrow stays unhandled.
Escape shuts the dropdown only while it stands open.

### QSlate.cs: `private void QSlateCloseRefine(object? sender, EventArgs e)`

A shut dropdown keeps no selected row and no rows, however it was shut.
So Enter reaches a stored tag only while the list stands open.

### QSlate.cs: `internal void QSlateKeyObserve(object sender, KeyEventArgs e)`

Nothing is selected while the list merely stands open, so Enter still commits the typed tag.
The user reaches the list with the arrows, and only then does Enter take a row.

### QSlate.cs: `QSlatePopup.PlacementTarget = QSlateFrameFind(box) ?? box ?? (UIElement)QSlateContents`

The frame inside the caret is the edge the dropdown is read against.
The caret falls back in, then the pane, so the list still opens when the frame is not built yet.
The sheet's minimum width is bound to that target's width, so it follows a resize while open.

### QSlate.cs: `private static CustomPopupPlacement[] QSlatePlace(Size popup, Size target, Point offset)`

The dropdown is placed by hand from the frame's own corner.
Placing it under the frame's edge left it short of that edge and over the caret.
The gutter the surface keeps for its shadow is taken back on both counts.
The second placement puts it above the frame, for a caret near the foot of the screen.

## `QUnit`

This class drives the lexical unit dropper at the head of the row.
It follows `QFreedom` in everything, with the title key shown while no unit is held.

## `QUnitItem`

This is one row of the unit menu.
It follows `QFreedomItem` in everything.

## Stamp

Commit: `c6e60141a172`
- `PMarkerChip.cs` `a140997f5eb7a142`
- `PSlateItem.cs` `994494d60631fe1c`
- `QFreedom.cs` `5a8c8788e06e7771`
- `QFreedomItem.cs` `27469f2726afbac9`
- `QMarker.cs` `ea44857a68abf2d6`
- `QSlate.cs` `2387b8e70f5eec37`
- `QUnit.cs` `65408138208702ba`
- `QUnitItem.cs` `47f0e19bc6379784`
