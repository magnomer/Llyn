# LSchemaInflection.cs

## `public static class LSchemaInflection`

Creates the inflected forms an Entry owns and the vocabulary they read their features from.

## `public static void LSchemaInflectionCreate(SqliteConnection connection)`

Entry-owned inflected forms and their ordered grammatical features, plus the language-controlled morphology display vocabulary.
A feature's identity is the two-level owned key (entry_id, inflection_position, position).
It cascades when its inflection is deleted.
An inflection cascades when its Entry is deleted.
Lexical rows store only stable ids, and display names live in morphology_value.
