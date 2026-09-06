# LCatalogTag.cs

## `public static class LCatalogTag`

The orderings the tag catalog is listed in, and what counts as a matching tag.
A tag is a word and nothing else, so both answers are about that one text.

## `public static IReadOnlyList<LTag> LCatalogTagSort(IReadOnlyList<LTag> tags, LCatalogOrder order)`

Orders the tags by their text, ascending under every ordering but the reverse one.
A tag is read as a word, so the comparison is the reader culture and not an ordinal one.

## `public static bool LCatalogTagMatch(LTag tag, string query)`

Whether one tag answers the query, over its text alone.
