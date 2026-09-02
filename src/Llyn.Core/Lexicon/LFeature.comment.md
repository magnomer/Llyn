# LFeature.cs

## `public sealed record LFeature(`

One grammatical feature carried by an inflection, ordered within it.
It stores only stable grammatical ids, a feature id and its value id, never display names.
The names are resolved from the language-controlled morphology vocabulary (`LMorphology`).
The feature's position and its parent inflection are given by its order within `LInflection.LInflectionFeatures`.

**Parameters**

- `LFeatureId` — Stable grammatical feature id (for example `number`).
- `LFeatureValueId` — Stable grammatical value id (for example `plural`).
