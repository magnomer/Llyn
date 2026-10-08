# QTranscriptMention.cs
Hash: `6930521213e35ccd`

## `internal sealed class QTranscriptMention`

The corpus scribe's side of the linking gesture, the same asks the card row makes.
Every gate call is the transcript's own, through `_cTranscript`, so the engine lands it on the draft's own Example.
No call asks the leave question, which is why the gates live on `CTranscript` and not `CCorpus`.
The chip line under the transcript shows what the transcript reads from that draft.
It subscribes what it paints itself, so the owner `QCorpus` only builds and introduces it.

## `private CTranscript _cTranscript`

The corpus's transcript, read once from `CCorpusTranscript` at introduce.
Every gate call and check here goes to it without reaching through the corpus.

## `private QProspect _qTranscriptMentionProspect`

The corpus editor's Entry picker, which the link command places over the field's selection.

## `internal QTranscriptMention(UserControl scope)`

Takes the Corpus page, and finds the transcript field and its chip line in it by contract ID.
It adds the five Mention command bindings to the transcript, since that field is the corpus's, not the editor's.

## `private QMentionMenu _qTranscriptMentionMenu`

The window's mention menu, which opens the Meanings read for the field's selection.
It is the menu driver alone, so this driver never holds the window.

## `internal void QTranscriptMentionIntroduce(CCorpus corpus, CAtelier atelier, QProspect prospect, QMentionMenu mentionMenu)`

`QCorpusIntroduce` calls it once the corpus Conduct and the editor exist.
It takes the atelier rather than the window, since the span check needs only its Mention gate.
It binds the chip line and attaches its fill, since the line's template carries no bindings.
It subscribes the transcript notice and the draft notice, which both repaint the chip line.
The transcript's mention offer is wired to the picker's Refine, so no record is handed across.

## `private void QTranscriptLinkObserve(object sender, ExecutedRoutedEventArgs e)`

Places the Entry picker over the field's selection, then calls the transcript's open gate with the selected word.
The picker belongs to an editor, so the corpus's own editor shows it.
The command runs only when the selection spans anything, so no empty look happens here.

## `private void QTranscriptPickObserve(object sender, ExecutedRoutedEventArgs e)`

Hears the mention pick command run on the transcript and links its selection to the chosen Entry.
The box's text and selection are handed raw, and the span is read below Conduct.
The command's parameter is the raw nullable id, and the gate decides what null means.

## `private void QTranscriptMeaningRefine(object sender, ExecutedRoutedEventArgs e)`

Opens the mention menu on the Meanings the transcript reads ready for the field's selection.
It subscribes the `QMentionAsk` the menu answers, so only its own pick comes back.
A selection inside no linked Mention reads nothing, so the menu stays shut.

## `private void QTranscriptSenseObserve(FrameworkElement _, long sense)`

Hears its own ask's `QMentionAskChosen` and hands the sense with the field's raw text and selection.
No other area's pick reaches it, so it compares no anchor.
The Mention under the selection at the pick is the one narrowed.

## `private void QTranscriptSilenceObserve(object sender, ExecutedRoutedEventArgs e)`

Hands the field's raw selection to the silence gate, so the selection stands for nothing.
The gate is its own, so no magic id is sent.

## `private void QTranscriptUnlinkObserve(object sender, ExecutedRoutedEventArgs e)`

Calls the gate that drops the chip's Mention, or else the one that drops the Mention under the selection.
The two gates are exclusive branches by whether the command carries a chip.

## `private void QTranscriptSpanRefine(object sender, CanExecuteRoutedEventArgs e)`

Whether the field's selection spans anything, as the atelier's Mention gate measures it.
The link and silence commands share it.

## `private void QTranscriptSenseRefine(object sender, CanExecuteRoutedEventArgs e)`

Whether a Mention linked to an Entry stands under the selection, as the transcript answers.

## `private void QTranscriptUnlinkRefine(object sender, CanExecuteRoutedEventArgs e)`

Whether a chip was asked from, or a Mention stands under the selection, as the transcript answers.

## `private void QTranscriptMentionRefine(CExample _)`

Paints the chip line from the transcript's ready read, so a draft that holds no Mention shows an empty line.
The notice's Example is not read, since the transcript answers the draft's Mentions itself.
The read's labels become chips through `QMentionChipCreate` first.
