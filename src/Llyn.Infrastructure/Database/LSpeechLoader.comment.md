# LSpeechLoader.cs

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

## Inline notes

### `private static LSpeechPack LSpeechPackRead(string language, JsonElement root)`

The pack as records with row id `0` and parent links holding codes.
Each part of speech takes its display order from its place in the list.
Each feature takes its order from its place among the features of the same part.
Each value takes its order from its place among the values of the same feature.
So the file states order by listing rather than by numbering it.

### `private static IEnumerable<JsonElement> LSpeechRowRead(JsonElement root, string name)`

The rows of one list, or none when the property is absent or not an array.

### `private static long? LSpeechNumberRead(JsonElement element, string name)`

One declared positive integer, or null when the property is absent, not a number, or not positive.
A code must be positive because user-added values take the negative range.

### `private static string? LSpeechTextRead(JsonElement element, string name)`

One declared string, or null when the property is absent, not a string, or blank.
A row with a blank name would be a vocabulary entry nothing can show.
