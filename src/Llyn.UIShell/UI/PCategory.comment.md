# PCategory.cs

## `public partial class PEditor`

The parts of speech the chosen language declares, as a menu the editor opens.
The presets are what a language pack lists, in the order it lists them.
Picking one puts its name in the part-of-speech field.

The menu is an offer, not the field itself.
A pack cannot have thought of everything a user will want to write down.
So the field still takes anything typed, and this menu only fills it faster.

## Inline notes

### `internal void PCategoryLoad()`

Fills the menu with the presets the chosen language declares.
It runs whenever that language changes, because the parts of speech are the language's.
An entry moved from English to Japanese is offered Japanese's presets from that moment.

### `values = [];`

A vocabulary that cannot be read leaves the field editable and the menu empty.
The presets are a convenience, and losing them is not a reason to refuse the typing.

### `PCategoryNotice.Visibility = _pCategoryItem.Count == 0 ? Visibility.Visible : Visibility.Collapsed;`

A language whose pack declares no parts of speech says so.
It does not open on a blank sheet that looks like a menu still loading.

### `internal void PCategoryHandle(object sender, RoutedEventArgs e)`

A preset chosen from the menu, put in the field as the text it shows.
Nothing else is recorded.
The field is what the form reads.
A picked name and a typed name are the same value from here on.
