# CMentionMark.cs
Hash: `e508634d8bf3ccb9`

## `public sealed record CMentionMark(long CMentionMarkEntry, long CMentionMarkSense)`

The link of the stored Mention a click found, which the mention area opens.
It keeps only the entry and the sense, since the engine already settled the span.

**Parameters**

- `CMentionMarkEntry`: the entry the Mention links to, zero when unlinked.
- `CMentionMarkSense`: the sense the Mention links to, zero when none.
