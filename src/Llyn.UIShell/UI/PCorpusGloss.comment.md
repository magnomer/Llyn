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

Marks a typed row dirty and defers the save with the rest of the form.
A chosen language is sent at once.

## `private IReadOnlyList<LRequest> PTranscriptGlossRead(long draft)`

The text requests of every dirty row, sent after the body, and the dirty set cleared.

## `private void PGlossAddHandle(object sender, RoutedEventArgs e)`

Adds a Gloss at the end of the list, in English.

## `private void PGlossRemoveHandle(object sender, ExecutedRoutedEventArgs e)`

Drops the Gloss the command carries.

## `private string PGlossLanguageRead()`

English when it is loaded, else the first loaded language, else empty.
