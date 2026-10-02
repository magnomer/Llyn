# CTimbreAccent.cs
Hash: `d8db05a6ce76e326`

## `public sealed record CTimbreAccent(CRespellingMark CTimbreAccentMark, CVariety CTimbreAccentPrimary, IReadOnlyList<CAccent> CTimbreAccentRows, bool CTimbreAccentFlagged)`

The editor's accent sheet, ready to paint.

**Parameters**

- `CTimbreAccentMark`: whether the rows print respellings, and the brackets they wear.
- `CTimbreAccentPrimary`: the primary pronunciation's variety.
- `CTimbreAccentRows`: every pronunciation after the primary, its text already in the form the mark picks.
- `CTimbreAccentFlagged`: whether the language draws its varieties as flags.
