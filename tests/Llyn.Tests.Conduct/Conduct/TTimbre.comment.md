# TTimbre.cs
Hash: `8f34b7a9feefbed0`

## `public sealed class TTimbre`

Covers the sound facts an editor reads for its held draft, over fake sound and settings ports or a workspace.
A phonemic respelling needs a respelling pack first, and a silent pack is not spoken.
An empty desk asks the pack for no language.
Its top level reads the top theme brush key, chosen in Conduct from the Core colour role.
The contour read draws only a tonal pack's toned reading, and its scale runs from five down to one.
Levels off the scale drop and the rest keep order and repeats, and a syllable left with none is untoned.
No syllables from the pack read none.
The accent sheet carries every row in the printed form the respelling switch picks.
An empty desk answers the mute sheet.
A typed accent answers the text the row now holds, and an empty desk takes nothing and answers empty text.
A desk filling its view takes nothing and answers the text its sheet holds for the row.
Adding a pronunciation places the blank row below its accent row, or first for the primary or a gone row.
Removing a pronunciation drops that accent row or the primary.
The font read answers the held draft's pack typography role by role, and an empty desk the blank font.
The flag read is covered by `TTimbreEnsign`, and the reflex block by `TTimbreReflex`.
The playback reads are covered by `TPlayback`.

## `internal static CEditor TTimbreAccentPrepare(LEngine engine, params string[] accents)`

An English editor holding a draft with a primary reading and one accent row per text in `accents`.
`TPlayback` builds its accent rows here too.

## `internal static CEditor TTimbreFlaggedPrepare(LEngine engine, string language)`

An editor holding a draft in `language` whose primary reading carries the British variety.
`TTimbreEnsign` builds its flagged drafts here too.

## `internal static CTimbre TTimbrePrepare(Dictionary<string, Func<object?[]?, object?>> answers)`

Builds an editor through `TTimbreEditorPrepare` and hands back its sound facts.
`TTimbreEnsign` builds its empty desk here too.

## `internal static CEditor TTimbreEditorPrepare(Dictionary<string, Func<object?[]?, object?>> answers)`

Builds an editor over stub ports, with its sound and settings ports answering `answers`.
The respelling checks sit on the settings port, so one map answers every sound fact.
A fact that compares levels with the display's scale reads the editor itself.
