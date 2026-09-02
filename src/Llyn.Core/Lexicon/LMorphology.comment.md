# LMorphology.cs

## `public sealed record LMorphology(`

One row of the language-controlled morphology display vocabulary.
It maps a stable `(part_of_speech id, feature id, value id)` triple to display names.
Those are the feature and value names shown for a given language.
Inflections store only the ids (`LFeature`).
The names live here and are resolved by `(language, part_of_speech_id, feature_id, value_id)`.
They are never copied onto an entry's lexical rows.

**Parameters**

- `LMorphologyLanguage` — Language the display names are governed by.
- `LMorphologySpeechId` — Stable part-of-speech id the feature applies to.
- `LMorphologyFeatureId` — Stable grammatical feature id (for example `number`).
- `LMorphologyFeatureName` — Feature display name for the language (for example `Number`).
- `LMorphologyValueId` — Stable grammatical value id (for example `plural`).
- `LMorphologyValueName` — Value display name for the language (for example `Plural`).
- `LMorphologyPosition` — Display order within the language's vocabulary.
