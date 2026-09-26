# TIndexItem.cs

## `public sealed class TIndexItem`

Covers the rows of the Library and Duplex entry lists that the deportment builds.
A built row carries the chosen mark the engine row holds.
Two rows match when only the mark differs, and refuse when the language differs.
A sync raises the change only when the mark moved.
The neighbour pick clamps at both ends, starts from the first row with none chosen, and answers null when empty.
