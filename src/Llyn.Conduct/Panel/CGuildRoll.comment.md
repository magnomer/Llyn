# CGuildRoll.cs
Hash: `4d3eb66139756c29`

## `public sealed record CGuildRoll(IReadOnlyList<CCatalogAuthor> CGuildRollRows, bool CGuildRollEmpty, CVita CGuildRollVita, IReadOnlyList<CReferenceKind> CGuildRollKind)`

The guild's roll answer, ready to paint in one go.
The vita comes with the rows, so the view never reads the sheet a second time.
The kind menu comes with them too, so the workspace opening paints from one read.

**Parameters**

- `CGuildRollRows`: the roll as the view lists it, each count worded.
- `CGuildRollEmpty`: whether the roll lists no row, so the empty notice shows.
- `CGuildRollVita`: the read sheet of the chosen Author, or the sheet of nobody while none is stored.
- `CGuildRollKind`: the kind menu the filter lists, each repeated tag dropped.
