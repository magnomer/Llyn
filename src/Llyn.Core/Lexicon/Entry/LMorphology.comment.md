# LMorphology.cs

## `public sealed record LMorphology(`

One value a grammatical feature can take (for example `plural` under `number`).
Inflections link it by `LMorphologyId` and never copy its name.
Inside a loaded `LSpeechPack` the parent link holds the parent's code, because the row id is unknown until import.

**Parameters**

- `LMorphologyId` — Row id, `0` before the row is stored.
- `LMorphologyFeatureId` — Feature the value belongs to.
- `LMorphologyCode` — Number the pack file gave the value, unique under its feature.
- `LMorphologyName` — Display name for the language.
- `LMorphologyPosition` — Display order within the feature.
