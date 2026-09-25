# LMorphologyArchive.cs

## `public sealed class LMorphologyArchive`

Persists and resolves a language's morphology vocabulary.
A feature belongs to a part of speech and owns its values.
Inflections link a value row by id, and the value knows its feature.
Names live here and are never copied onto an entry's lexical rows.

## `public LMorphologyArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LFeature LFeatureCreate(LFeature feature)`

Adds or replaces one feature, keyed by `(speech_value_parent, pack_code)`, and returns it with its row id.
A pack that renames a feature keeps the row id.

## `public LMorphology LMorphologyCreate(LMorphology value)`

Adds or replaces one value, keyed by `(morphology_feature_parent, pack_code)`, and returns it with its row id.
A pack that renames a value keeps the row id, so every inflection that links it follows the rename.

## `public IReadOnlyList<LFeature> LFeatureRead(long speechValueId)`

Reads every feature the part of speech takes, in display order.

## `public LMorphology? LMorphologyRead(long id)`

Reads one value by row id, or `null` when no row has it.

## `public LMorphology? LMorphologyFind(long featureId, string name)`

Resolves the value `name` names under the feature, or `null` when none does.
Matching is a trimmed, case-insensitive comparison of the display name.

## `public LMorphology? LMorphologyCodeFind(string language, long speechCode, long featureCode, long code)`

Resolves the value a language pack names by `code` under its declared feature and part.
Returns `null` when the language holds none.
A value code is only unique within its feature, so the feature and part codes bind the row exactly.
The join runs from the value through its feature to the part of speech.
The pack code is matched at each step.
