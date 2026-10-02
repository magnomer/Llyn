# TCatalogNames.cs
Hash: `7056f9ba4041e54f`

## `public sealed class TCatalogNames`

Duplicate headwords receive numbered display names in prospect, markup, and incoming-use rows.
Each result preserves the stored headword, so display labels do not alter domain text.

## `public void TranslationAdd_DuplicateHeadwords_CarriesNamesWithoutChangingStoredText()`

Two identical saved headwords appear as numbered row names in a typed translation's offer.
Both rows retain the original headword.

## `public void MarkupRead_DuplicateHeadwords_CarriesSeparateDisplayNames()`

Reading duplicate markup entries assigns distinct display names.
Neither imported headword is rewritten.

## `public void IncomingRead_DuplicateHeadwords_CarriesTwinnedNames()`

Incoming-use rows distinguish source entries with the same headword.
Both rows preserve their common stored text.
