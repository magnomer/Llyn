# LTranscriptionArchive.cs

## `public sealed class LTranscriptionArchive`

Persists the ordered transcriptions an entry owns, one row per scheme.
A transcription's id is assigned here on insertion and survives every later save.
So a draft and a shell may hold that id across saves.
The whole list of an entry is written at once.
The rows are few, and their order and schemes are one fact.

## `public LTranscriptionArchive(LDatabase database)`

Binds the store to the workspace `database` it opens sessions through.

## `public IReadOnlyList<LTranscription> LTranscriptionRead(long entryId)`

Reads the entry's transcriptions in position order, empty when it has none.

## `public IReadOnlyList<LTranscription> LTranscriptionSet(long entryId, IReadOnlyList<LTranscription> transcriptions)`

Makes `transcriptions` the whole list of the entry, in the order given.
A row with a positive id is rewritten in place under that id, so the id survives the save.
A row with no id is inserted and given one.
A stored row the list no longer names is deleted.
Returns the stored rows with their ids and positions filled in.
The whole write is one transaction.
It throws when a positive id names no row of this entry, and when two rows share a scheme.

## Inline notes

### `private static void LTranscriptionAsideMove(SqliteConnection connection, long entryId)`

Every surviving row is moved out of the way before the list is written back.
Its position is shifted far up and its scheme is prefixed with a character no scheme name carries.
Without this, two rows swapping schemes, or a row moving onto another's position, would hit the unique constraints mid-write.
Every shelved row is rewritten by the loop that follows, so nothing stays shelved.
