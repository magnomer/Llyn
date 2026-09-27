# QCorpusGloss.cs

## `internal sealed partial class QCorpus`

The Gloss rows of the corpus panel: the ones the read area lists and the ones the edit area holds.
Every edit goes through the desk's text gate, with card and sentence zero for the Example itself.

## `private void QExcerptGlossShow(IReadOnlyList<CGlossDraft> glosses)`

Lists every Gloss of the shown Example in the read area.
Hides the whole section when there is none, as a never-written field is not drawn.

## `private void QTranscriptGlossShow(CExample? example)`

Redraws the edit rows from the held Example, keeping the rows whose ids survive.

## `private void QGlossTextHandle(object sender, TextChangedEventArgs e)`

Hands the text typed into a Gloss field to the gate, which defers it keyed by the row.

## `private void QGlossSpeakerHandle(object sender, SelectionChangedEventArgs e)`

Hands a language picked in a row's list to the gate, which sends it at once.
The row's own fill sets the list with this handler off, so only a pick reaches the gate.

## `private void QGlossAddHandle(object sender, RoutedEventArgs e)`

Adds a Gloss after the row whose plus was pressed, or at the end from the seed line.
The language it starts in is the workspace gate's `QWorkspaceGlossRead`, read from the settings.

## `private void QGlossRemoveHandle(object sender, RoutedEventArgs e)`

Drops the Gloss whose minus was pressed.

## `private void QTranscriptGlossApply(FrameworkElement container, object item, string? change)`

Fills one transcript Gloss row through `PGloss.PGlossRowApply` with the row's text and list handlers off.
So a fill never reads as typing or a pick.
It subscribes the plus and minus clicks and gives them their icons, which their styles leave bare.

## `private void QTranscriptSeedShow()`

Shows the seed line only while the list is empty, so a first row can be added from somewhere.

## `private void QTranscriptSeedHandle(object sender, KeyboardFocusChangedEventArgs e)`

Focusing the seed field adds the first row and moves the caret into it once the row is drawn.
