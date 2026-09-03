# PSpeech.cs

## `public partial class PEditor`

The parts of speech an entry carries, as the editor sets them.
That is the chips already taken and the field the next one is typed into.
The presets a language offers are opened beside it and live in `PCategory`.

The field is a text box rather than a chooser on purpose.
The presets are what a language pack declares.
A pack cannot have thought of everything a user will want to write down.
So the box takes anything, whether it was picked or typed.
Text naming a preset is stored as that preset's stable id.
Text naming none is stored as typed.
Which of the two it is stays the engine's decision at the write.

## Inline notes

### `private void PSpeechContentsHandle(object sender, KeyEventArgs e)`

Enter is what turns typing into a chip.
Nothing else in the box commits, so a half-typed word stays editable.

### `private void PSpeechAdd(string name)`

Blank text and a name already carried both add nothing.
The box is cleared either way, so a repeated name does not sit there looking unread.

### `private IReadOnlyList<string> PSpeechRead()`

What the box still holds counts as carried.
A user who typed a last part of speech and stored without pressing Enter meant it.

### `private void PSpeechShow(IReadOnlyList<string>? names)`

A null list is the empty form rather than an entry with no parts of speech.
Both end with no chips, and the difference is the caller's.
