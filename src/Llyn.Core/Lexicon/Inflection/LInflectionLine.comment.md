# LInflectionLine.cs
Hash: `e5849ac49a233a0f`

## `public sealed record LInflectionLine(string LInflectionLineGroup, string LInflectionLineLabel, IReadOnlyList<IReadOnlyList<long>> LInflectionLineCells)`

One row of a view, with the cells it shows in order.
The group label sits on the first line of its group only, so later lines carry an empty group.
Loader-built cells contain distinct ascending codes, enabling exact sequence matching against slot codes.

**Parameters**

- `LInflectionLineGroup`: the group label key on the first line of a group, otherwise empty.
- `LInflectionLineLabel`: the row label key, or empty for an unlabelled row.
- `LInflectionLineCells`: the cells of the row, each a sorted list of pack value codes.
