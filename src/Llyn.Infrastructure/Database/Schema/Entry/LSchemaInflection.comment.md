# LSchemaInflection.cs
Hash: `39ce71c62dea519e`
Hash: `8b2328fb63cfcfa0`

## `public static class LSchemaInflection`

Creates the inflected forms an Entry owns and the vocabulary they read their features from.

## `public static void LSchemaInflectionCreate(SqliteConnection connection)`

The morphology vocabulary, then the entry-owned inflected forms and their ordered features.
morphology_feature belongs to a speech_value row and morphology_value belongs to a feature.
Both have their own integer id and are unique on (parent, pack_code), so a re-seed upserts and keeps ids.

An inflection has its own id and is unique on (entry_parent, position).
It cascades when its Entry is deleted.
An inflection_feature links its inflection by id and cascades with it.
It stores only the morphology_value link, because the value knows its feature.
regular is one when the form is what the paradigm's pattern predicts from the headword, derived on store.
prediction, marks and stamp hold the analysis by the pack's rule book, also derived on store.
prediction is the predicted form, and marks the departing ranges of text as `offset:length` pairs joined by commas.
An empty marks string means covered with nothing marked.
stamp names the book that made the analysis, so a changed book is noticed and the row analysed again.
Uncovered analysis leaves prediction and marks NULL but stores the examining book's stamp.
The schema permits NULL analysis when no book has examined the row.
A row carried from an older version takes NULL in all three.
