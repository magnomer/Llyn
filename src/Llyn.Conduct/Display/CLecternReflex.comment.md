# CLecternReflex.cs
Hash: `856e330e06d4a634`

## `public sealed record CLecternReflex(IReadOnlyList<CReflex> CLecternReflexRows, CLecternAnchor CLecternReflexAnchor, bool CLecternReflexPending)`

The reflex block of the reading view for the shown entry, ready to show.

**Parameters**

- `CLecternReflexRows`: the written reflex rows, each resolved by the shared reflex scan.
- `CLecternReflexAnchor`: the anchor text of each row and whether anchoring is offered.
- `CLecternReflexPending`: whether a reflex fill still runs, which shows the loading line.
