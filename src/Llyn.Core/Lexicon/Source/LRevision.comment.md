# LRevision.cs
Hash: `ba4120f37cbc1893`

## `public sealed record LRevision(long LRevisionId, string LRevisionCreatedUtc)`

One revision is a stamped point in the workspace's history under which a batch of changes was recorded.
A revision owns its `LRevisionDelta` rows and the tombstones written under it.
It owns no lexical data, and deleting lexical data never deletes a revision.
`LRevisionId` is the identity, an opaque and program-generated stable id.

**Parameters**

- `LRevisionId` — Opaque, program-generated stable id.
- `LRevisionCreatedUtc` — Round-trip UTC timestamp of when the revision was opened.
