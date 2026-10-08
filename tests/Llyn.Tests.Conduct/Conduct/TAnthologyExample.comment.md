# TAnthologyExample.cs
Hash: `8e6841754c13f727`

## `public sealed class TAnthologyExample`

Covers how the held transcript is written and shaped as an Example.
The text gate writes the typed sentence and answers the text placeholder, and the speaker gate sets the language.
A blank field matches an empty text.
A field matches the same text only when it holds no stray trailing space.
The Example map divides its excerpt at linked Mentions only while the text reads soundly.
It carries the tally it is handed, and the unknown mark as placeholder only for an unknown text.
An unknown text is worded with the unknown mark, and a never-written one with `Example.Unwritten`.
Only the never-written text is muted.
The text and speaker test runs on a real workspace and builds its anthology through `TAnthology.TAnthologyPrepare`.
