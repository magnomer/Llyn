# PNotationTemplate.xaml

## `<DataTemplate x:Key="Theme.Notation.Reading">`

One reading a source returned, as one button that takes it.
A tagged reading carries its variety in front of the transcription.
That is the flag when the language pack shows varieties as flags, with the name as tooltip.
It is the localized name when the pack shows them as text.
An untagged reading carries the transcription alone and offers no tooltip.
Its own style hides the name under a flag, since a template trigger cannot test for a present image.

## `<DataTemplate x:Key="Theme.Notation.Row">`

One source: its name, small, then what it had to say.
Its readings stand in a row of buttons beside the name, wrapping when they run out of width.
A source with no reading shows one italic line in place of them.
The line says whether it is still searching, has no entry, or could not be retrieved.
A source with two or more readings offers one more button that takes them all.
Hiding it otherwise keeps a row with one reading free of a choice that means nothing there.
