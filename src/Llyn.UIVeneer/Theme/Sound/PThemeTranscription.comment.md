# PThemeTranscription.xaml

The look of a transcription row, shared by the editor and the reading view.
The pronunciation and accent styles are merged, so a row reads as a further pronunciation row does.
A transcription is a spelling rather than a sound, so it wears no brackets and has no audio.

## `<Style x:Key="Theme.Transcription.Phonetician" TargetType="Button">`

The lookup button of a row, whose notation command and icon are `PLook` rows.
It stands where the play button would, so the row's field ends where a pronunciation field ends.

## `<Style x:Key="Theme.Transcription.Option" TargetType="ComboBoxItem">`

One scheme in the dropdown, drawn as a choice row is.
`LTranscriptionChoice.LTranscriptionChoiceApply` disables a scheme another row already holds, so the engine is never asked twice.
Its highlight, padding and greyed ink are `PLook` rows.

## `<Style x:Key="Theme.Transcription.Scheme" TargetType="ComboBox">`

The scheme dropdown of an editor row, standing where the reading view draws the scheme chip.
Closed, it reads as the chip does, with a small chevron after the name and the accent on hover.
Open, it lists every scheme the pack declares.
The toggle, popup, name ink and chevron are `PLook` rows.
`PEditor.PTranscriptionApply` fills its items and value, and writes a pick back to the row.
The fill also sets the label and scheme paths, so the style holds no member path.

## `<Style x:Key="Theme.Transcription.Addition" TargetType="Button">`

The plus that adds a row in the next free scheme after this one.
It is the accent plus with the transcription command as a `PLook` row, disabled when no scheme is left.

## `<Style x:Key="Theme.Transcription.Remove" TargetType="Button">`

The minus that drops a row, with the transcription removal command as a `PLook` row.

## `<DataTemplate x:Key="Theme.Transcription.Row">`

A row as the editor draws it: scheme dropdown, field, lookup, and the plus and minus pair.
`PEditor.PTranscriptionApply` fills the named parts and writes typing back to the row.

## `<DataTemplate x:Key="Theme.Transcription.Display">`

A row as the reading view draws it: scheme chip and text, nothing to type into.
`LTranscriptionItem.LTranscriptionItemApply` fills its named parts.
