# PThemeTranscription.xaml

The look of a transcription row, shared by the editor and the reading view.
The pronunciation and accent styles are merged, so a row reads as a further pronunciation row does.
A transcription is a spelling rather than a sound, so it wears no brackets and has no audio.

## `<Style x:Key="Theme.Transcription.Phonetician" TargetType="Button">`

The lookup button of a row, raising the notation command so the menu opens for that row in its scheme.
It stands where the play button would, so the row's field ends where a pronunciation field ends.

## `<Style x:Key="Theme.Transcription.Label" TargetType="TextBlock">`

The scheme name of a row, drawn as the variety name of a pronunciation row is drawn.

## `<Style x:Key="Theme.Transcription.Option" TargetType="ComboBoxItem">`

One scheme in the dropdown, drawn as a choice row is.
A scheme another row already holds is greyed out and cannot be picked, so the engine is never asked twice.

## `<Style x:Key="Theme.Transcription.Scheme" TargetType="ComboBox">`

The scheme dropdown of an editor row, standing where the reading view draws the scheme chip.
Closed, it reads as the chip does, with a small chevron after the name and the accent on hover.
Open, it lists every scheme the pack declares, taken from the row's own choice list.
Its value is the row's scheme, so a pick reaches the row model, which the editor listens to.

## `<Style x:Key="Theme.Transcription.Measure" TargetType="TextBlock">`

The unseen twin of a row's field, sized by the row's text or by the placeholder when it is blank.

## `<Style x:Key="Theme.Transcription.Addition" TargetType="Button">`

The plus that adds a row in the next free scheme after this one, raising the addition command.
It is the accent plus with the transcription command, so it is disabled when no scheme is left.

## `<Style x:Key="Theme.Transcription.Remove" TargetType="Button">`

The minus that drops a row, raising the removal command with the row as its parameter.

## `<DataTemplate x:Key="Theme.Transcription.Row">`

A row as the editor draws it: scheme dropdown, field, lookup, and the plus and minus pair.
The field is bound to the row so typing reaches the row model, which the editor listens to.

## `<DataTemplate x:Key="Theme.Transcription.Display">`

A row as the reading view draws it: scheme chip and text, nothing to type into.
