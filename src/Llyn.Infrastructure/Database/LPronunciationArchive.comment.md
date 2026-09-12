# LPronunciationArchive.cs

## `public sealed class LPronunciationArchive`

Persists the ordered pronunciations an entry owns, each with its ordered syllables.
A pronunciation's id is assigned here on creation.
Its children are written as ordered rows.
Their `pronunciation_id` and `position` come from that id and list order.
So reordering rewrites positions only.
A new pronunciation is appended after the entry's others, and `LPronunciationOrderSet` places the whole list.
Reading returns every pronunciation of an entry in order.
Updating replaces the variety, the IPA and the syllables of one row.
Deleting removes the syllables through the foreign-key cascade.

A pronunciation also owns at most one downloaded recording, kept beside the aggregate rather than inside it.
`LPronunciationAudioSave` and `LPronunciationAudioRead` write and read that one row.
Its file path is stored relative to the workspace folder.

## `public LPronunciationArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public LPronunciation LPronunciationCreate(LPronunciation pronunciation)`

Inserts `pronunciation` with a fresh opaque id, placed after the entry's other pronunciations.
The position it carries is ignored, because the entry's order is settled by `LPronunciationOrderSet`.
A blank variety or a blank IPA is stored as NULL.
It writes its syllables as ordered child rows.
Their pronunciation id and position are assigned from the new id and list order.
Returns the stored pronunciation with its id, position and children filled in.
The whole write is one transaction.

## `public IReadOnlyList<LPronunciation> LPronunciationRead(long entryId)`

Reads the entry's pronunciations in position order, each with its ordered syllables, empty when the entry has none.

## `public void LPronunciationUpdate(LPronunciation pronunciation)`

Replaces the variety, the IPA and the syllables of the pronunciation identified by `pronunciation`'s id.
Existing syllable rows are cleared and the supplied list written in order.
So the pronunciation id, entry link, position and identity stay fixed.
The whole write is one transaction, and it throws when no pronunciation carries that id.

## `public void LPronunciationOrderSet(long entryId, IReadOnlyList<long> order)`

Renumbers the entry's pronunciations so that `order` is their position order.
The first id becomes the primary pronunciation.

## `public void LPronunciationDelete(long id)`

Deletes the pronunciation identified by `id` and closes the gap it leaves in its entry's order.
Its syllables and its recording are removed by the foreign-key cascade.

## `public void LPronunciationAudioSave(long pronunciationId, string file, string? source)`

Records `file`, a path relative to the workspace folder, as the recording the pronunciation owns.
It records the `source` label it was downloaded from and the moment it was stored.
A pronunciation carries at most one recording.
So a second save for the same pronunciation replaces the first rather than adding a row.

## `public LPronunciationAudio? LPronunciationAudioRead(long pronunciationId)`

Reads the recording the pronunciation owns, or `null` when it has none.
The file path comes back exactly as stored, relative to the workspace folder.
So the caller that knows the workspace resolves it against the folder in use now.
