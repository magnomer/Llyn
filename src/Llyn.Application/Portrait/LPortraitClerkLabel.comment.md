# LPortraitClerkLabel.cs
Hash: `0d9717d181b0714e`

## `public static class LPortraitClerkLabel`

The wording of entry and catalog pages, built from the words a caller hands in.
The kind words and the legend sit together, since the legend looks up exactly the words the kind read lists.

## `public static IReadOnlyList<string> LPortraitKindRead()`

The stored word of every Source kind, in the kind's own order.
A legend is worded per kind by these words, so no caller names the kinds.

## `public static LPortraitLabel LPortraitLabelCreate(IReadOnlyList<string> words)`

The label an entry page is worded with, taken word by word in the record's order.
The last four words name the units Content, Function, Morpheme and Word, in that order.
It throws unless every one of the label's words is given.

## `public static LPortraitLegend LPortraitLegendCreate(IReadOnlyList<string> words, IReadOnlyDictionary<string, string> kinds)`

The legend a catalog page is worded with, taken word by word in the record's order.
Each Source kind is worded by its stored word, and a kind the words leave out keeps its own name.
