# PVowel.cs

## `public partial class PArticulation`

The vowel chart of the articulation aid.
Rows are tongue height and columns are tongue backness, as the IPA chart arranges them.
A cell holds the unrounded vowel and then the rounded one.

## Inline notes

### `private static readonly string[,] PVowelCharacter`

An empty cell is a position the IPA leaves empty or gives no ordinary symbol.
It is kept as a cell so that the rows and columns stay aligned with their headers.
