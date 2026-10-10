# QLookSound.cs
Hash: `369c1c12b487a6e6`

## `internal static class QLookSound`

The rows of the sound themes, kept apart so `QLookSheet` stays under the line ceiling.
`QLookSheet` spreads them at its end, so they read as one table with the rest.

## `internal static readonly IReadOnlyList<QLook.QLookSetter> QLookSoundState =`

One row per dismantled trigger, command, template binding or icon of the sound themes.
A derived style carries only its own rows, since `QLook` also applies its base styles' rows.
The series hinge `Theme.Stem.Fold` overrides only the checked text of `Theme.Reflex.Fold`, as its derived row wins.
A command parameter that was the row itself is a copy of the control's data context.
A chip's text that was its item copies the data context too, so a string list needs no fill.
The hover pair of a pronunciation row is shown by the surface's hover and focus rows.
The rebuild icon spins while its button carries the `Pending` cue, and stops when the cue clears.
The spin is the veneer resource `Theme.Sound.Rebuild.Spin`, pulled by contract ID.
The two switch halves send `false` and `true` as their parameter, so the markup holds no flag resource.
A representative star reads the `Marked` or `Faded` cue, which the line fill sets.
A link or anchor with empty content folds away or shows its prompt through the `Empty` state.
The articulation glyph is a style local to its markup, which `QArticulation` registers itself.
