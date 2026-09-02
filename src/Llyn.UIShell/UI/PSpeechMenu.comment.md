# PSpeechMenu.cs

## `public partial class PEditor`

The part of speech an entry carries, as the editor sets it.
That is the editable field and the label beside it that says what it now holds.
The dropdown of presets the chosen language declares belongs here too.

The field is a text box rather than a chooser on purpose.
The presets are what a language pack declares.
A pack cannot have thought of everything a user will want to write down.
So the dropdown fills the box, and the box still takes anything.
What was typed is stored either way.
Text naming a preset is stored as that preset's stable id.
Text naming none is stored as typed.
Which of the two it is stays the engine's decision at the write.

The label is bound to the box in markup, so it follows the typing without passing through here.
This file only fills the dropdown for the language in force and puts a chosen preset in the box.

## Inline notes

### `internal void PSpeechLoad()`

Fills the dropdown with the presets the chosen language declares, in the order its pack lists them.
It runs whenever that language changes, because the parts of speech are the language's.
An entry moved from English to Japanese is offered Japanese's presets from that moment.

### `values = [];`

A vocabulary that cannot be read leaves the field editable and the dropdown empty.
The presets are a convenience, and losing them is not a reason to refuse the typing.

### `PSpeechMenuNotice.Visibility = _pSpeechItem.Count == 0 ? Visibility.Visible : Visibility.Collapsed;`

A language whose pack declares no parts of speech says so.
It does not open on a blank sheet that looks like a menu still loading.

### `internal void PSpeechHandle(object sender, RoutedEventArgs e)`

A preset chosen from the dropdown, put in the field as the text it shows.
Nothing else is recorded.
The field is what the form reads.
A picked name and a typed name are the same value from here on.
