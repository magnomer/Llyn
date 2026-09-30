# PTranscription.cs

## `public partial class PEditor`

The transcription rows of the input panel, rendered from the draft the engine holds.
They stand beneath the pronunciation rows, one row per scheme, and only for a language whose pack declares schemes.
A row shows its scheme as a dropdown, then the field, then its lookup button and the plus and minus.
The dropdown offers every scheme the pack declares, a scheme another row holds greyed out.
It wears no brackets and no audio, because a transcription is a spelling rather than a sound.
A row edit goes to the transcription text gate, and a scheme picked becomes a scheme request.
A plus becomes an addition request after it, and a minus a removal request.
Lookup opens the editor's one menu under the row's own button, searching in the row's scheme.
The rows are rendered as a diff on each draft bulletin, so a row being typed into is left alone.

## `private static void PTranscriptionApply(FrameworkElement container, object item, string? name)`

Fills an editor row: the shared parts through `QTranscriptionItem.QTranscriptionItemRefine`, then the field and dropdown.
The dropdown lists the row's schemes, selects its own, and greys out the taken ones.
It shows each choice by its label and selects by its scheme, paths set here rather than in the style.

## `private void PTranscriptionFieldObserve(object sender, TextChangedEventArgs e)`

Hands the row's id and the raw typed text to the transcription text gate, `CTimbre.CTimbreTranscriptionSet`.
The row keeps the draft's text until the draft returns with the typed one.

## `private static void PTranscriptionSchemeHandle(object sender, SelectionChangedEventArgs e)`

Hands the picked scheme to the row, which the editor listens to.

## `internal void PTranscriptionAddHandle(object sender, ExecutedRoutedEventArgs e)`

Adds a row in the first declared scheme the draft does not yet hold.
It lands after the row carried as the command parameter.
The engine refuses a doubled scheme, so the next free one is chosen here rather than asked for blindly.
Nothing is sent when every declared scheme already has its row.

## `internal void PTranscriptionAddCheck(object sender, CanExecuteRoutedEventArgs e)`

The plus is enabled only while a declared scheme is still free.
So it never offers a row the engine would refuse.

## `internal void PTranscriptionRemoveHandle(object sender, ExecutedRoutedEventArgs e)`

Drops the row carried as the command parameter.
The engine answers with a draft bulletin, and the render takes the row off the screen.
Dropping the last row leaves the language without one.
The next render then asks for a fresh row in the first scheme.

## `private void PTranscriptionNotationRefine(object sender, ExecutedRoutedEventArgs e)`

Opens the lookup menu under the row's own button.
It is subscribed before `PTranscriptionNotationObserve`, so the old search closes first.

## `private void PTranscriptionNotationObserve(object sender, ExecutedRoutedEventArgs e)`

Asks the errand for a search on the pressed row in that row's scheme, then paints the start.

## `private string? PTranscriptionSchemeFind()`

The first declared scheme no shown row carries, or nothing when all are taken.

## `internal void PTranscriptionShow(CEntryDraft draft)`

Renders every transcription as a row, keyed by the transcription id.
The declared schemes are asked of the language pack once per render.
Every row's dropdown then takes `CSounding.CSoundingSchemeRead`, which marks what other rows hold.
The list is shown only while there is one.
A stored row in a scheme the pack no longer declares is still shown, because it is the entry's data.
The rows come from `CTimbre.CTimbreGlyphRead`, which leaves out the glyph row the glyph block shows beneath.

## `private QTranscriptionItem PTranscriptionUpdate(QTranscriptionItem row, CTranscriptionDraft spelled)`

Brings a shown row up to the draft row with the same id.
A changed scheme is written onto the row, which relabels itself.
A row with a text request waiting keeps its text, since the draft is about to become what it holds.

## `private void PTranscriptionAttach()`

Hands the transcription list its rows and fill, and binds the three row commands on the sound panel.
