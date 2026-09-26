# PLookSound.cs

## `internal static class PLookSound`

The rows of the sound themes, kept apart so `PLookSheet` stays under the line ceiling.
`PLookSheet` spreads them at its end, so they read as one table with the rest.

## `internal static readonly IReadOnlyList<PLook.PLookSetter> PLookSoundState =`

One row per dismantled trigger, command, template binding or icon of the sound themes.
A derived style carries only its own rows, since `PLook` also applies its base styles' rows.
A command parameter that was the row itself is a copy of the control's data context.
A chip's text that was its item copies the data context too, so a string list needs no fill.
The hover pair of a pronunciation row is shown by the surface's hover and focus rows.
The rebuild icon spins while its button's tag reads `Pending`, and stops when the tag clears.
The two switch halves send `false` and `true` as their parameter, so the markup holds no flag resource.
A representative star reads `Marked` or `Faded` from its tag, which the line fill sets.
A link or anchor with empty content folds away or shows its prompt through the `Empty` state.
The articulation glyph is a style local to its markup, which `PArticulation` registers itself.
