# LAnatomyPattern.cs

## `public sealed record LAnatomyPattern(`

One way of cutting a reading into its parts: the rewrites that prepare it and the regex that cuts it.
The regex names its parts with the groups `onset`, `vowel`, `coda` and `tone`, any of which it may omit.
An omitted group reads as empty, so a toneless language simply declares no `tone` group.

**Parameters**

- `LAnatomyPatternRegex` — A .NET regex over the prepared reading, its named groups the parts.
- `LAnatomyPatternRewrites` — Rewrites run in order over the reading before the regex sees it.
  A kana table maps each kana to its letters, and a superscript table turns tone marks into digits.
- `LAnatomyPatternDecomposed` — Whether the reading is decomposed into base letters and marks before the rewrites.
  A Hangul syllable then falls into its jamo, and a nasalized vowel into the vowel and its tilde.
  Off, the reading is composed instead, so a precomposed letter stays one character.

The regex is compiled once when the record is built, so a broken pattern fails while the pack loads.
The compiled form is a private field, so it never enters the record's equality.

## `public LAnatomyPiece LAnatomyPatternResolve(string reading)`

The piece the pattern cuts from the reading, after the decomposition and the rewrites.
A reading left blank by the rewrites, or one the regex does not match, gives the empty piece.

## `public bool Equals(LAnatomyPattern? other)`

Two patterns are equal when their regex text, decomposition flag and rewrite sequence match.
The synthesized record equality would compare the compiled regex by reference and call twins unequal.

## `public override int GetHashCode()`

Hashes the regex text, the flag and the rewrite count.

## `private string LAnatomyPatternPrepare(string reading)`

The reading normalized as the flag says and run through every rewrite in order, then trimmed.
