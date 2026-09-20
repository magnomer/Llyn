# LTranscriptionVault.cs

## `public interface LTranscriptionVault`

The persistence port for the Transcription rows the engine reads and writes.
It lists exactly what the engine asks of transcription storage, and nothing about how rows are kept.
`LTranscriptionArchive` in Infrastructure is its adapter over the workspace database.

## `IReadOnlyList<LTranscription> LTranscriptionRead(long entryId);`

Reads the entry's transcriptions in position order, empty when it has none.

## `IReadOnlyList<LTranscription> LTranscriptionSet(long entryId, IReadOnlyList<LTranscription> transcriptions);`

Makes `transcriptions` the whole list of the entry, in the order given.
A row with a positive id is rewritten in place under that id, so the id survives the save.
A row with no id is inserted and given one.
A stored row the list no longer names is deleted.
Returns the stored rows with their ids and positions filled in.
The whole write is one transaction.
It throws when a positive id names no row of this entry, and when two rows share a scheme.
