# PCorpusTranscript.xaml

## `ResourceDictionary`

The editing side of the Corpus panel: the transcript fields, the Gloss row and the pickers under it.
Its class `PCorpusTranscript` stays in Deportment and merges this markup into itself.
No event is named here, so the panel subscribes the class's forwarding handlers on each realized part.
That is the shape every template dictionary with code takes from now on.
The panel adds it from code after its own markup is parsed, so the markup reads the keys dynamically.
The language choice and the citation row stand here because both open from the transcript.

## `<Style x:Key="Theme.Transcript.Row" TargetType="Border">`

The Gloss row and the seed share this surface, transparent so the pointer finds it.
The look sheet reveals `PTranscriptShelf` while the row is under the pointer or holds focus.

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
The click is forwarded to the panel, which knows which transcript is open.
The flag and name reuse the Gloss option part names, so the language item fill serves both.

## `<Image x:Name="PGlossFlag">`

The flag, the ring, the popup, the picker list and the text are named for `PGloss.PGlossRowApply`.
The fill gives the flag its source, the list its options and the text its value and placeholder.
The shared fill names the list's value path, and the panel's own fill wires `PGlossAddition` and `PGlossRemoval`.
