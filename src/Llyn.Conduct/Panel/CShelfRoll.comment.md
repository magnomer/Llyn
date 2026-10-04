# CShelfRoll.cs
Hash: `5b98cc881a816079`

## `public sealed record CShelfRoll(IReadOnlyList<CCatalogReference> CShelfRollRows, bool CShelfRollEmpty, string CShelfRollTally)`

The shelf's rows answer, ready to paint in one go.
The tally comes with the rows, so the view never reads the shelf a second time.

**Parameters**

- `CShelfRollRows`: the Sources as the view lists them.
- `CShelfRollEmpty`: whether the shelf lists no Source, so the empty notice shows.
- `CShelfRollTally`: the worded tally of the chosen Source, read after a stale choice closes.
