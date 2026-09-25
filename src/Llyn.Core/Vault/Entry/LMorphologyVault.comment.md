# LMorphologyVault.cs

## `public interface LMorphologyVault`

The persistence port for the Morphology rows the engine reads and writes.
It lists exactly what the engine asks of morphology storage, and nothing about how rows are kept.
`LMorphologyArchive` in Infrastructure is its adapter over the workspace database.

## `LFeature LFeatureCreate(LFeature feature);`

Adds or replaces one feature, keyed by `(speech_value_parent, pack_code)`, and returns it with its row id.
A pack that renames a feature keeps the row id.

## `LMorphology LMorphologyCreate(LMorphology value);`

Adds or replaces one value, keyed by `(morphology_feature_parent, pack_code)`, and returns it with its row id.
A pack that renames a value keeps the row id, so every inflection that links it follows the rename.

## `IReadOnlyList<LFeature> LFeatureRead(long speechValueId);`

Reads every feature the part of speech takes, in display order.

## `LMorphology? LMorphologyRead(long id);`

Reads one value by row id, or `null` when no row has it.

## `LMorphology? LMorphologyFind(long featureId, string name);`

Resolves the value `name` names under the feature, or `null` when none does.
Matching is a trimmed, case-insensitive comparison of the display name.

## `LMorphology? LMorphologyCodeFind(string language, long speechCode, long featureCode, long code);`

Resolves the value a language pack names by `code` under its declared feature and part.
Returns `null` when the language holds none.
A value code is only unique within its feature, so the feature and part codes bind the row exactly.
The join runs from the value through its feature to the part of speech.
The pack code is matched at each step.
