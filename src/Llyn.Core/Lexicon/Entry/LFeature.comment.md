# LFeature.cs

## `public sealed record LFeature(`

One grammatical feature a part of speech takes (for example `number` on a noun).
It owns the `LMorphology` rows that name its values.
Inside a loaded `LSpeechPack` the parent link holds the parent's code, because the row id is unknown until import.

**Parameters**

- `LFeatureId` — Row id, `0` before the row is stored.
- `LFeatureSpeechId` — Part of speech the feature applies to.
- `LFeatureCode` — Number the pack file gave the feature, unique under its part of speech.
- `LFeatureName` — Display name for the language.
- `LFeaturePosition` — Display order within the part of speech.
