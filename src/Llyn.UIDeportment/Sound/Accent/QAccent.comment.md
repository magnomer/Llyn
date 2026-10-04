# QAccent.cs
Hash: `37ab74c6aaf9c514`

## `internal sealed class QAccent`

The further pronunciation rows of the input panel, rendered from the draft the engine holds.
The primary pronunciation keeps its own field above them.
Every pronunciation after it stands on its own row beneath, with its variety, its IPA and its controls.
The controls are play when the row has audio, lookup, download and the plus and minus pair.
That is the same set the primary row wears.
A row edit goes to the accent gate with that row's id.
A plus or a minus goes to the add or remove gate with that row's id.
Lookup and download open the editor's one menu under the row's own button.
The rows are rendered as a diff on each draft bulletin.
The primary field also wears a variety chip here, because the chip is drawn from the same draft row.

## `internal void QAccentIntroduce(CEditor editor)`

Holds the Conduct editor and repaints the rows and the flags after each draft change.

## `private void QAccentAddObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the pressed row's id to the add gate, or zero for the primary when no row is carried.

## `private void QAccentRemoveObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the pressed row's id to the remove gate, or zero for the primary when no row is carried.
The engine answers with a draft bulletin, and the render takes the row off the screen.

## `private void QAccentNotationRefine(object sender, ExecutedRoutedEventArgs e)`

Opens the pronunciation menu under the row's own button, through the editor's `QNotation`.
It is subscribed before `QAccentNotationObserve`, so the old search closes first.

## `private void QAccentNotationObserve(object sender, ExecutedRoutedEventArgs e)`

Asks the errand for a pronunciation search on the pressed row, then paints the start.

## `private void QAccentClipRefine(object sender, ExecutedRoutedEventArgs e)`

Opens the audio menu under the row's own button.
It is bound before the clip Observe, since shutting the popup cancels the search.

## `private void QAccentClipObserve(object sender, ExecutedRoutedEventArgs e)`

Starts the recording search for the pressed row and paints the roll it answers.

## `private void QAccentPlaybackObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the pressed row's id to the play gate and its answer to the playback Refine.

## `private void QAccentPlaybackRefine(Uri? address)`

Plays the address on the editor's player, handed in at build.
A null answer plays nothing, since the engine cleared the gone file and the draft bulletin repaints the row.
A file the player then refuses reaches the user through the sound driver's one failure subscription.

## `private UIElement QAccentAnchorRead(ExecutedRoutedEventArgs e)`

The button that raised the command, so the menu opens under it rather than under the list.

## `private void QAccentRefine(CEntryDraft _)`

Answers a draft bulletin by showing the list for a spoken language and painting the editor's ready accent sheet.

## `private void QAccentRefine(CTimbreAccent accent)`

Paints every pronunciation after the primary as a row, keyed by the pronunciation id.
The primary variety is painted in the same pass.

## `private QAccentItem QAccentRowRefine(CAccent spoken, CTimbreAccent accent)`

Builds the row for one ready accent and listens to its text.

## `private QAccentItem QAccentRowRefine(QAccentItem row, CAccent spoken, CTimbreAccent accent)`

Brings a shown row up to the ready accent with the same id.
A changed variety or mark rebuilds the row, because its label, flag and brackets are fixed at creation.
The audio is always taken from the sheet, because the row never edits it.

## `private void QAccentPrimaryRefine(CTimbreAccent accent)`

Draws the primary variety as a flag when one is in the store, and as a label otherwise.

## `private async void QAccentEnsignRefine(CEntryDraft _)`

Answers the same bulletin after the sheet, loading the variety flags through the editor's flag read.
It paints the answer once they are in the store.
It is its own subscriber, so each Refine asks Conduct once.

## `private void QAccentFlagRefine(CTimbreAccent? accent)`

Paints the stored flags on every row and on the primary.
A null answer paints nothing, since the load failed or another language took the editor meanwhile.

## `internal QAccent(FrameworkElement surface, QNotation notation, QClip menu, MediaPlayer player)`

Holds the surface, the notation and clip menus, and the player the row commands use.
Binds the accent list to its rows and attaches its look, quill and control behaviour.
It also binds the five row commands on the sound panel.
The bindings stand on that panel, as the markup had them, so a row command reaches this editor.
The clip binding carries its Refine and then its Observe, one handler per role in that order.
