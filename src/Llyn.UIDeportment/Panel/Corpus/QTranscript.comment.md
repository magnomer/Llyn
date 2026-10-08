# QTranscript.cs
Hash: `045f9f0e592d2d31`

## `internal sealed class QTranscript`

The sentence field of the Corpus panel's edit area, with its placeholder and its tally chip.
What the field holds is handed raw to `CAnthologyTextSet`.
The desk on the corpus Conduct holds the tenure from start to commit or cancel.
It subscribes what it paints itself, so the owner `QCorpus` only builds, introduces and lights it.

## `internal QTranscript(UserControl scope)`

Takes the Corpus page, and finds the transcript parts in it by contract ID.
It subscribes the sentence field's typing.

## `internal void QTranscriptDeskIntroduce(CCorpus corpus)`

`QCorpusIntroduce` calls it once the corpus Conduct exists.
It subscribes the transcript notice and the transcript's draft notice, which redraw the field.
The desk shows its own failures.
It subscribes the anthology's rows event, which redraws the tally chip.

## `internal void QTranscriptVisibleRefine()`

Shows the edit area while the diptych edits its parent.
The field is live while the corpus says so, through `CCorpusTranscriptEnabled`.
The owner's mode update calls it with the rest of the panel's mode.

## `private void QTranscriptAttach()`

Subscribes the sentence field's typing Observe.

## `private void QTranscriptTeardown()`

Unsubscribes the typing handler, so a fill from the held sentence writes nothing back.
Filling a field raises the same change the user typing raises, and only typing is a request.

## `private void QTranscriptTextObserve(object sender, TextChangedEventArgs e)`

Hands the typed text to `CAnthologyTextSet`, and paints the placeholder key it answers.

## `private void QTranscriptHintRefine(string hint)`

Paints the sentence field's placeholder from the key Conduct chose.

## `private void QTranscriptRefine(CExample example)`

Fills the field from the held sentence, which is the blank Example when no draft stands.
The tally chip shows the tally the Example carries.

## `private void QTranscriptDraftRefine(CExample example)`

Redraws the field from the held sentence only where it says something else.
Whether the text differs is the Example's `CExampleTextKept` verdict from Conduct, so a bulletin never moves the caret.

## `private void QTranscriptTallyRefine()`

Rewrites the tally chip from `CApertureTallyRead` when the catalog's rows change.
A quotation added elsewhere refills the catalog, so its count shows at once.
