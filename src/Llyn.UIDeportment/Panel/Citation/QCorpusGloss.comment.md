# QCorpusGloss.cs

## `internal sealed partial class QCorpus`

The Gloss rows of the corpus panel: the ones the read area lists and the ones the edit area holds.
Every edit goes through the anthology's gloss gates, the card sentence's gates over the Example itself.

## `private void QExcerptGlossShow(IReadOnlyList<CGlossDraft> glosses)`

Lists every Gloss of the shown Example in the read area.
Hides the whole section when there is none, as a never-written field is not drawn.

## `private void QTranscriptGlossShow(CExample? example)`

Redraws the edit rows from the held Example, keeping the rows whose ids survive.

## `private PGloss QTranscriptGlossCreate(CGlossDraft draft)`

Builds one edit row and subscribes the row's pick notice to `QGlossSpeakerObserve`.

## `private void QGlossTextObserve(object sender, TextChangedEventArgs e)`

Hands the text typed into a Gloss field to `CAnthologyGlossSet`, which defers it keyed by the row.

## `private void QGlossSpeakerObserve(PGloss gloss, string language)`

Hands the language a row's list raised to `CAnthologyLanguageSet`, which sends it at once.
The row raises only a pick, since its own fill sets the list with its handlers off.

## `private void QGlossAddObserve(object sender, RoutedEventArgs e)`

Hands the place of the row whose plus was pressed, or the last place from the seed line, to `CAnthologyGlossAdd`.

## `private void QGlossRemoveObserve(object sender, RoutedEventArgs e)`

Hands the id of the Gloss whose minus was pressed to `CAnthologyGlossRemove`.

## `private void QTranscriptGlossApply(FrameworkElement container, object item, string? change)`

Fills one transcript Gloss row through `PGloss.PGlossRowApply` with the row's text handler off.
So a fill never reads as typing.
It subscribes the plus and minus clicks and gives them their icons, which their styles leave bare.

## `private void QTranscriptSeedShow()`

Shows the seed line only while the list is empty, so a first row can be added from somewhere.

## `private void QTranscriptSeedObserve(object sender, KeyboardFocusChangedEventArgs e)`

Focusing the seed field asks `CAnthologyGlossPrepare` for a first row.
Only a made row hands on to the focus Refine.

## `private void QTranscriptSeedRefine()`

Moves the caret into the first row once the row is drawn.
