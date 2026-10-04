# QGloss.cs
Hash: `4ec560ef56146fba`

## `internal sealed class QGloss`

The Gloss gestures of a card's sentence row, each handed raw to one sentence gate.
The sentence menu wires the add button and the remove command to it.
The editor routes typed Gloss text here.

## `internal QGloss(ObservableCollection<PCard> meaning, ObservableCollection<PCard> collocation, QSentence sentence)`

Holds the two card lists the editor keeps, which name the row a Gloss sits under.
It holds the sentence driver, which finds the card of a pressed sentence row.
It has no control of its own to wire.

## `internal void QGlossIntroduce(CEditor editor)`

Holds the Conduct editor whose sentence gates the handlers call.

## `internal void QGlossAddObserve(object sender, RoutedEventArgs e)`

Hands the pressed sentence row to the gloss add gate.
The engine chooses the new Gloss's language and its place.

## `internal void QGlossRemoveObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the Gloss the command carries, and the row it was raised on, to the gloss remove gate.

## `internal void QGlossTextObserve(PGloss gloss, string text)`

Hands the text typed into a Gloss field to the sentence's gloss gate, with the row it stands in.

## `private (PCard, PSentence)? QGlossSentenceFind(PGloss gloss)`

The card and sentence row a Gloss row sits under, which every gate about the Gloss names.
