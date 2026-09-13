# PDisplayAccent.cs

## `public partial class PDisplay`

The further pronunciations of a shown entry, listed under the primary one.
Each stands with its variety as a flag or a label, its IPA in brackets, and play when audible.
A row without an IPA is left out, because the reading view shows only what reads.
The primary pronunciation wears its own variety chip in the same way.

## `private void PDisplayAccentShow(LEntryDraft draft)`

Rebuilds the rows from the draft, and asks the pack once whether varieties draw as flags.
Flags not yet in the ensign store are loaded afterwards and painted in when they arrive.

## `internal void PDisplayPlaybackHandle(object sender, ExecutedRoutedEventArgs e)`

Plays the row's recording on this view's player.
A file that has gone from disk plays nothing.

## `private void PDisplayPrimaryShow()`

Draws the primary pronunciation's variety as a flag when one is known, and as a label otherwise.

## `private async Task PDisplayFlagLoad(string language, bool flagged)`

Loads the variety flags the rows need and paints them once they are in the store.
Another entry shown meanwhile wins, so a late load paints nothing.

## `private void PDisplayAccentClear()`

Empties the rows and the primary chip when the view is cleared.
