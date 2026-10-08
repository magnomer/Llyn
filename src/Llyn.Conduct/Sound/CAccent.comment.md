# CAccent.cs
Hash: `39170fc0b7e94529`

## `public sealed record CAccent(long CAccentId, CVariety CAccentVariety, string CAccentText, string CAccentAudio);`

One pronunciation row after the primary, ready to show.

**Parameters**

- `CAccentId`: the draft pronunciation the row stands for.
- `CAccentVariety`: the row's variety with its label and flag keys.
- `CAccentText`: the form the respelling mark picks, already resolved.
- `CAccentAudio`: the recording's address, empty when none is held.

## `internal static CAccent CAccentRead(string language, LAccentRow row)`

Maps one engine accent row into the ready row, reading no rule.
The editor's sheet and the reading view's block share it.
