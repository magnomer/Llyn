# LSchemaInflection.cs

## `public static class LSchemaInflection`

Creates the inflected forms an Entry owns and the vocabulary they read their features from.

## `public static void LSchemaInflectionCreate(SqliteConnection connection)`

The morphology vocabulary, then the entry-owned inflected forms and their ordered features.
morphology_feature belongs to a speech_value row and morphology_value belongs to a feature.
Both have their own integer id and are unique on (parent, pack_ref), so a re-seed upserts and keeps ids.

An inflection has its own id and is unique on (entry_id, position).
It cascades when its Entry is deleted.
An inflection_feature links its inflection by id and cascades with it.
It stores only the morphology_value link, because the value knows its feature.
