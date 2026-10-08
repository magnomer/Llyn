# LEntryLoaderReading.cs
Hash: `d637332d8f11d2c5`

## `public sealed class LEntryLoaderReading`

Reads how an entry is said and written back into the values its draft carries.
That is its pronunciations with their recordings, its transcriptions and its reflexes.
`LEntryLoader` builds it inside its own session, so these reads join one snapshot.

## `public LEntryLoaderReading(LDatabase database)`

Binds the loader to the workspace `database` it reads through.

## `public IReadOnlyList<LPronunciationDraft> LEntrySoundRead(long id)`

Every stored pronunciation of the entry and its recording, in stored order, as the values the draft carries.
An entry with no pronunciation row carries an empty list.
Each row's id travels with it, so a save updates that row instead of writing another.
The audio's added time stays behind, because it is workspace bookkeeping.

## `public IReadOnlyList<LTranscriptionDraft> LEntrySpellingRead(long id)`

Every stored transcription of the entry, in stored order, each with its scheme and its id.

## `public IReadOnlyList<LReflexDraft> LEntryReflexRead(long id)`

Every stored reflex of the entry, in stored order, each with its id.
