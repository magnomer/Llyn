# LPortraitClerkLabel.cs
Hash: `2d3cf92501455e97`

## `public static class LPortraitClerkLabel`

The wording of entry and catalog pages, built from the words a caller hands in.
The kind words and the legend sit together, since the legend looks up exactly the words the kind read lists.

## `public static IReadOnlyList<string> LPortraitKindRead()`

The stored word of every Source kind, in the kind's own order.
A legend is worded per kind by these words, so no caller names the kinds.

## `public static LPortraitLabel LPortraitLabelCreate(IReadOnlyList<string> words)`

The label an entry page is worded with, taken word by word in the record's order.
The first 24 words fill the record's text fields, from Unknown to Tag.
The Other, Stub and Heading words stand between the Collocation word and the Incoming word.
The next three words name the units Morpheme, Word and Expression, in that order.
The last two name the freedoms Dependent and Standalone, in that order.
It throws unless exactly 29 words are given.

## `public static LPortraitLegend LPortraitLegendCreate(IReadOnlyList<string> words, IReadOnlyDictionary<string, string> kinds)`

The legend a catalog page is worded with, taken word by word in the record's order.
It throws unless exactly 13 words are given.
Each Source kind is worded by its stored word, and a kind the words leave out keeps its own name.
