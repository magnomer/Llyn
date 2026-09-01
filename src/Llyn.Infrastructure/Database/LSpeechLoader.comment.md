# LSpeechLoader.cs

## `public static class LSpeechLoader`

Loads a language's display vocabulary from `languages//vocabulary.json`, resolved against the application's base directory the same way `LLanguageLoader` resolves a pronunciation pack. The parts of speech a language uses and the morphology each of them takes are language-specific facts, so they are data in the pack, never names compiled into the store: adding a language needs a folder, not a recompile.

A missing or malformed file yields an empty vocabulary rather than throwing. A language may legitimately declare no morphology at all — an isolating language has none to declare — so an empty result is an answer, not a failure, and one unreadable pack never stops the others from loading.

## `public static LSpeechPack LSpeechLoaderLoad(string language)`

Reads the vocabulary `language` declares, or an empty one when the pack declares none.

## Inline notes

### `private static LSpeechPack LSpeechPackRead(string language, JsonElement root)`

The pack as records: each part of speech takes its display order from its place in the list, and each morphology value takes its order from its place among the values of the same feature, so the file states order by listing rather than by numbering it.

### `private static string? LSpeechTextRead(JsonElement element, string name)`

One declared string, or null when the property is absent, not a string, or blank — a row keyed by a blank id would be a vocabulary entry nothing can resolve.
