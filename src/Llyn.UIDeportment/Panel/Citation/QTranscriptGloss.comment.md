# QTranscriptGloss.cs
Hash: `721f6bb7610c05b6`

## `internal sealed class QTranscriptGloss`

The Gloss rows of the Corpus panel's edit area, while `QExcerpt` lists the read ones.
Every edit goes through the anthology's gloss gates, the card sentence's gates over the Example itself.
It subscribes what it paints itself, so the owner `QCorpus` only builds and introduces it.

## `internal QTranscriptGloss(UserControl scope)`

Takes the Corpus page, and finds the Gloss parts in it by contract ID.
It gives the plus its icon and the seed field its placeholder, which their styles leave bare.
It subscribes the seed field's focus and the plus click.

## `internal void QTranscriptGlossIntroduce(CCorpus corpus, ObservableCollection<PLanguageItem> language)`

`QCorpusIntroduce` calls it once the corpus Conduct exists.
`language` is the speaker menu `QTranscriptSpeaker` fills, which each edit row lists.
It subscribes the transcript notice and the draft notice, which both redraw the rows.
It binds the row list and attaches its own fill, since the template carries no bindings.

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
