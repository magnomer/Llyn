# TCatalogTag.cs

## `public sealed class TCatalogTag`

Covers the tag catalog, which is ordered and matched over one text and nothing else.
It covers name order and its reverse.
It covers the query, which ignores case because a tag is read as a word.
It covers the split of a found text around the typed word, which a picker draws in weight.
It covers the usage count an offered row shows, which is empty for a row nobody uses.
