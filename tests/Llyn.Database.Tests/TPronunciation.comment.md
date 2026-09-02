# TPronunciation.cs

## `public sealed class TPronunciation`

Covers the engine's stored-pronunciation seams.
That is the pronunciation row an Entry keeps.
It is also the audio hanging from it, stored workspace-relative and handed back resolved.
It is also the single note an Entry keeps beside them.

## Inline notes

### `Assert.Equal(`

Stored relative, so a moved workspace keeps its audio.
Handed back full, so the shell plays a path without knowing where the workspace is.

### `engine.LEnginePronunciationDelete(pronunciation.LPronunciationId);`

The pronunciation going takes the audio row with it.
The file on disk is the workspace's.

### `private static string TPronunciationFileRead(TWorkspace workspace)`

The audio path as it sits in the table, which is the point of the test.
The stored form is relative.
Only the seam turns it into a path the shell can play.
