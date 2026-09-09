# PThemePronunciation.xaml

## `<Style x:Key="Theme.Pronunciation.Surface" TargetType="Border">`

The row a pronunciation is read and written in.
It carries no border and no fill, so the brackets stand alone against the page.
Its height matches the playback group beside it, so the two sit on one line.
Without a box the reading has to hold the eye on its own.
It is set larger than the buttons around it.

## `<Style x:Key="Theme.Pronunciation.Bracket" TargetType="TextBlock">`

The two brackets that hold the reading.
They are set in the muted ink, so they frame the reading without competing with it.

## `<Style x:Key="Theme.Pronunciation.Text" TargetType="TextBlock">`

The pronunciation as it reads.
It takes exactly the width its text needs.
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
