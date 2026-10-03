# TCatalogNames.cs
Hash: `5411a4197730731b`

## `public sealed class TCatalogNames`

Duplicate headwords receive numbered display names in markup and incoming-use rows.
Each result preserves the stored headword, so display labels do not alter domain text.

## `public void MarkupRead_DuplicateHeadwords_CarriesSeparateDisplayNames()`

Reading duplicate markup entries assigns distinct display names.
Neither imported headword is rewritten.

## `public void IncomingRead_DuplicateHeadwords_CarriesTwinnedNames()`

Incoming-use rows distinguish source entries with the same headword.
Both rows preserve their common stored text.
