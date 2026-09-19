# LPronunciationVault.cs

## `public interface LPronunciationVault`

The persistence port for the Pronunciation rows the engine reads and writes.
It lists exactly what the engine asks of pronunciation storage, and nothing about how rows are kept.
`LPronunciationArchive` in Infrastructure is its adapter over the workspace database.

## `LPronunciation LPronunciationCreate(LPronunciation pronunciation);`

Inserts `pronunciation` with a fresh opaque id, placed after the entry's other pronunciations.
The position it carries is ignored, because the entry's order is settled by `LPronunciationOrderSet`.
A blank variety, a blank IPA or a blank respelling is stored as NULL.
It writes its syllables as ordered child rows.
Their pronunciation id and position are assigned from the new id and list order.
Returns the stored pronunciation with its id, position and children filled in.
The whole write is one transaction.

## `IReadOnlyList<LPronunciation> LPronunciationRead(long entryId);`

Reads the entry's pronunciations in position order, each with its ordered syllables, empty when the entry has none.

## `void LPronunciationUpdate(LPronunciation pronunciation);`

Replaces the variety, the IPA, the respelling and the syllables of the pronunciation identified by `pronunciation`'s id.
Existing syllable rows are cleared and the supplied list written in order.
So the pronunciation id, entry link, position and identity stay fixed.
The whole write is one transaction, and it throws when no pronunciation carries that id.

## `void LPronunciationOrderSet(long entryId, IReadOnlyList<long> order);`

Renumbers the entry's pronunciations so that `order` is their position order.
The first id becomes the primary pronunciation.

## `void LPronunciationDelete(long id);`

Deletes the pronunciation identified by `id` and closes the gap it leaves in its entry's order.
Its syllables and its recording are removed by the foreign-key cascade.

## `void LPronunciationAudioSave(long pronunciationId, string file, string? source);`

Records `file`, a path relative to the workspace folder, as the recording the pronunciation owns.
It records the `source` label it was downloaded from and the moment it was stored.
A pronunciation carries at most one recording.
So a second save for the same pronunciation replaces the first rather than adding a row.

## `LPronunciationAudio? LPronunciationAudioRead(long pronunciationId);`

Reads the recording the pronunciation owns, or `null` when it has none.
The file path comes back exactly as stored, relative to the workspace folder.
So the caller that knows the workspace resolves it against the folder in use now.

## `IReadOnlyList<string> LPronunciationAudioScan();`

Every recording file the database names, as stored, in one statement.
The sweep that drops orphaned recordings reads the kept set through it.

## `long? LPronunciationHolderRead(long id);`

Which entry holds the pronunciation, or nothing when no row carries the id.
The engine uses it to stamp the entry when a pronunciation or its recording changes on its own.
