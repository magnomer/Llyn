# PCategory.cs

## `public partial class PEditor`

The parts of speech offered under the part-of-speech field, as a menu the editor opens.
The presets start as what the chosen language pack lists, in the order it lists them.
Picking one puts its name in the field.

The menu is an offer, not the field itself.
A pack cannot have thought of everything a user will want to write down.
So the field still takes anything typed.
What is typed anew joins the menu, so the pack's list is a starting point rather than a ceiling.

The names are held apart from the rows shown.
The rows are what the field's text has narrowed the names to, rebuilt on every change.
That is what makes the field an editable dropdown rather than a box beside a list.

## Inline notes

### `internal void PCategoryLoad()`

Reads the presets the chosen language declares and makes them the menu's names.
It runs whenever that language changes, because the parts of speech are the language's.
An entry moved from English to Japanese is offered Japanese's presets from that moment.

### `values = [];`

A vocabulary that cannot be read leaves the field editable and the menu empty.
The presets are a convenience, and losing them is not a reason to refuse the typing.

### `private void PCategoryAdd(string name)`

A part of speech no preset names is declared as one, under the chosen language.
It is what makes the next entry in that language offer what this one taught it.
A name already held is left alone, so picking from the menu declares nothing new.
A write that fails leaves the chip standing, because the entry is what the user was writing.

### `private void PCategoryUpdate()`

The rows the menu shows, narrowed to the names the field's text appears in.
An empty field narrows nothing, so the whole list is offered.
A name already carried as a chip is marked rather than hidden.
Hiding it would make the menu shorten as the user works, and a taken name is worth seeing.

### `PCategoryAbsent.Visibility`

Two different silences are worth telling apart.
A language declaring no parts of speech is one, and typing that matches none is the other.
The first is the pack's, the second is the user's, and each says so in its own words.

### `internal void PCategoryHandle(object sender, RoutedEventArgs e)`

A preset chosen from the menu, put in the field as the text it shows.
Nothing else is recorded.
The field is what the form reads.
A picked name and a typed name are the same value from here on.
