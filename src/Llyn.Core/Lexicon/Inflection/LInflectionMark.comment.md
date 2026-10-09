# LInflectionMark.cs
Hash: `5122222b0e97fbcf`

## `public sealed record LInflectionMark(int LInflectionMarkOffset, int LInflectionMarkLength)`

One range of the actual form that the rules did not predict.
Offsets and lengths count UTF-16 code units of the stored form, not its folded copy.

**Parameters**

- `LInflectionMarkOffset`: the starting UTF-16 index in the actual form.
- `LInflectionMarkLength`: the marked length in UTF-16 code units.

## `public static string LInflectionMarkFormat(IReadOnlyList<LInflectionMark> marks)`

Writes `marks` as `offset:length` pairs joined by commas, in invariant culture.
No marks give the empty string, which differs from a missing analysis.

## `public static IReadOnlyList<LInflectionMark> LInflectionMarkParse(string text)`

Reads the pairs `LInflectionMarkFormat` writes.
Malformed, signed, whitespace-padded, or overflowing pairs are skipped independently.
Zero-length ranges are accepted, and parsing does not validate bounds against form text.
