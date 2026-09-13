# PThemePronunciation.xaml

## `<Style x:Key="Theme.Pronunciation.Surface" TargetType="Border">`

The row a pronunciation is read and written in.
It carries no border and no fill, so the brackets stand alone against the page.
Its height is fixed to the playback group beside it, so the two sit on one line.
The reading view wears the same height, so a reading stands where it stands in the editor, row for row.
Without a box the reading has to hold the eye on its own.
It is set larger than the buttons around it.

## `<Style x:Key="Theme.Pronunciation.Lead" TargetType="StackPanel">`

The flag and name, or the scheme, that a row leads with.
Every row puts it in a grid column of the shared size group `PReadingLabel`.
The editor and the reading view each open one such scope over their pronunciation and transcription rows.
So every row's reading starts at one x, whatever the row leads with, in any language and any mix.

## `<Style x:Key="Theme.Pronunciation.Bracket" TargetType="TextBlock">`

The two brackets that hold the reading.
They are set in the muted ink, so they frame the reading without competing with it.

## `<Style x:Key="Theme.Pronunciation.Flag" TargetType="Image">`

The variety flag before the brackets, hidden while no flag is set.
It is the size the lookup menu draws a flag at, so a taken reading keeps its flag's size.

## `<Style x:Key="Theme.Pronunciation.Label" TargetType="TextBlock">`

The variety name before the brackets, in the small muted face, hidden while blank.
It stands in where the pack draws no flags or none resolves.

## `<Style x:Key="Theme.Pronunciation.Text" TargetType="TextBlock">`

The pronunciation as it reads.
It takes exactly the width its text needs.
It is set in the phonetic face, as the field and the measure are.
So the IPA stays whole whatever face the headword's language pack chose.
A ceiling keeps a long reading from pushing the row wide.

## `<Style x:Key="Theme.Pronunciation.Measure" TargetType="TextBlock">`

An unseen twin of the field, carrying the same text in the same face.
The field is sized by this twin, so the box grows and shrinks with what is typed.
When nothing is typed the twin carries the placeholder instead.
The empty field is still wide enough to read it.

## `<Style x:Key="Theme.Pronunciation.Field" TargetType="TextBox">`

The same pronunciation with a caret in it.
It holds no floor width and fills the width its unseen twin measures.
It carries the read size and is pulled a pixel left.
A WPF text box keeps that pixel for the caret.
So a reading sits in the same place whether it is being read or being typed.
