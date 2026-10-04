# CTimbreReflex.cs
Hash: `e4f94ce4e8a030d8`

## `public sealed record CTimbreReflex(bool CTimbreReflexShown, IReadOnlyList<CReflex> CTimbreReflexRows, CLecternAnchor CTimbreReflexAnchor, bool CTimbreReflexOpened, bool CTimbreReflexPending)`

The editor's reflex block for the held draft, ready to paint.

**Parameters**

- `CTimbreReflexShown`: whether the block shows, when the pack declares a rule or the draft holds a row.
- `CTimbreReflexRows`: every reflex row of the draft, each resolved by the shared reflex scan.
- `CTimbreReflexAnchor`: the anchor text of each row and whether anchoring is offered.
- `CTimbreReflexOpened`: whether the folded rows show, shared with every reading view.
- `CTimbreReflexPending`: whether a reflex fill still runs, which shows the fetching line.
