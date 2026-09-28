# QCorpusEditor.cs

## `internal sealed partial class QCorpus`

Editing behavior of the Corpus panel.
An Example is quoted by any number of cards, so rewriting it here rewrites what every one of them quotes.
The panel offers no way to fork an Example while editing it.
A sentence meant for one card alone is a new Example on that card.
What the controls hold is handed to the desk's text gate, which builds each request.
The driver keeps no copy of a stored Example, and asks the engine what counts as a change.

## `private void QTranscriptAttach()`

Subscribes the sentence field's typing handler.

## `private void QTranscriptDetach()`

Unsubscribes the typing handler, so a fill from the held sentence writes nothing back.
Filling a field raises the same change the user typing raises, and only typing is a request.

## `private void QSpeakerLoad()`

Reads the workspace languages the picker offers.
No default is chosen, because an Example's language is optional and the store accepts none.

## `private void QSpeakerHandle(object sender, RoutedEventArgs e)`

The picked language goes to the text gate, and the chip changes when the draft bulletin returns.

## `private void QSpeakerApply(FrameworkElement container, object item, string? change)`

Fills one speaker choice through the language item fill, then subscribes its click.

## `private void QTranscriptTextHandle(object sender, TextChangedEventArgs e)`

Typing clears the unknown mark and hands the text to the text gate, which defers it.

## `private static string QTranscriptHintRead(bool unknown)`

The placeholder the sentence field shows while empty.
A never-written sentence asks for a sentence, and an unknown one reads the unknown mark until typing clears it.

## `private void QTranscriptApply(CExample? example)`

Fills every field from the held sentence, or empties them when no draft stands.
The chip line is redrawn from the sentence's Mentions along with the fields.
The language is taken as the sentence carries it, an empty one included.
The tally chip follows the stored id the draft names, not the sentence being written.

## `private void QTranscriptShow(CExample? example)`

Redraws each field from the held sentence only where the field says something else.
Whether the text differs is the `CAnthology.CAnthologyTextCheck` verdict, so a bulletin never moves the caret.
The language and the citation are compared the same way, so a bulletin that changed nothing redraws nothing.

## `private void QCorpusFreshHandle(object sender, RoutedEventArgs e)`

New makes whatever the emptier panel would list, and the gate `CCorpusExampleCreate` decides which.
