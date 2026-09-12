# LSchemaTranscription.cs

## `public static class LSchemaTranscription`

Creates the transcriptions an Entry owns.

## `public static void LSchemaTranscriptionCreate(SqliteConnection connection)`

An entry carries an ordered list of transcriptions, keyed by its own id and placed by (entry_parent, position).
A transcription is the reading spelled in a named scheme, such as Jyutping, and is never IPA.
IPA lives in pronunciation, which says how the word sounds.
One scheme spells one reading one way, so (entry_parent, scheme) is unique.
The scheme name is stored on the row rather than in a lookup table.
So a row stays readable after its language pack drops or renames the scheme.
A row always carries text, because a blank transcription is no transcription.
The rows cascade when their entry is deleted.
