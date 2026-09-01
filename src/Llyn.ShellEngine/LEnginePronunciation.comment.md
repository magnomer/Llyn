# LEnginePronunciation.cs

## `public sealed partial class LEngine`

The pronunciation half of the engine as stored data — the row an Entry keeps, its syllables and representations, and the audio file hanging from it. This is the stored side; finding a pronunciation or a recording on the web is the lookup side, and the two meet only where a chosen recording is downloaded and then saved here.

An Entry keeps at most one pronunciation, so it is read by the Entry it belongs to and there is nothing to order. The audio row hangs off the pronunciation rather than the Entry, which is why a recording saved with no typed IPA still needs a pronunciation to hang from.

A file is stored workspace-relative and handed out full, the same way a loaded draft's audio is: the shell plays a path and never has to know the workspace folder, and an entry opened from a moved workspace still resolves to a file that is there. A path outside the workspace has no relative form and is kept as it stands.

## `public LPronunciation LEnginePronunciationCreate(LPronunciation pronunciation)`

Creates `pronunciation` with its syllables and representations as its ordered child rows and returns it with its assigned id.

## `public LPronunciation? LEnginePronunciationRead(string entryId)`

Reads the pronunciation the Entry identified by `entryId` keeps, with its syllables and representations, or `null` when it keeps none.

## `public void LEnginePronunciationUpdate(LPronunciation pronunciation)`

Rewrites the pronunciation `pronunciation` identifies, its syllables and its representations. The audio hanging from it is untouched: its id does not change, so the recording stays attached across an edit of the IPA.

## `public void LEnginePronunciationDelete(string id)`

Deletes the pronunciation identified by `id` with its syllables, representations and audio row. The file on disk is not removed — the workspace owns it, and another entry may have been given the same recording.

## `public void LEngineAudioSave(string pronunciationId, string file, string? source)`

Saves `file` as the audio of the pronunciation identified by `pronunciationId`, replacing whatever it played before, and records the `source` the recording came from. The path is stored relative to the workspace when it lies inside it, so a moved workspace keeps its audio.

## `public LPronunciationAudio? LEngineAudioRead(string pronunciationId)`

Reads the audio of the pronunciation identified by `pronunciationId` with its file resolved to a full path in the workspace in use now, or `null` when it has none.

## `public void LEngineNoteSave(LNote note)`

Saves `note` as the note of its Entry, replacing the one there — an Entry keeps at most one note, keyed by the Entry itself, so a save is a create or a rewrite and the caller need not know which.

## `public LNote? LEngineNoteRead(string entryId)`

Reads the note the Entry identified by `entryId` keeps, or `null` when it keeps none.

## `public void LEngineNoteDelete(string entryId)`

Deletes the note of the Entry identified by `entryId`, if it has one.
