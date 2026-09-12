# TPronunciation.cs

## `public sealed class TPronunciation`

Covers the engine's stored-pronunciation seams.
That is the ordered pronunciation rows an Entry keeps, the first being the primary one.
It is also the audio hanging from each, stored workspace-relative and handed back resolved.
It is also the single note an Entry keeps beside them.
It also covers the draft requests that edit the primary row alone or any row by id.

## Inline notes

### `Assert.Equal(`

Stored relative, so a moved workspace keeps its audio.
Handed back full, so the shell plays a path without knowing where the workspace is.

### `engine.LEnginePronunciationDelete(pronunciation.LPronunciationId);`

The pronunciation going takes the audio row with it.
The file on disk is the workspace's.

### `public void EntryUpdate_ReorderedPronunciations_KeepsIdsAndAudio()`

A row keeps its id across a save.
The recording hanging from it therefore stays attached when the list is reordered.
The first row after the save is the one every summary shows.

### `private static string TPronunciationFileRead(TWorkspace workspace)`

The audio path as it sits in the table, which is the point of the test.
The stored form is relative.
Only the seam turns it into a path the shell can play.
