# PMarker.cs

## `public partial class PEditor`

The parts of speech an entry carries, as the editor sets them.
That is the chips already taken and the field the next one is typed into.
The presets the field offers are opened beside it and live in `PCategory`.

The field is a text box rather than a chooser on purpose.
The presets are what a language pack declares.
A pack cannot have thought of everything a user will want to write down.
So the box takes anything, whether it was picked or typed.
A chip picked or declared as a preset carries that preset's row id.
A chip the engine could not declare carries only its text.
Whether typed text names a preset stays the engine's decision at the write.

## Inline notes

### `internal void PMarkerChipHandle(object sender, RoutedEventArgs e)`

A chip dropped is a name the menu may offer again unmarked.
So the menu is rebuilt with the chip gone.
The chips are sent at once, since dropping one is a whole action.

### `private void PMarkerFieldHandle(object sender, KeyEventArgs e)`

Enter is what turns typing into a chip.
Nothing else in the box commits, so a half-typed word stays editable.
Escape closes the menu and leaves the typing where it stands.
A user shutting the offer away has not asked to lose what they wrote.

### `private void PMarkerTextHandle(object sender, TextChangedEventArgs e)`

Typing narrows the menu and opens it on what is left.
An empty box or nothing matching closes it, because a menu with nothing to offer is in the way.
A form being filled from a draft is not typing, so it moves nothing.

### `private void PMarkerAdd(string name)`

Blank text and a name already carried both add no chip.
The box is cleared either way, so a repeated name does not sit there looking unread.
A name the presets do not hold is declared as one, so the menu offers it next time.
The chip takes the declared row's id and name, or the bare text when declaring failed.
The chips are sent at once, since adding one is a whole action.

### `private bool PMarkerMatch(IReadOnlyList<LSpeechDraft> speeches)`

Whether the chips already say what the draft says, name for name and id for id.
The render asks before rebuilding, because a rebuild clears the typing field.

### `private IReadOnlyList<LSpeechDraft> PMarkerRead()`

Each chip as the draft it stands for, linked by id where the chip knows one.
What the box still holds counts as carried.
A user who typed a last part of speech and stored without pressing Enter meant it.

### `private void PMarkerShow(IReadOnlyList<LSpeechDraft>? speeches)`

A null list is the empty form rather than an entry with no parts of speech.
Both end with no chips, and the difference is the caller's.
The menu is rebuilt after either, because what is marked has changed.
