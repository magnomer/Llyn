# TCatalogNames.cs

## `public sealed class TCatalogNames`

Duplicate headwords receive numbered display names in prospect, markup, and incoming-use rows.
Each result preserves the stored headword, so display labels do not alter domain text.

## `ProspectFind_DuplicateHeadwords_CarriesNamesWithoutChangingStoredText()`

Two identical saved headwords appear as numbered row names.
Both rows retain the original headword.

## `MarkupRead_DuplicateHeadwords_CarriesSeparateDisplayNames()`

Reading duplicate markup entries assigns distinct display names.
Neither imported headword is rewritten.

## `IncomingRead_DuplicateHeadwords_CarriesTwinnedNames()`

Incoming-use rows distinguish source entries with the same headword.
Both rows preserve their common stored text.
