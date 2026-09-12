# LSchemaPronunciation.cs

## `public static class LSchemaPronunciation`

Creates the pronunciations an Entry owns, their syllables and their recordings.

## `public static void LSchemaPronunciationCreate(SqliteConnection connection)`

An entry carries an ordered list of pronunciations, keyed by its own id and placed by (entry_parent, position).
The first row is the primary one every summary shows.
Its id counts strictly upward and is never given to a later pronunciation once the row is gone.
A revision names a deleted pronunciation by that id.
A reused number would point the history at a living row.
A row may carry no ipa yet, because a recording is often fetched before the reading is typed.
The variety says why the row stands beside the others, such as a region.
It is NULL when there is one row.
A pronunciation owns ordered syllables keyed by (pronunciation_parent, position).
A syllable requires only its nucleus.
Its columns are the segments and the tone, never a spelling.
Every other syllable field is optional.
They store NULL when absent, which is distinct from empty.
The pronunciation also owns at most one downloaded recording.
pronunciation_audio has no id of its own, since pronunciation_parent is its primary key.
Its file is stored relative to the workspace folder.
So a moved or copied workspace keeps its audio.
Children cascade when their pronunciation is deleted, and the pronunciation cascades when its entry is deleted.
