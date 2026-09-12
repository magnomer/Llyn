# LEnginePronunciation.cs

## `public sealed partial class LEngine`

The pronunciation half of the engine as stored data.
That is the rows an Entry keeps, their syllables, and their audio files.
This is the stored side.
Finding a pronunciation or a recording on the web is the lookup side.
The two meet only where a chosen recording is downloaded and then saved here.

An Entry keeps an ordered list of pronunciations, the first being the primary one.
They are read by the Entry they belong to, and a save of the Entry's draft places them.
The audio row hangs off one pronunciation rather than the Entry.
So a recording saved with no typed IPA still needs a pronunciation to hang from.

A file is stored workspace-relative and handed out full.
That is the same way a loaded draft's audio is handled.
The shell plays a path and never has to know the workspace folder.
An entry opened from a moved workspace still resolves to a file that is there.
A path outside the workspace has no relative form and is kept as it stands.

## `public LPronunciation LEnginePronunciationCreate(LPronunciation pronunciation)`

Creates `pronunciation` after the Entry's others, with its syllables as its ordered child rows, and returns it with its assigned id and position.

## `public IReadOnlyList<LPronunciation> LEnginePronunciationRead(long entryId)`

Reads the pronunciations the Entry identified by `entryId` keeps, in order, each with its syllables; empty when it keeps none.

## `public void LEnginePronunciationUpdate(LPronunciation pronunciation)`

Rewrites the variety, the IPA and the syllables of the pronunciation `pronunciation` identifies.
The audio hanging from it and its place in the list are untouched.
Its id does not change, so the recording stays attached across an edit of the IPA.

## `public void LEnginePronunciationDelete(long id)`

Deletes the pronunciation identified by `id` with its syllables and audio row.
The file on disk is not removed.
The workspace owns it, and another entry may have been given the same recording.

## `public void LEngineAudioSave(long pronunciationId, string file, string? source)`

Saves `file` as the audio of the pronunciation identified by `pronunciationId`.
It replaces whatever it played before.
It records the `source` the recording came from.
The path is stored relative to the workspace when it lies inside it, so a moved workspace keeps its audio.

## `public LPronunciationAudio? LEngineAudioRead(long pronunciationId)`

Reads the audio of the pronunciation identified by `pronunciationId`.
Its file is resolved to a full path in the workspace in use now.
It returns `null` when it has none.

## `public void LEngineNoteSave(LNote note)`

Saves `note` as the note of its Entry, replacing the one there.
The text is Markdown and is normalized by `LMarkdown` before it is written.
An Entry keeps at most one note, keyed by the Entry itself.
So a save is a create or a rewrite, and the caller need not know which.

## `public LNote? LEngineNoteRead(long entryId)`

Reads the note the Entry identified by `entryId` keeps, or `null` when it keeps none.

## `public void LEngineNoteDelete(long entryId)`

Deletes the note of the Entry identified by `entryId`, if it has one.

## `public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(string query, LCatalogOrder order)`

The entries answering `query`, in `order`, each already carrying the primary pronunciation stored for it.
The sound is read here because the phonology catalog orders by it and shows it.
