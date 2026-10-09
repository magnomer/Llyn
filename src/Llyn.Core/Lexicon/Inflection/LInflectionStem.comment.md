# LInflectionStem.cs
Hash: `bc8d0e33bd7da549`

## `public sealed record LInflectionStem(IReadOnlyList<long> LInflectionStemValues, IReadOnlyDictionary<string, string> LInflectionStemTemplates, IReadOnlyList<LInflectionEnding> LInflectionStemEndings)`

One paradigm row combines kind-specific stem templates with the endings of its covered columns.
The row's values plus one ending's values make the full cell the stem answers.
Prediction can use another matching kind's template when one matching kind lacks a template.

**Parameters**

- `LInflectionStemValues`: the pack value codes the row shares, such as mood and tense.
- `LInflectionStemTemplates`: replacement templates keyed by kind name, expanded against the headword.
- `LInflectionStemEndings`: the endings of the row's covered columns, with absent columns left out.
