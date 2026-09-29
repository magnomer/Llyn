# PMarker.cs

## `public partial class PEditor`

The parts of speech an entry carries, as the editor sets them.
That is the chips already taken and the field the next one is typed into.
The presets the field offers are opened beside it and live in `PCategory`.

The field is a text box rather than a chooser on purpose.
The presets are what a language pack declares.
A pack cannot have thought of everything a user will want to write down.
So the box takes anything, whether it was picked or typed.
The chips and the pending text are Conduct's `CCardSpeech`, and this file only paints and hears them.

## `private readonly PMarkerTemplate _pMarkerTemplate`

The marker dictionary, held so it merges into this control's resources.

## `private void PMarkerApply(FrameworkElement container, object item, string? _)`

Fills one marker chip: its name, its close icon and its close click.

## `private void PMarkerAttach()`

Hands the chip list its chips and fill, and subscribes the marker field's keys and text.
The chip list is held to the width of its row, where a binding followed that width.
The switch icon is set here, where the markup held an icon lookup.

## `private void PMarkerEraseObserve(object sender, RoutedEventArgs e)`

Hands the erased chip's name to the gate, and the draft's change repaints the chips.

## `private void PMarkerCloseRefine(object sender, KeyEventArgs e)`

Escape closes the menu and leaves the typing where it stands.
A user shutting the offer away has not asked to lose what they wrote.

## `private void PMarkerCommitObserve(object sender, KeyEventArgs e)`

Enter is what turns typing into a chip, and nothing else in the box commits.
The box is cleared after the gate either way, so a repeated name does not sit there looking unread.

## `private void PMarkerTextObserve(object sender, TextChangedEventArgs e)`

Hands the raw text to the gate, which keeps it pending in the draft.
The gate answers whether the text names a part of speech, so the menu knows to open.

## `private void PMarkerDropperRefine(bool offered)`

Typing narrows the menu and opens it on what is left.
An empty box or nothing matching closes it, because a menu with nothing to offer is in the way.

## `private bool PMarkerFind(string name)`

Whether a chip already carries the name, so the menu marks it taken.
It stays for `PCategoryUpdate`, whose own row moves the check down.

## `internal void PMarkerRefine(CEntryDraft _)`

Paints the chips and the pending text Conduct answers for the held draft.
The menu is rebuilt after, because what is marked may have changed.
