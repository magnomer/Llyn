# LPortraitFrame.cs

## `public static class LPortraitFrame`

Builds the bracketed head that stands before an example sentence.

## `public static string LPortraitFrameRead(LSentenceDraft sentence, LSentenceOrder order, string mark)`

The frame belongs to the card's hold on the Example, so it is read off that row.
The particle and the dependence are placed in the order the language declares.
An empty pair yields no head at all, so no lone brackets are drawn.
The result matches what the display puts left of the sentence.
