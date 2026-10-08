# TPlayback.cs
Hash: `1256fdde401fcd82`

## `public sealed class TPlayback`

Covers the playback gate an editor holds, over a held draft, a real engine and real workspace files.
The read answers the draft's own recording while its file exists, and whether any row is audible.
A play press answers the file's address, and a file gone answers nothing.
A gone accent file is also cleared off its row.
An empty desk reads no audio and plays nothing.

## `private static string TPlaybackFileSave(TWorkspace workspace, string name)`

Writes a one-byte recording under the workspace's audio folder and hands back its path.

## `private static CPlayback TPlaybackPrepare()`

Builds an editor over stub ports and hands back its playback gate, whose desk holds nothing.
