# PSwathSeam.cs
Hash: `7d4ab1291ea7b670`

## `public readonly record struct PSwathSeam(int PSwathSeamIndex, TextPointer? PSwathSeamCaret)`

One end of the band: the index of a page item and, for a text block, the position inside it.
An item that is not text carries no position, since it is taken whole or not at all.
It stays a value, so an end is copied rather than shared between the anchor, head and tail.

### `public TextPointer PSwathSeamStart(int index, TextBlock block)`

Where the band starts inside `block`, read as the head end.
The head's own block starts at its position, and any later block starts at its beginning.

### `public TextPointer PSwathSeamFinish(int index, TextBlock block)`

Where the band ends inside `block`, read as the tail end.
The tail's own block ends at its position, and any earlier block ends at its end.
