# LMorphologyArchive.cs

## `public sealed class LMorphologyArchive`

Persists and resolves a language's morphology vocabulary.
A feature belongs to a part of speech and owns its values.
Inflections link a value row by id, and the value knows its feature.
Names live here and are never copied onto an entry's lexical rows.

## `public LMorphologyArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LFeature LFeatureCreate(LFeature feature)`

Adds or replaces one feature, keyed by `(speech_value_id, pack_ref)`, and returns it with its row id.
A pack that renames a feature keeps the row id.

## `public LMorphology LMorphologyCreate(LMorphology value)`

Adds or replaces one value, keyed by `(morphology_feature_id, pack_ref)`, and returns it with its row id.
A pack that renames a value keeps the row id, so every inflection that links it follows the rename.

## `public IReadOnlyList<LFeature> LFeatureRead(long speechValueId)`

Reads every feature the part of speech takes, in display order.

## `public LFeature? LFeatureFind(long speechValueId, string name)`

Resolves the feature `name` names under the part of speech, or `null` when none does.
Matching is a trimmed, case-insensitive comparison of the display name.

## `public LMorphology? LMorphologyRead(long id)`

Reads one value by row id, or `null` when no row has it.

## `public IReadOnlyList<LMorphology> LMorphologyScan(long featureId)`

Reads every value the feature takes, in display order.

## `public LMorphology? LMorphologyFind(long featureId, string name)`

Resolves the value `name` names under the feature, or `null` when none does.
Matching is a trimmed, case-insensitive comparison of the display name.
