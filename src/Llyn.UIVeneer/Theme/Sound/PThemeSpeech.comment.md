# PThemeSpeech.xaml
Hash: `5f5d5a5e9c0afb20`

## `<Style x:Key="Theme.Speech.Chip" TargetType="Border">`

One part of speech as it reads.
It matches the editor's marker chip apart from the removal button that chip carries.
The parts of speech take a row of their own, below the pronunciation rather than beside it.

## `<Style x:Key="Theme.Unit.Name" TargetType="TextBlock">`

The lexical unit as it reads, a muted line above the speech chips.
It is not a chip, because it names a kind of piece, not one of the piece's roles.
Its right margin parts it from the freedom that follows on the same line.

## `<Style x:Key="Theme.Freedom.Name" TargetType="TextBlock" BasedOn="{StaticResource Theme.Unit.Name}" />`

The freedom as it reads, written as the unit is, right after it.
