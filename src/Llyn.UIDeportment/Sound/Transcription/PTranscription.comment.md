# PTranscription.cs

## `public partial class PEditor`

The transcription rows of the input panel, rendered from the draft the engine holds.
They stand beneath the pronunciation rows, one row per scheme, and only for a language whose pack declares schemes.
A row shows its scheme as a dropdown, then the field, then its lookup button and the plus and minus.
The dropdown offers every scheme the pack declares, a scheme another row holds greyed out.
It wears no brackets and no audio, because a transcription is a spelling rather than a sound.
A row edit goes to the transcription text gate, and a scheme picked to the scheme gate.
A plus goes to the add gate with the row, and a minus to the remove gate.
Lookup opens the editor's one menu under the row's own button, searching in the row's scheme.
The rows are rendered as a diff on each draft bulletin, so a row being typed into is left alone.

## `private bool _pTranscriptionFree`

The ready verdict `CTranscriptionSheet.CTranscriptionSheetFree` from the last paint of the block.
The plus asks it on every requery, so no query reads the draft again.

## `private static void PTranscriptionRefine(FrameworkElement container, object item, string? name)`

Fills an editor row: the shared parts through `QTranscriptionItem.QTranscriptionItemRefine`, then the field and dropdown.
The dropdown lists the row's schemes, selects its own, and greys out the taken ones.
It shows each choice by its label and selects by its scheme, paths set here rather than in the style.

## `private void PTranscriptionFieldObserve(object sender, TextChangedEventArgs e)`

Hands the row's id and the raw typed text to the transcription text gate, `CTranscription.CTranscriptionSet`.
The row keeps the draft's text until the draft returns with the typed one.

## `private void PTranscriptionSchemeObserve(object sender, SelectionChangedEventArgs e)`

Hands the row's id and the picked scheme to the scheme gate, `CTranscription.CTranscriptionSchemeSet`.
The fill unhooks it while it sets the selection, so only a user's pick is heard.

## `private void PTranscriptionAddObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the pressed row's id, or zero without a row, to the add gate, `CTranscription.CTranscriptionAdd`.
The engine picks the free scheme and the place after the row.

## `private void PTranscriptionAddRefine(object sender, CanExecuteRoutedEventArgs e)`

Enables the plus from the verdict the last paint stored in `_pTranscriptionFree`.
So it never offers a row the engine would refuse, and it reads nothing itself.

## `private void PTranscriptionRemoveObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the row carried as the command parameter to the remove gate, `CTranscription.CTranscriptionRemove`.
The engine answers with a draft bulletin, and the render takes the row off the screen.

## `private void PTranscriptionNotationRefine(object sender, ExecutedRoutedEventArgs e)`

Opens the lookup menu under the row's own button.
It is subscribed before `PTranscriptionNotationObserve`, so the old search closes first.

## `private void PTranscriptionNotationObserve(object sender, ExecutedRoutedEventArgs e)`

Asks the errand for a search on the pressed row in that row's scheme, then paints the start.

## `internal void PTranscriptionSheetRefine(CEntryDraft _)`

Paints the ready block `CTranscription.CTranscriptionRead`, a row per transcription keyed by its id.
The block shows while the read answers it shown.
It stores the read's free verdict for the plus and asks the commands to requery.
Each row's dropdown takes the schemes its ready row carries, paired by the row's id.
A stored row in a scheme the pack no longer declares is still shown, because it is the entry's data.
The read leaves out the glyph row the glyph block shows beneath.

## `private QTranscriptionItem PTranscriptionStateRefine(QTranscriptionItem row, CTranscriptionDraft spelled)`

Brings a shown row up to the draft row with the same id.
A changed scheme is written onto the row, which relabels itself.
A row with a text request waiting keeps its text, since the draft is about to become what it holds.

## `private void PTranscriptionAttach()`

Hands the transcription list its rows and fill, and binds the three row commands on the sound panel.
