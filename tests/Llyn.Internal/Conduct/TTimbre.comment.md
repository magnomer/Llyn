# TTimbre.cs

## `public sealed class TTimbre`

Covers the sound facts an editor reads for its held draft, over a fake phonology port.
A phonemic respelling needs a respelling pack first, and a silent pack is not spoken.
An empty desk asks the pack for no language.
The playback read and the play gate run over a held draft, a real engine and real workspace files.

## `private static CTimbre TTimbrePrepare(Dictionary<string, Func<object?[]?, object?>> answers)`

Builds an editor over stub ports and a phonology port answering `answers`, and hands back its sound facts.

## `private static string TTimbreFileSave(TWorkspace workspace, string name)`

Writes a one-byte recording under the workspace's audio folder and hands back its path.
