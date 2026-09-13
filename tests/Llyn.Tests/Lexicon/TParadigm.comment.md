# TParadigm.cs

## `public sealed class TParadigm`

Covers what a language pack declares about inflection: which part specialises which, and which forms a part takes.
Each test writes a pack of its own into a temporary language folder and loads it.
So the vocabulary loader is read as it reads a real pack, and nothing shipped is touched.

## `public void SpeechPackLoad_PartNamingParent_ReadsParentCode()`

A part naming its parent carries that code, and one naming none or naming it badly carries `0`.

## `public void SpeechPackLoad_ParadigmDeclared_ReadsValuesInOrder()`

A paradigm keeps its values in the order the pack lists them, because that order is the display order.
The regular-form pattern is carried as written, and null when the pack states none.

## `public void SpeechPackLoad_ParadigmRowMalformed_SkipsRow()`

A row whose part or any value is not a positive integer is dropped, and its neighbours are kept.
A blank pattern is no pattern.

## `public void SpeechPackLoad_EnglishPack_DeclaresParadigms()`

The shipped English pack states parents and paradigms, so the loader is read against disk and not a fixture alone.

## Inline notes

### `private static LSpeechPack TParadigmPackLoad(string json)`

Writes one vocabulary file under the application's language folder and removes it after the load.
