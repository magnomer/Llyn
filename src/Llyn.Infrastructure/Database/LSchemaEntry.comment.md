# LSchemaEntry.cs

## `public static class LSchemaEntry`

Creates the Entry root and the rows an Entry owns directly.
Data-contract names (table and column identifiers) are persisted keys, so they stay lowercase and independent of code member names.

## `public static void LSchemaEntryCreate(SqliteConnection connection)`

The Entry root, its owned written forms and parts of speech, and the language-controlled POS display vocabulary.
Owned child rows carry an (entry_id, position) identity and cascade when their Entry is deleted.

A part_of_speech row says its part of speech one of two ways and never both.
value_id names a preset the language declares, whose display name lives in part_of_speech_value.
custom_name carries text the user typed that no preset names.
The CHECK keeps the two from drifting into a row that is half id, half name.
