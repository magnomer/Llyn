# PNotationTemplate.xaml

## `<DataTemplate x:Key="Theme.Notation.Reading">`

One reading a source returned, as one button that takes it.
A tagged reading carries its variety in front of the transcription.
That is the flag when the language pack shows varieties as flags, with the name as tooltip.
It is the localized name when the pack shows them as text.
An untagged reading carries the transcription alone and offers no tooltip.
The name shows only when there is no flag and the name is not empty.
The brackets and the text are one string, blank brackets for a reading that is not bracketed.
`PNotationReading.PNotationReadingApply` sets the parts, the tooltip and the column's shared size group.
The transcription is set in the phonetic face, so no language font is asked for a combining mark it lacks.
A transcription reading is such a one, because a spelling in Pinyin or Jyutping is not IPA.

## `<DataTemplate x:Key="Theme.Notation.Row">`

One source: its name, small, then what it had to say.
Its readings stand in a row of buttons beside the name, wrapping when they run out of width.
A source with no reading shows one italic line in place of them.
The line says whether it is still searching, has no entry, or could not be retrieved.
`PNotationItem.PNotationItemApply` fills the row and switches the line against the readings.
