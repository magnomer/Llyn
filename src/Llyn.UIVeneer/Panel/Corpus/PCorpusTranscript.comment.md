# PCorpusTranscript.xaml

## `ResourceDictionary`

The editing side of the Corpus panel: the transcript fields, the Gloss row and the pickers under it.
Three of its templates answer clicks the panel handles, so it carries a class and forwards to its host.
That is the shape every editor template dictionary takes, so a merged template still reaches the panel.
The panel adds it from code after its own markup is parsed, so the markup reads the keys dynamically.
The language choice and the citation row stand here because both open from the transcript.

## `<DataTemplate x:Key="Theme.Transcript.Line">`

The editable Gloss row, shaped as `Theme.Gloss.Display` shapes the read row.
It is written here rather than taken from the card's row template.
The card dresses its Gloss in the card's own face.
The reading side dresses a Gloss in the display value face, so the editing side must too.

## `<Style x:Key="Theme.Transcript.Citation" TargetType="ToggleButton">`

The citation as a bare toggle, so the cited Source reads where and as the reading side reads it.
The chip edge shows only under the pointer, which is what says the text can be changed.

## `<DataTemplate x:Key="Theme.Citation.Row">`

One offered Source for the citation field, with its usage count against the right edge.
It answers the pointer going down, as the candidate row of the entry editor does.
The popup takes the window's activation when pressed, and the press must land before that.

## `<DataTemplate x:Key="Theme.Language.Choice">`

One language in the speaker dropdown, its flag before its name.
The click is forwarded to the panel, which knows which transcript is open.
