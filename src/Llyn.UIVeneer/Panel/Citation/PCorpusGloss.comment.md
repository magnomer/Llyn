# PCorpusGloss.cs

## `public partial class PCorpus`

The Gloss rows of the corpus panel: the ones the read area lists and the ones the edit area holds.

## `private void PExcerptGlossShow(IReadOnlyList<LGloss> glosses)`

Lists every Gloss of the shown Example in the read area.
Shows the unset mark instead when there is none.

## `private void PTranscriptGlossShow(LExample? example)`

Redraws the edit rows from the held Example, keeping the rows whose ids survive.
A row whose text is still dirty keeps what the user typed.

## `private PGloss PTranscriptGlossCreate(LGlossDraft draft)`

One row, subscribed so its edits reach the panel.

## `private void PTranscriptGlossChange(object? sender, PropertyChangedEventArgs arguments)`

Sends a chosen language at once, since the row notices its picker.
No dirty set is kept, since a restore writes what is waiting first and then reads it back.

## `internal void PTranscriptGlossHandle(object sender, TextChangedEventArgs e)`

Defers the text typed into a Gloss field as its own request, keyed by the row.
So the tenure keeps only the latest.
Only a field the keyboard is in has been typed into, so a value the engine drew raises nothing.

## `private void PGlossAddHandle(object sender, RoutedEventArgs e)`

Adds a Gloss in English, after the row whose plus was pressed, or at the end from the seed line.

## `private void PGlossRemoveHandle(object sender, RoutedEventArgs e)`

Drops the Gloss whose minus was pressed.

## `private void PTranscriptSeedShow()`

Shows the seed line only while the list is empty, so a first row can be added from somewhere.

## `private void PTranscriptSeedHandle(object sender, KeyboardFocusChangedEventArgs e)`

Focusing the seed field adds the first row and moves the caret into it once the row is drawn.

## `private string PGlossLanguageRead()`

English when it is loaded, else the first loaded language, else empty.
