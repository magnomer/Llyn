# CAccent.cs

## `public sealed record CAccent(long CAccentId, CVariety CAccentVariety, string CAccentText, string CAccentAudio)`

One pronunciation row after the primary, ready to show.

**Parameters**

- `CAccentId`: the draft pronunciation the row stands for.
- `CAccentVariety`: the row's variety with its label and flag keys.
- `CAccentText`: the form the respelling mark picks, already resolved.
- `CAccentAudio`: the recording's address, empty when none is held.
