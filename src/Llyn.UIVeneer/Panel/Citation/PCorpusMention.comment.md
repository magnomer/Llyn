# PCorpusMention.cs

## `public partial class PCorpus`

The corpus scribe's side of the linking gesture, the same four asks the card row makes.
Every request names card 0 and row 0, so the engine lands it on the draft's own Example.
The chip line under the transcript shows what the draft holds.

## Inline notes

### `private void PTranscriptLinkHandle(object sender, ExecutedRoutedEventArgs e)`

Asks the window for the Entry picker over the selection.
The picker belongs to the editor.
So the window passes the ask through rather than the scribe holding a second popup.
On pick the selection becomes a Mention of the chosen Entry.

### `private void PTranscriptSenseHandle(object sender, ExecutedRoutedEventArgs e)`

Opens the Meaning menu on the Entry the Mention under the selection stands for.
A Mention standing for nothing offers no Meanings, so the item is disabled for it.
Pending typing is persisted first, so the Mention is found against the text the field shows.

### `private void PTranscriptSilenceHandle(object sender, ExecutedRoutedEventArgs e)`

Marks the selection as standing for nothing, which is an addition with Entry 0.

### `private void PTranscriptUnlinkHandle(object sender, ExecutedRoutedEventArgs e)`

Drops the Mention under the selection, or the one whose chip was asked from.
A chip carries itself as the command parameter, so the same command serves both.
Pending typing is persisted first, so the Mention is found against the text the field shows.

### `private void PTranscriptMentionShow(LExample? example)`

Takes the Mentions the held sentence carries and redraws the chip line from them.
A draft that stands empty clears the line and what it had read.
