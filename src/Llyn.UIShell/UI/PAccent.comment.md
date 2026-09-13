# PAccent.cs

## `public partial class PEditor`

The further pronunciation rows of the input panel, rendered from the draft the engine holds.
The primary pronunciation keeps its own field beside the toolbar.
Every pronunciation after it stands on its own row beneath, with its variety, its IPA and a remove control.
A row edit becomes a request on that row's id, and a remove becomes a removal request.
The rows are rendered as a diff on each draft bulletin, so a row being typed into is left alone.
The primary field also wears a variety chip here, because the chip is drawn from the same draft row.

## `private static string PAccentRequestFormat(long id)`

The pending-request key of one row's IPA, so a draft render skips a row whose edit is still waiting.

## `internal void PAccentRemoveHandle(object sender, ExecutedRoutedEventArgs e)`

Drops the row carried as the command parameter.
The engine answers with a draft bulletin, and the render takes the row off the screen.

## `private void PAccentChangeHandle(object? sender, PropertyChangedEventArgs e)`

Defers an IPA request when a row's text changes.
The row model raises the change, not the text box, so a draft render writing the same text raises nothing.
The fill guard holds during a render, so a render writing a different text raises no request either.

## `private void PAccentShow(LEntryDraft draft)`

Renders every pronunciation after the primary as a row, keyed by the pronunciation id.
Whether varieties show as flags is asked of the language pack once per render.
The primary variety chip is refreshed in the same pass.
Flags not yet in the ensign store are loaded afterwards and painted in when they arrive.

## `private PAccentItem PAccentUpdate(PAccentItem row, LPronunciationDraft spoken)`

Brings a shown row up to the draft row with the same id.
A changed variety rebuilds the row, because its label and flag are fixed at creation.
A row with an IPA request waiting keeps its text, since the draft is about to become what it holds.

## `private void PAccentPrimaryShow()`

Draws the primary pronunciation's variety as a flag when one is known, and as a label otherwise.
Nothing is drawn for a primary without a variety.

## `private async Task PAccentFlagLoad(string language, bool flagged)`

Loads the variety flags the rows need and paints them once they are in the store.
A render for another language that started meanwhile wins, so a late load paints nothing.
A load that fails leaves the labels standing.

## `private void PAccentClear()`

Empties the rows and the primary chip when the form resets.
