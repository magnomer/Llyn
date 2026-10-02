# PCorpusTranscript.xaml
Hash: `0971b525a138f847`

## `ResourceDictionary`

The editing side of the Corpus panel holds the transcript fields, the Gloss row and the pickers under it.
The page merges it as it merges the excerpt, so it is loose and has no class of its own.
No event is named here, so the corpus driver subscribes its own handlers on each realized part.
The language choice and the citation row stand here because both open from the transcript.

## `<Style x:Key="Theme.Transcript.Sentence" TargetType="TextBox" ...>`

The transcript field at the size and weight the excerpt page prints the sentence in.
So the sentence does not move when the page turns from reading to writing.

## `<Style x:Key="Theme.Transcript.Speaker" TargetType="ToggleButton" ...>`

The toggle takes the height, margin and padding of the speech chip under the excerpt's language chip.
So the language stands in the same box when read and when chosen.

## `<Style x:Key="Theme.Transcript.Row" TargetType="Border">`

The Gloss row and the seed share this surface, transparent so the pointer finds it.
The look sheet reveals `PTranscriptShelf` while the row is under the pointer or holds focus.

## `<Style x:Key="Theme.Transcript.Control" TargetType="StackPanel">`

The Gloss row's buttons start invisible and take no pointer.
The look sheet raises both on `Theme.Transcript.Row` while the row is hovered or focused.
A hidden button that still took clicks would drop a Gloss the reader never saw.

## `<Style x:Key="Theme.Transcript.Addition" TargetType="Button" BasedOn="{StaticResource Theme.Card.Handle}">`

The style carries no icon, since a setter's element would be one instance shared by every button.
Each button holds its own bare icon, and the panel's fill gives it a source.

## `<DataTemplate x:Key="Theme.Transcript.Line">`

The editable Gloss row, shaped as `Theme.Gloss.Display` shapes the read row.
It is written here rather than taken from the card's row template.
The card dresses its Gloss in the card's own face.
The reading side dresses a Gloss in the display value face, so the editing side must too.

## `<Style x:Key="Theme.Transcript.Citation" TargetType="TextBox" BasedOn="{StaticResource Theme.Input.Bare}">`

The citation as a bare text field, so the cited Source reads where and as the reading side reads it.
The frame shows only under the pointer or focus, which is what says the text can be changed.

## `<DataTemplate x:Key="Theme.Citation.Row">`

One offered Source for the citation field, with its usage count against the right edge.
It answers the pointer going down, as the candidate row of the entry editor does.
The panel fills its named runs and subscribes the press on `PCitationRow`.
The popup takes the window's activation when pressed, and the press must land before that.

## `<DataTemplate x:Key="Theme.Language.Choice">`

One language in the speaker dropdown, its flag before its name.
The driver subscribes the click, since it knows which transcript is open.
The flag and name reuse the Gloss option part names, so the language item fill serves both.

## `<Image x:Name="PGlossFlag">`

The flag, the ring, the popup, the picker list and the text are named for `PGloss.PGlossRowApply`.
The fill gives the flag its source, the list its options and the text its value and placeholder.
The shared fill names the list's value path.
The driver's own fill wires `PGlossAddition`, `PGlossRemoval`, the text and the list's choice.
