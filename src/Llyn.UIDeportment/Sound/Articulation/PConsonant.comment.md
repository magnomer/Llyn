# PConsonant.cs

## `public partial class PArticulation`

The pulmonic consonant chart of the articulation aid.
Rows are manner of articulation and columns are place of articulation.
A cell holds the voiceless consonant and then the voiced one.

## Inline notes

### `private static readonly string[,] PConsonantCharacter`

An empty cell is a position judged impossible or one the IPA gives no symbol for.
It is kept as a cell so that the rows and columns stay aligned with their headers.
