# TVaultFakeTranscription.cs
Hash: `76c3478e6a91afcf`

## `internal sealed class TVaultFakeTranscription : LTranscriptionVault`

An in-memory transcription store, so an entry clerk save can run its transcription sync without SQLite.
Rows are kept per entry and a set replaces the list, numbering the rows from one.
