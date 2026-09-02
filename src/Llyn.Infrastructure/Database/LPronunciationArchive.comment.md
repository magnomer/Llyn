# LPronunciationArchive.cs

## `public sealed class LPronunciationArchive`

Persists the single pronunciation an entry owns, together with its ordered syllables and representations.
A pronunciation's id is assigned here on creation.
Its children are written as ordered rows.
Their `pronunciation_id` and `position` come from that id and list order.
So reordering rewrites positions only.
One pronunciation per entry is enforced by the unique `entry_id` column.
A second create for the same entry fails at the database.
Reading returns the whole aggregate by entry id.
Updating replaces the level, IPA, and both child lists.
Deleting removes the syllables and representations through the foreign-key cascade.

A pronunciation also owns at most one downloaded recording, kept beside the aggregate rather than inside it.
`LPronunciationAudioSave` and `LPronunciationAudioRead` write and read that one row.
Its file path is stored relative to the workspace folder.

## `public LPronunciationArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LPronunciation LPronunciationCreate(LPronunciation pronunciation)`

Inserts `pronunciation` with a fresh opaque id.
It writes its syllables and representations as ordered child rows.
Their pronunciation id and position are assigned from the new id and list order.
Returns the stored pronunciation with its id and children filled in.
The whole write is one transaction.
The entry's unique constraint rejects a second pronunciation for the same entry.

## `public LPronunciation? LPronunciationRead(string entryId)`

Reads the entry's pronunciation with its ordered syllables and representations, or `null` when the entry has none.

## `public void LPronunciationUpdate(LPronunciation pronunciation)`

Replaces the level, IPA, and both child lists of the pronunciation identified by `pronunciation`'s id.
Existing syllable and representation rows are cleared and the supplied lists written in order.
So the pronunciation id, entry link, and identity stay fixed.
The whole write is one transaction, and it throws when no pronunciation carries that id.

## `public void LPronunciationDelete(string id)`

Deletes the pronunciation identified by `id`.
Its syllables and representations are removed by the foreign-key cascade.

## `public void LPronunciationAudioSave(string pronunciationId, string file, string? source)`

Records `file`, a path relative to the workspace folder, as the recording the pronunciation owns.
It records the `source` label it was downloaded from and the moment it was stored.
A pronunciation carries at most one recording.
So a second save for the same pronunciation replaces the first rather than adding a row.

## `public LPronunciationAudio? LPronunciationAudioRead(string pronunciationId)`

Reads the recording the pronunciation owns, or `null` when it has none.
The file path comes back exactly as stored, relative to the workspace folder.
So the caller that knows the workspace resolves it against the folder in use now.
