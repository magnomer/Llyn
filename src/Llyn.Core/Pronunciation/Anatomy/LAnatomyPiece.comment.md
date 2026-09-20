# LAnatomyPiece.cs

## `public sealed record LAnatomyPiece(`

One cut of one reading: the four parts a pattern reads off it.
Two pieces, one from the reading and one from its respelling, make one [LAnatomy](LAnatomy.comment.md).

**Parameters**

- `LAnatomyPieceOnset` — The text the `onset` group matched, or empty.
- `LAnatomyPieceVowel` — The text the `vowel` group matched, or empty.
- `LAnatomyPieceCoda` — The text the `coda` group matched, or empty.
- `LAnatomyPieceTone` — The text the `tone` group matched, or empty.

## `public static readonly LAnatomyPiece LAnatomyPieceEmpty = new();`

The piece of a reading the pattern does not match, every part empty.
