# QCorpusEditor.cs

## `internal sealed partial class QCorpus`

Editing behavior of the Corpus panel.
An Example is quoted by any number of cards, so rewriting it here rewrites what every one of them quotes.
The panel offers no way to fork an Example while editing it.
A sentence meant for one card alone is a new Example on that card.
What the controls hold is handed raw to the gates on `CAnthology`, and ShellEngine builds each request.
The driver keeps no copy of a stored Example, and asks the engine what counts as a change.

## `private string _qTranscriptCitation`

The citation line the field last showed, as the Example carried it ready.
It is paint memory for that one control, never a decision about data.
Escape, leaving the field and a pick or commit redraw the field from it, so they read nothing.
A bulletin repaints the field only when the carried line differs from it, so typing there survives unrelated edits.

## `private void QTranscriptAttach()`

Subscribes the sentence field's typing Observe.

## `private void QTranscriptDetach()`

Unsubscribes the typing handler, so a fill from the held sentence writes nothing back.
Filling a field raises the same change the user typing raises, and only typing is a request.

## `private void QSpeakerRefine(IReadOnlyList<string> languages)`

Fills the speaker menu with the given workspace languages.
The chip is not redrawn, since a fresh workspace holds no draft.
No default is chosen, because an Example's language is optional and the store accepts none.

## `private void QSpeakerObserve(object sender, RoutedEventArgs e)`

Hands the picked language to `CAnthologySpeakerSet`, then closes the dropper.
The chip changes when the draft bulletin returns.

## `private void QSpeakerDropperRefine()`

Closes the speaker dropper after a pick.

## `private void QSpeakerApply(FrameworkElement container, object item, string? change)`

Fills one speaker choice through the language item fill, then subscribes its click.

## `private void QSpeakerShow(string language)`

Paints the speaker chip's name and flag from the language the Example carries.

## `private void QTranscriptTextObserve(object sender, TextChangedEventArgs e)`

Hands the typed text to `CAnthologyTextSet`, which defers it, and paints the placeholder key it answers.
Typing so clears the unknown mark.

## `private void QTranscriptHintRefine(string hint)`

Paints the sentence field's placeholder from the key Conduct chose.

## `private void QTranscriptRefine(CExample example)`

Fills every field from the held sentence, which is the blank Example when no draft stands.
The chip line is redrawn from the sentence's Mentions along with the fields.
The language is taken as the sentence carries it, an empty one included.
The tally chip paints the sentence the Example carries, the chosen Example's count.

## `private void QTranscriptDraftRefine(CExample example)`

Redraws each field from the held sentence only where the field says something else.
Whether the text differs is the `CAnthology.CAnthologyTextCheck` verdict, so a bulletin never moves the caret.
The speaker chip is painted on every bulletin, since painting the same language changes nothing.
The citation field is repainted only when its carried line changed, as `_qTranscriptCitation` states.

## `private void QCorpusFreshObserve(object sender, RoutedEventArgs e)`

New makes whatever the emptier panel would list, and the gate `CCorpusExampleCreate` decides which.
