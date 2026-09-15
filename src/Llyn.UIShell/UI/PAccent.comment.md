# PAccent.cs

## `public partial class PEditor`

The further pronunciation rows of the input panel, rendered from the draft the engine holds.
The primary pronunciation keeps its own field above them.
Every pronunciation after it stands on its own row beneath, with its variety, its IPA and its controls.
The controls are play when the row has audio, lookup, download and the plus and minus pair.
That is the same set the primary row wears.
A row edit becomes a request on that row's id.
A plus becomes an addition request after it, and a minus a removal request.
Lookup and download open the editor's one menu under the row's own button.
The rows are rendered as a diff on each draft bulletin, so a row being typed into is left alone.
The primary field also wears a variety chip here, because the chip is drawn from the same draft row.

## `private LRequest PAccentRequestCreate(PAccentItem row)`

The request a row's typed text becomes, a respelling request while respellings are shown and a reading request otherwise.
The row prints the form the switch picks, so the text it holds belongs to that form.

## `private static string PAccentRequestFormat(long id)`

The pending-request key of one row's IPA, so a draft render skips a row whose edit is still waiting.

## `internal void PAccentAddHandle(object sender, ExecutedRoutedEventArgs e)`

Adds a blank pronunciation after the row carried as the command parameter, or after the primary when none is.
A primary the draft does not yet hold is added first.
So the new row lands beneath it rather than in its place.

## `internal void PAccentRemoveHandle(object sender, ExecutedRoutedEventArgs e)`

Drops the row carried as the command parameter, or the primary when none is.
Nothing is sent for a primary the draft does not hold.
The engine answers with a draft bulletin, and the render takes the row off the screen.

## `internal async void PAccentNotationHandle(object sender, ExecutedRoutedEventArgs e)`

Opens the pronunciation menu under the row's own button, for that row.

## `internal async void PAccentClipHandle(object sender, ExecutedRoutedEventArgs e)`

Opens the audio menu under the row's own button, for that row.

## `internal void PAccentPlaybackHandle(object sender, ExecutedRoutedEventArgs e)`

Plays the row's recording on the editor's player.
A file gone from disk is cleared off the row instead, so the button stops offering it.

## `private UIElement PAccentAnchorRead(ExecutedRoutedEventArgs e)`

The button that raised the command, so the menu opens under it rather than under the list.

## `private PAccentItem? PAccentFind(long id)`

The shown row with one pronunciation id, or nothing when the draft no longer has it.

## `private void PAccentFreshClear()`

Clears the audio off every further row downloaded this session and not yet stored.
A fetched recording is audio of one spelling in one language, so a change of either drops it.
A row that has gone is skipped, since its audio went with it.

## `private void PAccentChangeHandle(object? sender, PropertyChangedEventArgs e)`

Defers a reading or respelling request when a row's text changes.
The row model raises the change, not the text box, so a draft render writing the same text raises nothing.
The fill guard holds during a render, so a render writing a different text raises no request either.

## `private void PAccentShow(LEntryDraft draft)`

Renders every pronunciation after the primary as a row, keyed by the pronunciation id.
Whether varieties show as flags is asked of the language pack once per render.
Which form the rows print and which brackets they wear is read once per render too.
A change of that state drops every row first, because a row fixes its brackets when it is built.
The primary id is kept, so the primary row's own plus and minus reach the right pronunciation.
The primary variety chip is refreshed in the same pass.
Flags not yet in the ensign store are loaded afterwards and painted in when they arrive.

## `private PAccentItem PAccentUpdate(PAccentItem row, LPronunciationDraft spoken)`

Brings a shown row up to the draft row with the same id.
A changed variety rebuilds the row, because its label and flag are fixed at creation.
A row with a text request waiting keeps its text, since the draft is about to become what it holds.
The audio is always taken from the draft, because the row never edits it.

## `private void PAccentPrimaryShow()`

Draws the primary pronunciation's variety as a flag when one is known, and as a label otherwise.
Nothing is drawn for a primary without a variety.

## `private async Task PAccentFlagLoad(string language, bool flagged)`

Loads the variety flags the rows need and paints them once they are in the store.
A render for another language that started meanwhile wins, so a late load paints nothing.
A load that fails leaves the labels standing.

## `private void PAccentRowClear()`

Drops every row and stops listening to it, for a rebuild or a reset.

## `private void PAccentClear()`

Empties the rows, the primary chip and the fresh-audio set when the form resets.
