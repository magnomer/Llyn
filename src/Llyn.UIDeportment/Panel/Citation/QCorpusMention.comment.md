# QCorpusMention.cs

## `internal sealed partial class QCorpus`

The corpus scribe's side of the linking gesture, the same four asks the card row makes.
Every gate call names card 0 and sentence 0, so the engine lands it on the draft's own Example.
The chip line under the transcript shows what the draft holds.

## `private void QTranscriptLinkHandle(object sender, ExecutedRoutedEventArgs e)`

Reads the selection off the field the command came from and hands it straight on.
The mention menu stands only on the transcript's sentence field, so the source is that field.

## `private void QTranscriptLinkShow(`

Opens the Entry picker over a selection that spans anything.
The picker belongs to an editor, so the corpus's own editor shows it.

## `private Action<long> QTranscriptLinkRead(int offset, int length)`

The pick the Entry picker answers with, which links the selection to the chosen Entry.

## `private void QTranscriptSenseHandle(object sender, ExecutedRoutedEventArgs e)`

Persists pending typing, so the Mention is found against the text the field shows.
The Mention under the selection is handed straight to the sense show.

## `private void QTranscriptSenseShow(CMentionDraft? mention)`

Opens the Meaning menu on the Entry a linked Mention stands for.
A Mention standing for nothing offers no Meanings, so the menu stays shut for it.

## `private Action<long> QTranscriptSenseRead(long mention)`

The pick the Meaning menu answers with, which points the Mention at the chosen sense.

## `private void QTranscriptSilenceHandle(object sender, ExecutedRoutedEventArgs e)`

Reads the selection off the field the command came from and hands it straight on.

## `private void QTranscriptSilenceRun(`

Marks a selection that spans anything as standing for nothing, which is an addition with Entry 0.

## `private void QTranscriptUnlinkHandle(object sender, ExecutedRoutedEventArgs e)`

Drops the Mention whose chip was asked from, or else the one under the selection.
Pending typing is persisted first, so the Mention is found against the text the field shows.

## `private void QTranscriptUnlinkRun(long? mention)`

Hands a found Mention to the gate, and nothing when none was found.

## `private void QTranscriptLinkCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether the field's selection spans anything, as the window's engine measures it.

## `private void QTranscriptSenseCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether a Mention under the selection stands for an Entry.

## `private void QTranscriptUnlinkCheck(object sender, CanExecuteRoutedEventArgs e)`

Whether a chip was asked from, or a Mention stands under the selection.

## `private CMentionDraft? QTranscriptMentionFind()`

The held Mention under the field's selection, as the desk finds it.

## `private void QTranscriptMentionShow(CExample? example)`

Takes the Mentions the held sentence carries and redraws the chip line from them.
A draft that stands empty clears the line.
