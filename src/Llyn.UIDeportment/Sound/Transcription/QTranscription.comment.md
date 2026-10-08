# QTranscription.cs
Hash: `a2b2e6fc84c1b2af`

## `internal sealed class QTranscription`

The editor's driver for the transcription rows of the input panel, rendered from the draft the engine holds.
They stand beneath the pronunciation rows, one row per scheme, and only for a language whose pack declares schemes.
A row shows its scheme as a dropdown, then the field, then its lookup button and the plus and minus.
The dropdown offers every scheme the pack declares, a scheme another row holds greyed out.
It wears no brackets and no audio, because a transcription is a spelling rather than a sound.
A row edit goes to the transcription text gate, and a scheme picked to the scheme gate.
A plus goes to the add gate with the row, and a minus to the remove gate.
Lookup opens the editor's one menu under the row's own button, searching in the row's scheme.
The rows are rendered as a diff on each draft bulletin, so a row being typed into is left alone.

## `internal QTranscription(FrameworkElement surface, QNotation notation)`

Hands the transcription list its rows and fill, and binds the three row commands on the sound panel.
The lookup menu is the editor's one `QNotation`.

## `internal void QTranscriptionIntroduce(CEditor editor)`

Holds the Conduct editor and repaints the block after each draft change.

## `private void QTranscriptionRefine(FrameworkElement container, object item, string? name)`

Fills an editor row: the shared parts through `QTranscriptionItem.QTranscriptionItemRefine`, then the field and dropdown.
The dropdown lists the row's schemes, selects its own, and greys out the taken ones.
It shows each choice by its label and selects by its scheme, paths set here rather than in the style.

## `private void QTranscriptionFieldObserve(object sender, TextChangedEventArgs e)`

Hands the row's id and the raw typed text to the transcription text gate, `CTranscription.CTranscriptionSet`.
The row keeps the draft's text until the draft returns with the typed one.

## `private void QTranscriptionSchemeObserve(object sender, SelectionChangedEventArgs e)`

Hands the row's id and the picked scheme to the scheme gate, `CTranscription.CTranscriptionSchemeSet`.
The fill unhooks it while it sets the selection, so only a user's pick is heard.

## `private void QTranscriptionAddObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the pressed row's id, or null without a row, to the add gate, `CTranscription.CTranscriptionAdd`.
Null is Conduct's word for append, so no magic id is sent.
The engine picks the free scheme and the place after the row.

## `private void QTranscriptionAddRefine(object sender, CanExecuteRoutedEventArgs e)`

Enables the plus from the verdict the last paint stored in `_qTranscriptionFree`.
So it never offers a row the engine would refuse, and it reads nothing itself.

## `private void QTranscriptionRemoveObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the row carried as the command parameter to the remove gate, `CTranscription.CTranscriptionRemove`.
The engine answers with a draft bulletin, and the render takes the row off the screen.

## `private void QTranscriptionNotationRefine(object sender, ExecutedRoutedEventArgs e)`

Opens the lookup menu under the row's own button.
It is subscribed before `QTranscriptionNotationObserve`, so the old search closes first.

## `private void QTranscriptionNotationObserve(object sender, ExecutedRoutedEventArgs e)`

Asks the errand for a search on the pressed row in that row's scheme, then paints the start.

## `private void QTranscriptionSheetRefine(CEntryDraft _)`

Paints the ready block `CTranscription.CTranscriptionRead`, a row per transcription keyed by its id.
The block shows while the read answers it shown.
It stores the read's free verdict for the plus and asks the commands to requery.
Each row's dropdown takes the schemes its ready row carries, inside the pairing of `PCard.PCardRowShow`.
That pairing matches rows one to one, so a repeated id never gives one row two scheme lists.
A stored row in a scheme the pack no longer declares is still shown, because it is the entry's data.
The read leaves out the glyph row the glyph block shows beneath.
