# TPronunciation.cs

## `public sealed class TPronunciation`

Covers the engine's stored-pronunciation seams: the pronunciation row an Entry keeps, the audio hanging from it — stored workspace-relative and handed back resolved — and the single note an Entry keeps beside them.

## Inline notes

### `Assert.Equal(`

Stored relative, so a moved workspace keeps its audio; handed back full, so the shell plays a path without knowing where the workspace is.

### `engine.LEnginePronunciationDelete(pronunciation.LPronunciationId);`

The pronunciation going takes the audio row with it; the file on disk is the workspace's.

### `private static string TPronunciationFileRead(TWorkspace workspace)`

The audio path as it sits in the table, which is the point of the test: the stored form is relative, and only the seam turns it into a path the shell can play.
