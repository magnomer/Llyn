# LSchemaPronunciation.cs

## `public static class LSchemaPronunciation`

Creates the single pronunciation an Entry owns and its owned child structures.

## `public static void LSchemaPronunciationCreate(SqliteConnection connection)`

An entry carries at most one pronunciation, enforced by the unique entry_id.
The pronunciation itself carries no position.
It owns ordered syllables and representations keyed by (pronunciation_id, position).
A syllable requires only its nucleus.
Every other syllable field and a representation's local_tone are optional.
They store NULL when absent, which is distinct from empty.
The pronunciation also owns at most one downloaded recording.
pronunciation_audio has no id of its own, since pronunciation_id is its primary key.
Its file is stored relative to the workspace folder.
So a moved or copied workspace keeps its audio.
Children cascade when their pronunciation is deleted, and the pronunciation cascades when its entry is deleted.
