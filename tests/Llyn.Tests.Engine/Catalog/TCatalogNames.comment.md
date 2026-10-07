# TCatalogNames.cs
Hash: `7662272cb1d99b92`

## `public sealed class TCatalogNames`

Duplicate headwords receive numbered display names in markup and incoming-use rows.
Each result preserves the stored headword, so display labels do not alter domain text.

## `public void MarkupRead_DuplicateHeadwords_CarriesSeparateDisplayNames()`

Reading duplicate markup entries assigns distinct display names.
Neither imported headword is rewritten.

## `public void MarkupRead_HeadwordAcrossLanguages_KeepsBareNames()`

One headword imported in English and in French keeps a bare display name in both rows.

## `public void IncomingRead_DuplicateHeadwords_CarriesTwinnedNames()`

Incoming-use rows distinguish source entries with the same headword.
Both rows preserve their common stored text.

## `public void IncomingRead_HeadwordAcrossLanguages_KeepsBareNames()`

Incoming-use rows from one headword in English and in French stay unnumbered.
