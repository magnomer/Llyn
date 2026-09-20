# LSpeechPack.cs

## `public sealed record LSpeechPack(`

The display vocabulary one language pack declares.
It lists the parts of speech, the features each takes, and the values each feature takes.
Loaded from `languages/<Name>/vocabulary.json` and upserted into `speech_value`, `morphology_feature`, and `morphology_value`.
So a pack is the only place a language's vocabulary is stated, and no such fact is compiled in.

Rows in a pack have row id `0`.
Their parent links hold the parent's code, and the import resolves those to row ids.

**Parameters**

- `LSpeechPackValues` — The parts of speech, in the order the pack lists them.
- `LSpeechPackFeatures` — The features, in the order the pack lists them.
- `LSpeechPackMorphology` — The feature values, in the order the pack lists them.
- `LSpeechPackParadigms` — The paradigms, in the order the pack lists them, each naming its part by code.
