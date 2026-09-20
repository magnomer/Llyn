# LHypothesisSound.cs

## `public sealed record LHypothesisSound(string LHypothesisSoundText, string LHypothesisSoundClass);`

What the hypothesis gives for one placement: the reconstructed syllable and its tone class.

**Parameters**

- `LHypothesisSoundText` — The syllable with its tone rewrites applied, such as `ngoʔ`.
- `LHypothesisSoundClass` — The tone class key, such as `4S`, or empty when the tone had no matching row.
