# Sound

This folder holds the in-memory sound stores an entry clerk save runs over.

## `TVaultFakePronunciation`

An in-memory pronunciation store keyed from one, so a clerk save can run without SQLite.
Rows are kept by id and read back by entry, and the order set is accepted and ignored.
A recording save is refused, since no clerk test writes audio through it.

## `TVaultFakeReflex`

An in-memory reflex store, so an entry clerk save can run its reflex sync without SQLite.
Rows are kept per entry and a set replaces the list, numbering the rows from one.
The anchor scan answers nothing, since no clerk test tallies anchors.

## `TVaultFakeTranscription`

An in-memory transcription store, so an entry clerk save can run its transcription sync without SQLite.
Rows are kept per entry and a set replaces the list, numbering the rows from one.

## Stamp

Commit: `aae5349dbb94`
- `TVaultFakePronunciation.cs` `23fc443861d386b7`
- `TVaultFakeReflex.cs` `f5c05bb32d9f3a49`
- `TVaultFakeTranscription.cs` `76c3478e6a91afcf`
