# LCatalogTag.cs
Hash: `17fde97cf17d2843`

## `public sealed record LCatalogTag(LTag LCatalogTagStored, bool LCatalogTagChosen = false)`

One Tag as a browsed row, and the home of the orderings and the match the tag catalog uses.
A tag is a word and nothing else, so both answers are about that one text.

**Parameters**

- `LCatalogTagStored` — The stored Tag the row stands for.
- `LCatalogTagChosen` — True on the row of the Tag the vista stands on, false until the vista find fills it.

## `public static IReadOnlyList<LTag> LCatalogTagSort(IReadOnlyList<LTag> tags, LCatalogOrder order)`

Orders the tags by their text, ascending under every ordering but the reverse one.
A tag is read as a word, so the comparison is the reader culture and not an ordinal one.

## `public static bool LCatalogTagMatch(LTag tag, string query)`

Whether one tag answers the query, over its text alone.
