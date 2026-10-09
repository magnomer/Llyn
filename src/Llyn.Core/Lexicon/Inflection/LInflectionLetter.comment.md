# LInflectionLetter.cs
Hash: `767bc789323b28d2`

## `internal readonly record struct LInflectionLetter(char LInflectionLetterValue, int LInflectionLetterOffset, int LInflectionLetterEnd)`

One folded UTF-16 unit retains its original text span, including an empty span for zero-width replacements.
`LInflectionDifference` compares these and marks the spans of the unmatched ones.

**Parameters**

- `LInflectionLetterValue`: the folded UTF-16 code unit used for comparison.
- `LInflectionLetterOffset`: the inclusive start index in the original text.
- `LInflectionLetterEnd`: the exclusive end index in the original text.
