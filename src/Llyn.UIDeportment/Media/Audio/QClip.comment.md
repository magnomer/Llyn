# QClip.cs
Hash: `73c0340db4b50623`

## `internal sealed class QClip`

Audio download as the editor shows it.
Every pronunciation row carries its own download button, and the menu opens under the one pressed.
The menu is one popup the editor owns, retargeted at the row that asked for it.
Opening it starts a search for recordings of the headword, the same search from whichever row.
The search and the menu's state live on the desk's errand, and the menu keeps neither.
Recordings stream in from the engine and fill the list, one per variety a source returned.
The engine already narrowed them to the variety of the row that opened the menu.
Each recording can be previewed, and taking one has the errand download, attach and tag it.
The form refills from the draft bulletin that attach raises.
The errand marshals every step onto the editor's thread and raises the clip state once per step.
A preview or a taking raises it too, as each of its stages lands.
The menu repaints the ready clip state each raise carries.
So the menu decides nothing about a step, a preview or a download.

## `internal QClip(FrameworkElement surface, MediaPlayer player)`

Hands the audio list its fill, and wires the download button and the menu's close.
The button's open is heard before its start, since placing the popup shuts it and cancels the search.
The download icon is set here, where the markup held an icon lookup.

## `internal void QClipIntroduce(CErrand errand)`

Holds the desk's errand and subscribes the menu's Refine to the errand's clip notice.

## `private void QClipButtonRefine(object sender, RoutedEventArgs e)`

Places the menu under the editor's own download button.

## `private void QClipButtonObserve(object sender, RoutedEventArgs e)`

Starts the search for the primary row once the menu stands.
A null target is the primary row, which the engine may not have minted yet.
Null is Conduct's word for no row, so no magic id is sent.

## `private void QClipClosedObserve(object? sender, EventArgs e)`

A closed menu ends the errand's searches and forgets the marked preview.
Nothing is repainted, since the next opening paints the fresh state its start answers.

## `internal void QClipOpenRefine(UIElement anchor)`

Opens the menu under the button of one row.
The popup is shut first, so a press on another row's button moves it instead of leaving it put.

## `internal void QClipRecordingStart(long? id)`

Starts the recording search for the row with that id and paints the roll it answers.
A null id is the primary row, passed on to the errand untouched.
The input editor's accent rows call it, so no Conduct roll crosses to the surface.

## `private async void QClipEnsignRefine(CClipRoll roll)`

Paints the state the start answered, then paints it again once the variety flags loaded.
The flags load while the search runs, so a recording that landed first gains its flag here.

## `private void QClipRefine(CClipRoll roll)`

What the menu shows, from the ready clip state.
The running line follows the search alone, since a source that answered does not end it.
The notice shows only while no row stands, since a row reports its own download itself.

## `private void QClipRowApply(FrameworkElement container, object item, string? _)`

Fills one source row, and shows its recordings or its note by whether the source is ready.
It attaches the reading fill to the row's own recording list.

## `private void QClipReadingApply(FrameworkElement container, object item, string? _)`

Fills one recording, its flag or name, its play button and its taking button.
The play button carries the preview state as its cue, playing before fetching before refused.
The column joins the shared size group of its place in the row.
Both buttons are unsubscribed first, since a recycled container would otherwise hear twice.

## `private async void QClipPreviewObserve(object sender, RoutedEventArgs e)`

Hands the pressed recording to the preview gate, and its answer to the player.

## `private void QClipPreviewRefine(Uri? address)`

Plays the local file the preview gate fetched.
No address means the fetch failed or another preview took over, so nothing sounds.

## `private void QClipEndObserve(object? sender, EventArgs e)`

The player's end and failure both finish the preview, since either way nothing sounds any more.
The player is shared, so an end with no preview marked changes nothing.

## `private async void QClipSelectorObserve(object sender, RoutedEventArgs e)`

Hands the taken recording to the save gate, and its verdict to the menu.

## `private void QClipCloseRefine(bool attached)`

Taking a recording closes the menu, the way taking a pronunciation candidate does.
A recording the draft had moved away from leaves the menu open.
