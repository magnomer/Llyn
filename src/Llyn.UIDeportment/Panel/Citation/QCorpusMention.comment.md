# QCorpusMention.cs
Hash: `87732a83528e0fa3`

## `internal sealed partial class QCorpus`

The corpus scribe's side of the linking gesture, the same asks the card row makes.
Every gate call is the corpus's own, so the engine lands it on the draft's own Example.
The chip line under the transcript shows what the corpus reads from that draft.

## `private void QTranscriptLinkObserve(object sender, ExecutedRoutedEventArgs e)`

Places the Entry picker over the field's selection, then calls the corpus's open gate with the selected word.
The picker belongs to an editor, so the corpus's own editor shows it.
The command runs only when the selection spans anything, so no empty look happens here.

## `private void QTranscriptPickObserve(object sender, ExecutedRoutedEventArgs e)`

Hears the mention pick command run on the transcript and links its selection to the chosen Entry.
The box's text and selection are handed raw, and the span is read below Conduct.

## `private void QTranscriptMeaningRefine(object sender, ExecutedRoutedEventArgs e)`

Opens the Meaning menu on the Meanings the corpus reads ready for the field's selection.
A selection inside no linked Mention reads nothing, so the menu stays shut.

## `private void QTranscriptSenseObserve(FrameworkElement anchor, long sense)`

Hears the Meaning menu's pick and hands the sense with the field's raw text and selection.
The Mention under the selection at the pick is the one narrowed.

## `private void QTranscriptSilenceObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the field's raw selection to the add gate with Entry 0, which marks it as standing for nothing.

## `private void QTranscriptUnlinkObserve(object sender, ExecutedRoutedEventArgs e)`

Calls the gate that drops the chip's Mention, or else the one that drops the Mention under the selection.
The two gates are exclusive branches by the command's source.

## `private void QTranscriptSpanRefine(object sender, CanExecuteRoutedEventArgs e)`

Whether the field's selection spans anything, as the window's engine measures it.
The link and silence commands share it.

## `private void QTranscriptSenseRefine(object sender, CanExecuteRoutedEventArgs e)`

Whether a Mention linked to an Entry stands under the selection, as the corpus answers.

## `private void QTranscriptUnlinkRefine(object sender, CanExecuteRoutedEventArgs e)`

Whether a chip was asked from, or a Mention stands under the selection, as the corpus answers.

## `private void QTranscriptMentionRefine()`

Paints the chip line from the corpus's ready read, so a draft that holds no Mention shows an empty line.
The read's labels become chips through `QMentionChipCreate` first.
