# TAnthologyExample.cs
Hash: `7b627fdeb76ed1de`

## `public sealed class TAnthologyExample`

Covers how the held transcript is written and shaped as an Example.
The text gate writes the typed sentence and answers the text placeholder, and the speaker gate sets the language.
A blank field matches an empty text.
A field matches the same text only when it holds no stray trailing space.
The Example map divides its excerpt at linked Mentions only while the text reads soundly.
It carries the tally it is handed, and the unknown mark as placeholder only for an unknown text.
An unknown text is worded with the unknown mark, and a never-written one with `Example.Unwritten`.
Only the never-written text is muted.
The tests that need an anthology run on a real workspace and build it through `TAnthology.TAnthologyPrepare`.
