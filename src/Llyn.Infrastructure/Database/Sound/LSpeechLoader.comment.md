# LSpeechLoader.cs
Hash: `46d335516437555c`
Hash: `025348892f72e984`

## `public static class LSpeechLoader`

Loads a language's display vocabulary from `languages/<Name>/vocabulary.json`.
It is resolved against the application's base directory.
That is the same way `LLanguageLoader` resolves a pronunciation pack.
The parts of speech a language uses and the morphology each takes are language-specific facts.
So they are data in the pack, never names compiled into the store.
Adding a language needs a folder, not a recompile.

A missing or malformed file yields an empty vocabulary rather than throwing.
A language may legitimately declare no morphology at all.
An isolating language has none to declare.
So an empty result is an answer, not a failure.
One unknown pack never stops the others from loading.

## `public static LSpeechPack LSpeechLoaderLoad(string language)`

Reads the vocabulary `language` declares, or an empty one when the pack declares none.
A name `LLanguageNameValidate` refuses reads nothing.

## `private static LSpeechPack LSpeechPackRead(string language, JsonElement root)`

The pack as records with row id `0` and parent links holding codes.
Each part of speech takes its display order from its place in the list.
Each feature takes its order from its place among the features of the same part.
Each value takes its order from its place among the values of the same feature.
So the file states order by listing rather than by numbering it.
A part may name the part it specialises, and a paradigm names its part, both by code.
A paradigm row is skipped when its part or any of its values is not a positive integer.
A row may state `values`, `cells` or both, and a row with neither is skipped.
Each value becomes a cell of one value, listed before the stated cells.
A present but malformed `values` or `cells` skips the row.
A paradigm may except parts by code and may carry regular-form rules as pattern and replacement pairs.

## `private static IEnumerable<JsonElement> LSpeechRowRead(JsonElement root, string name)`

The rows of one list, or none when the property is absent or not an array.

## `private static long? LSpeechNumberRead(JsonElement element, string name)`

One declared positive integer, or null when the property is absent, not a number, or not positive.
A code must be positive because user-added values take the negative range.

## `internal static IReadOnlyList<long>? LSpeechNumbersRead(JsonElement element, string name)`

One declared list of positive integers, or null when the property is absent, not an array, or holds anything else.
A paradigm naming one value nothing can resolve is no paradigm at all.
The inflection loader reads its value lists with it too.

## `private static IReadOnlyList<LSpeechRetirement> LSpeechRetirementScan(JsonElement root)`

The `retired` rows, each with its code, its `into` target or `0`, and its `unit` key.
A row without a positive `id` is skipped.

## `private static IReadOnlyList<LParadigmRule> LSpeechRuleScan(JsonElement element, string name)`

The declared rules of a paradigm, each a two-string array, or none when the property is absent.
A row that is not such a pair or has an empty pattern is skipped.
So is one that does not parse as a regular expression.

## `internal static IReadOnlyList<IReadOnlyList<long>>? LSpeechCellsRead(JsonElement element, string name)`

The declared cells, each a non-empty list of positive integers, or null when anything else is found.
The values keep the order the pack lists them, since that order is the slot's display name.
The inflection loader reads its columns and line cells with it too.

## `private static string? LSpeechTextRead(JsonElement element, string name)`

One declared string, or null when the property is absent, not a string, or blank.
A row with a blank name would be a vocabulary entry nothing can show.
