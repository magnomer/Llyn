# QCorpusExcerpt.cs

## `internal sealed partial class QCorpus`

The reading side of the Corpus panel, the page one chosen Example is shown on.
The sentence stands at its head and the cited Source under it.
A click on a word asks the corpus's gate what it names.

## `private void QExcerptRefine(CExample example)`

Paints the read page of an Example the corpus raises.
The tally chip paints the ready `CExampleTally`, the chosen Example's count.

## `private void QExcerptSentenceRefine(CExample example)`

Writes the sentence at the head of the page, as a situation's title stands at the head of its page.
It looks up `CExampleWording` when Conduct chose a word, and shows the text itself otherwise.
`CExampleMuted` picks the muted colour, because the head of the page cannot be empty.
The text itself draws the pieces Conduct divided at its Mentions.
A worded key draws as one plain piece made by `QMentionPieceCreate`, since the pieces belong to the text it replaces.
The click asks the corpus gate by offset alone, so the mention text holds no Mentions here.

## `private void QExcerptCitationRefine(CExample example)`

Writes the Example's ready citation line under its heading, or hides the heading when no citation was ever written.

## `private void QExcerptMentionObserve(object? sender, PMentionArgument e)`

Hands the clicked offset to `CCorpusMentionFind`, which asks, finds, opens and reports failures.
Its ready offer goes to the window's menu under the clicked word.
