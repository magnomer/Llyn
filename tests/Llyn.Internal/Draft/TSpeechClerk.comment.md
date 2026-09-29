# TSpeechClerk.cs

## `public sealed class TSpeechClerk`

Covers the part of speech rules with no engine.
Typed text is appended trimmed and once, and blank text adds nothing.
A declared name carries its catalog value, and a refused one is kept as typed.
A blank or held name declares nothing.
An erased chip leaves alone.
A draft that agrees keeps the pending text, and one changed elsewhere is cleaned and clears it.
Only a typed text that appends a part is pending, and the chips leave the pending last part out.

## `private static string TSpeechNameRead(LSpeechDraft speech)`

The name a part of speech shows.
