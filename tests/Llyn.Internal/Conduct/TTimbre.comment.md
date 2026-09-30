# TTimbre.cs

## `public sealed class TTimbre`

Covers the sound facts an editor reads for its held draft, over a fake phonology port.
A phonemic respelling needs a respelling pack first, and a silent pack is not spoken.
An empty desk asks the pack for no language.
The contour read draws only a tonal pack's toned reading, and its scale runs from one to five.
The playback read and the play gate run over a held draft, a real engine and real workspace files.
The accent sheet carries every row in the printed form the respelling switch picks.
An empty desk answers the mute sheet.
The flag read is covered by `TTimbreEnsign`.

## `internal static CEditor TTimbreFlaggedPrepare(LEngine engine, string language)`

An editor holding a draft in `language` whose primary reading carries the British variety.
`TTimbreEnsign` builds its flagged drafts here too.

## `internal static CTimbre TTimbrePrepare(Dictionary<string, Func<object?[]?, object?>> answers)`

Builds an editor over stub ports and a phonology port answering `answers`, and hands back its sound facts.
`TTimbreEnsign` builds its empty desk here too.

## `private static string TTimbreFileSave(TWorkspace workspace, string name)`

Writes a one-byte recording under the workspace's audio folder and hands back its path.
