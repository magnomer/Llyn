# PTranscription.cs

## `public partial class PEditor`

The transcription rows of the input panel, rendered from the draft the engine holds.
They stand beneath the pronunciation rows, one row per scheme, and only for a language whose pack declares schemes.
A row shows its scheme as a dropdown, then the field, then its lookup button and the plus and minus.
The dropdown offers every scheme the pack declares, a scheme another row holds greyed out.
It wears no brackets and no audio, because a transcription is a spelling rather than a sound.
A row edit becomes a text request on that row's id, and a scheme picked becomes a scheme request.
A plus becomes an addition request after it, and a minus a removal request.
Lookup opens the editor's one menu under the row's own button, searching in the row's scheme.
The rows are rendered as a diff on each draft bulletin, so a row being typed into is left alone.

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

## `internal async void PTranscriptionNotationHandle(object sender, ExecutedRoutedEventArgs e)`

Opens the lookup menu under the row's own button, for that row and in that row's scheme.

## `private string? PTranscriptionSchemeFind()`

The first declared scheme no shown row carries, or nothing when all are taken.

## `private LTranscriptionItem? PTranscriptionFind(long id)`

The shown row with one transcription id, or nothing when the draft no longer has it.
The glyph row is searched too, because the lookup menu fills it through the same path.

## `private void PTranscriptionChangeHandle(object? sender, PropertyChangedEventArgs e)`

Sends a scheme request at once when a row's scheme changes, and defers a text request when its text changes.
A scheme change is sent at once because it is one pick, not a keystroke stream.
The row model raises the change, not the text box, so a draft render writing the same text raises nothing.
The fill guard holds during a render, so a render writing a different text raises no request either.

## `private void PTranscriptionShow(LEntryDraft draft)`

Renders every transcription as a row, keyed by the transcription id.
The declared schemes are asked of the language pack once per render.
Every row's dropdown is then told which schemes the other rows hold, so it greys those out.
The list is shown only while there is one.
A stored row in a scheme the pack no longer declares is still shown, because it is the entry's data.
A row in the glyph scheme is left out, because the glyph row shows it beneath.

## `private IReadOnlyList<LTranscriptionDraft> PTranscriptionScan(LEntryDraft draft)`

The draft's transcription rows without the glyph row of the language shown.

## `private static IReadOnlyList<LTranscriptionDraft> PTranscriptionScan(LEntryDraft draft, LGlyph? glyph)`

The draft's transcription rows without the row in `glyph`'s scheme.
The glyph form is stored as a transcription row, so this is where the two are told apart.

## `private LTranscriptionItem PTranscriptionUpdate(LTranscriptionItem row, LTranscriptionDraft spelled)`

Brings a shown row up to the draft row with the same id.
A changed scheme is written onto the row, which relabels itself.
A row with a text request waiting keeps its text, since the draft is about to become what it holds.
