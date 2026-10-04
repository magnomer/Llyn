# CAnchorRow.cs
Hash: `285a2303c7372f60`

## `public sealed record CAnchorRow(long CAnchorRowId, string CAnchorRowSummary, bool CAnchorRowHeld, bool CAnchorRowEstimated)`

One fanqie reading a reflex may anchor to, as the anchor menu offers it.

**Parameters**

- `CAnchorRowId`: the reading the anchor names.
- `CAnchorRowSummary`: the reading summed up in one label.
- `CAnchorRowHeld`: whether the reflex already anchors to the reading.
- `CAnchorRowEstimated`: whether the engine only estimates the match.
