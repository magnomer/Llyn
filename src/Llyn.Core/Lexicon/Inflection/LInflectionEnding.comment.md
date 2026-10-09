# LInflectionEnding.cs
Hash: `1508c10daab8aea5`

## `public sealed record LInflectionEnding(IReadOnlyList<long> LInflectionEndingValues, string LInflectionEndingText)`

The ending of one column in a stem's row.
The text may carry operator characters that only the book's rules give meaning.

**Parameters**

- `LInflectionEndingValues`: the column's pack value codes, such as person and number.
- `LInflectionEndingText`: the raw ending appended after the expanded stem.
