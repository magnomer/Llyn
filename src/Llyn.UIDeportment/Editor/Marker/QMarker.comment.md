# QMarker.cs
Hash: `0efa7979ab7d01af`

## `internal sealed class QMarker`

The parts of speech an entry carries, as the editor sets them.
That is the chips already taken and the field the next one is typed into.
The presets the field offers are opened beside it and live in `QCategory`.

The field is a text box rather than a chooser on purpose.
The presets are what a language pack declares.
A pack cannot have thought of everything a user will want to write down.
So the box takes anything, whether it was picked or typed.
The chips and the pending text are Conduct's `CCardSpeech`, and this file only paints and hears them.

## `internal QMarker(FrameworkElement surface)`

Hands the chip list its chips and fill, and subscribes the marker field's keys and text.
The chip list is held to the width of its row, where a binding followed that width.
The switch icon is set here, where the markup held an icon lookup.

## `internal void QMarkerIntroduce(CCardSpeech speech, CEntry entry, QCategory category, QUnit unit)`

Holds the speech facet, the category driver and the unit driver, and subscribes the marker Refine to the entry draft.
Each draft change hands the unit driver the units read with the field, so the row is read once.

## `private void QMarkerApply(FrameworkElement container, object item, string? _)`

Fills one marker chip with its name, its close icon and its close click.

## `private void QMarkerEraseObserve(object sender, RoutedEventArgs e)`

Hands the erased chip's name to the gate, and the draft's change repaints the chips.

## `private void QMarkerCloseRefine(object sender, KeyEventArgs e)`

Escape closes the menu and leaves the typing where it stands.
A user shutting the offer away has not asked to lose what they wrote.

## `private void QMarkerCommitObserve(object sender, KeyEventArgs e)`

Enter is what turns typing into a chip, and nothing else in the box commits.
The box is cleared after the gate either way, so a repeated name does not sit there looking unread.

## `private void QMarkerTextObserve(object sender, TextChangedEventArgs e)`

Hands the raw text to the gate, which keeps it pending in the draft.
The gate answers the category menu for the text, and the dropper paints it.

## `private void QMarkerDropperRefine(CCategory category)`

Typing narrows the menu and opens it when the engine says the menu shows.
An empty box or nothing matching closes it, because a menu with nothing to offer is in the way.

## `internal void QMarkerFieldRefine()`

Empties the box after a commit.
The category menu calls it too, so a picked name leaves no typed text behind.

## `private void QMarkerRefine(CEntryDraft _)`

Paints the chips and the pending text Conduct answers for the held draft.
The menu the same read carries is painted after, because what is marked may have changed.
