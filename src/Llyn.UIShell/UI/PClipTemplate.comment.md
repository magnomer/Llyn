# PClipTemplate.xaml

## `<DataTemplate x:Key="Theme.Clip.Reading">`

One recording a source returned, as its flag or name, a play button, and a taking button.
A tagged recording carries its variety in front of the buttons.
That is the flag when the language pack shows varieties as flags, with the name as tooltip.
It is the localized name when the pack shows them as text.
An untagged recording carries the two buttons alone.
Its own style hides the name under a flag, since a template trigger cannot test for a present image.
A recording itself has nothing to read, so nothing is written about it, it is played.
The taking button carries this recording's own download state, so a later arrival cannot overwrite it.
Each button lights on its own hover alone, since a row holds several and none is the row's choice.

## `<DataTemplate x:Key="Theme.Clip.Row">`

One source, its name set small in the fixed column the pronunciation menu gives it.
The two menus line up.
Its recordings stand in a row beside the name, wrapping when they run out of width.
A source that offered nothing shows one italic line in place of them.
Hiding them rather than disabling them keeps a row that cannot act from looking like one that is merely busy.
