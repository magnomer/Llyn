# LSpeechPack.cs
Hash: `1b09958a138f4949`

## `public sealed record LSpeechPack(IReadOnlyList<LSpeechValue> LSpeechPackValues, IReadOnlyList<LFeature> LSpeechPackFeatures, IReadOnlyList<LMorphology> LSpeechPackMorphology, IReadOnlyList<LParadigm> LSpeechPackParadigms, IReadOnlyList<LSpeechRetirement>? LSpeechPackRetirements = null)`

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
- `LSpeechPackRetirements` — The removed parts, in the order the pack lists them, empty when it lists none.

## `public IReadOnlyList<LSpeechRetirement> LSpeechPackRetirements { get; init; }`

The removed parts the pack lists, never null, so the import walks them without a check.

## `public IReadOnlyList<LSpeechValue> LSpeechPackSort(IReadOnlyList<LSpeechValue> rows)`

The one rule that orders a language's parts of speech for every view.
Values the pack declares come first, in the order the pack lists them.
Values the user typed follow, alphabetical in the current culture without case.
A stored position or row id never decides the order, because import and typing fill them by arrival.
The row id breaks only a tie between identical names, so the result is stable.
A stored value the pack no longer lists sorts with the typed ones, since nothing declares its place.

## `private int LSpeechPackFind(long code)`

The index of `code` in the pack's list, or `int.MaxValue` when the pack does not declare it.
A code of zero or less is never a pack code, since a typed value takes a negative one.
