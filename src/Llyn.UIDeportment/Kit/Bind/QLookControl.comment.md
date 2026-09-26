# QLookControl.cs

## `internal static class QLookControl`

The rows of the control themes, kept apart so `QLookSheet` stays under the line ceiling.
`QLookSheet` spreads them at its end, so they read as one table with the rest.

## `internal static readonly IReadOnlyList<QLook.QLookSetter> QLookControlState =`

One row per dismantled trigger setter of the catalog, choice, command, language, paradigm, playback and volume themes.
Each style's rows stay together and in their trigger order, so the later row still wins.
