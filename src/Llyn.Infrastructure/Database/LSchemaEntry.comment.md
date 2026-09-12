# LSchemaEntry.cs

## `public static class LSchemaEntry`

Creates the Entry root and the rows an Entry owns directly.
Data-contract names (table and column identifiers) are persisted keys, so they stay lowercase and independent of code member names.

## `public static void LSchemaEntryCreate(SqliteConnection connection)`

The Entry root, its owned written forms and parts of speech, and the language's part-of-speech vocabulary.
Owned child rows carry an (entry_id, position) identity and cascade when their Entry is deleted.

speech_value has its own integer id and is unique on (language, pack_ref).
pack_ref is the id the pack file gave the value.
So a re-seed upserts, and a rename keeps the row id.

A part_of_speech row says its part of speech one of two ways and never both.
speech_value_ref links a value the language declares, whose name lives on that row.
custom_name carries text the user typed that no value names.
The CHECK keeps the two from drifting into a row that is half link, half name.
