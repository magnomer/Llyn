# QExcerpt.cs
Hash: `63a34235d8c793b4`

## `internal sealed class QExcerpt`

The reading side of the Corpus panel, the page one chosen Example is shown on.
The sentence stands at its head, its Gloss rows under it, and the cited Source last.
A click on a word asks the corpus's gate what it names.
It subscribes what it paints itself, so the owner `QCorpus` only builds, introduces and lights it.

## `internal QExcerpt(UserControl scope)`

Takes the Corpus page, and finds every `PExcerpt` part in it by contract ID.
It subscribes the word clicks of the sentence.

## `internal event Action<PMention, CMentionOffer?>? QExcerptMentionOffered`

Carries the offer a word click's gate answered, with the sentence the word stands in.
The owner hands it to the window's mention menu, so this driver never holds the window.

## `internal void QExcerptIntroduce(CCorpus corpus, ObservableCollection<PLanguageItem> language)`

`QCorpusIntroduce` calls it once the corpus Conduct exists.
`language` is `QTranscriptSpeakerLanguage`, the menu `QTranscriptSpeaker` fills, which each read Gloss row lists.
It subscribes the corpus's example notice and the anthology's rows event.
It binds the Gloss list and attaches the shared Gloss fill.

## `internal void QExcerptVisibleRefine()`

Shows the page while the diptych shows its parent, and the body or the unselected notice as the corpus says.
The owner's mode update calls it with the rest of the panel's mode.

## `private void QExcerptTallyRefine()`

Rewrites the tally chip from `CApertureTallyRead` when the catalog's rows change.
A quotation added elsewhere refills the catalog, so its count shows at once.

## `private void QExcerptRefine(CExample example)`

Paints the read page of an Example the corpus raises.
The tally chip paints the ready `CExampleTally`, the chosen Example's count.

## `private void QExcerptSentenceRefine(CExample example)`

Writes the sentence at the head of the page, as a situation's title stands at the head of its page.
It looks up `CExampleWording` when Conduct chose a word, and shows the text itself otherwise.
`CExampleMuted` picks the muted colour, because the head of the page cannot be empty.
The text itself draws the pieces Conduct divided at its Mentions.
A worded key draws as one plain piece made by `QMentionPieceCreate`, since the pieces belong to the text it replaces.
The click hands the corpus gate only raw click values, so the mention text holds no Mentions here.

## `private void QExcerptGlossShow(IReadOnlyList<CGlossDraft> glosses)`

Lists every Gloss of the shown Example in the read area.
Hides the whole section when there is none, as a never-written field is not drawn.

## `private void QExcerptCitationRefine(CExample example)`

Writes the Example's ready citation line under its heading, or hides the heading when no citation was ever written.

## `private void QExcerptMentionObserve(object? sender, PMentionArgument e)`

Reads the raw click values and hands them unchanged to its one gate, `CCorpusMentionFind`.
The gate asks, converts the click, finds, opens and reports failures.
Its ready offer goes out through `QExcerptMentionOffered`, to be shown under the found word.
