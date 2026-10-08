# TAnthologyGloss.cs
Hash: `9359d7893d79d281`

## `public sealed class TAnthologyGloss`

Covers the gloss gates of the held transcript over a corpus desk on a real workspace.
The transcript's gloss gates add below a row in the gloss language, or before the first row.
They seed a first gloss only into an empty held transcript.
They write a gloss text, retag its language and drop it.
Each test builds its list through `TAnthology.TAnthologyPrepare`.

## `private static IReadOnlyList<CGlossDraft> TAnthologyGlossRead(CAnthology anthology, CDesk desk)`

The glosses the held transcript carries, or none while it is not held.
