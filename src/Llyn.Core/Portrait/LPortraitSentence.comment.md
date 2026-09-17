# LPortraitSentence.cs

## `public static class LPortraitSentence`

Turns one example row of a card into an unnumbered child section.

## `public static LPortraitSection LPortraitSentenceCreate(LSentenceDraft sentence, LSentenceOrder order, string language, LPortraitLabel label, IReadOnlyDictionary<long, LPortraitLink> targets, IReadOnlyDictionary<long, string> sources)`

The heading is the example word, and the role is quote, so writers draw it as an example row.
The first line carries the bracketed frame as its label and the sentence as its text.
Each written gloss follows as a line labelled by its language, or by the gloss word when unnamed.
The cited source follows as a line labelled by the source word, or the mark when unknown.
The source reads as the display cites it, by byline, which the engine hands in.
The example's own language becomes a chip only when it differs from the entry's.
The mentioned entries become a bridge child of links, each once, so a writer keeps their ids.
A row with no example holds only the frame line.
