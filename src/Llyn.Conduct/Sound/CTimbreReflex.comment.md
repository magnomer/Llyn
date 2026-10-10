# CTimbreReflex.cs
Hash: `822e57a2173be39b`

## `public sealed record CTimbreReflex(bool CTimbreReflexShown, IReadOnlyList<CReflex> CTimbreReflexRows, CLecternAnchor CTimbreReflexAnchor, bool CTimbreReflexOpened, bool CTimbreReflexPending, bool CTimbreReflexFoldable)`

The editor's reflex block for the held draft, ready to paint.

**Parameters**

- `CTimbreReflexShown`: whether the block shows, when the pack declares a rule or the draft holds a row.
- `CTimbreReflexRows`: every reflex row of the draft, each resolved by the shared reflex scan.
- `CTimbreReflexAnchor`: the anchor text of each row and whether anchoring is offered.
- `CTimbreReflexOpened`: whether the folded rows show, as the engine stores it for the held entry.
- `CTimbreReflexPending`: whether a reflex fill still runs, which shows the fetching line.
- `CTimbreReflexFoldable`: whether any row folds, which shows the fold toggle.
