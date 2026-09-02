# LMorphologyArchive.cs

## `public sealed class LMorphologyArchive`

Persists and resolves the language-controlled morphology display vocabulary.
Inflections store only stable ids.
The feature and value display names for a language live here.
They are resolved by `(language, part_of_speech_id, feature_id, value_id)`.
So a name is never copied onto an entry's lexical rows.

## `public LMorphologyArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public void LMorphologyCreate(LMorphology morphology)`

Adds or replaces one vocabulary row, keyed by `(language, part_of_speech_id, feature_id, value_id)`.
It carries its feature and value display names and display order.

## `public LMorphology? LMorphologyRead(string language, string speechId, string featureId, string valueId)`

Resolves the vocabulary row for `language`, `speechId`, `featureId`, and `valueId`, or `null` when the vocabulary has no such row.
